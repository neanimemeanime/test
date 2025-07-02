using RepairServiceAppMVVM.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation; // Добавили для анимации

namespace RepairServiceAppMVVM.Views
{
    public partial class PieChartControl : UserControl
    {
        public PieChartControl()
        {
            InitializeComponent();
            // Убедимся, что начальная прозрачность 0, чтобы анимация fadeIn сработала при первом появлении
            this.Opacity = 0;
        }

        public static readonly DependencyProperty SlicesProperty =
            DependencyProperty.Register("Slices", typeof(ObservableCollection<PieSliceViewModel>), typeof(PieChartControl),
            new PropertyMetadata(null, OnSlicesChanged));

        public ObservableCollection<PieSliceViewModel> Slices
        {
            get { return (ObservableCollection<PieSliceViewModel>)GetValue(SlicesProperty); }
            set { SetValue(SlicesProperty, value); }
        }

        private static void OnSlicesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PieChartControl control)
            {
                if (e.OldValue is ObservableCollection<PieSliceViewModel> oldCollection)
                {
                    oldCollection.CollectionChanged -= control.OnSlicesCollectionChanged;
                }
                if (e.NewValue is ObservableCollection<PieSliceViewModel> newCollection)
                {
                    newCollection.CollectionChanged += control.OnSlicesCollectionChanged;
                }
                // Вызываем UpdateChart при изменении коллекции
                control.UpdateChart();
            }
        }

        private void OnSlicesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            var itemsControl = this.FindName("PieItemsControl") as ItemsControl;
            if (itemsControl == null) return;

            // Если данных нет, очищаем и скрываем диаграмму, делаем ее прозрачной
            if (Slices == null || Slices.Count == 0)
            {
                itemsControl.ItemsSource = null;
                // Анимируем исчезновение
                DoubleAnimation fadeOutAnimation = new DoubleAnimation(this.Opacity, 0, TimeSpan.FromSeconds(0.2));
                fadeOutAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };
                this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                return;
            }

            // Пересчитываем геометрию для каждого среза
            double currentAngle = -90; // Начинаем с верхней точки (-90 градусов)
            double radius = 100; // Радиус диаграммы. Если изменяете Canvas в XAML, измените и здесь.
            Point center = new Point(radius, radius); // Центр Canvas (150,150 для радиуса 150)

            foreach (var slice in Slices)
            {
                double angle = slice.Percentage * 360;

                // Вычисляем начальную и конечную точки сегмента
                Point startPoint = new Point(center.X + radius * Math.Cos(currentAngle * Math.PI / 180),
                                             center.Y + radius * Math.Sin(currentAngle * Math.PI / 180));

                currentAngle += angle; // Переходим к следующему углу

                Point endPoint = new Point(center.X + radius * Math.Cos(currentAngle * Math.PI / 180),
                                           center.Y + radius * Math.Sin(currentAngle * Math.PI / 180));

                slice.Points = new PointCollection { startPoint, endPoint };
                slice.IsLargeArc = angle > 180; // Проверяем, нужна ли большая дуга
            }

            // Принудительная перерисовка ItemsControl
            // Этот трюк нужен, чтобы Path в ItemTemplate перечитали свойства Points и IsLargeArc
            itemsControl.ItemsSource = null;
            itemsControl.ItemsSource = this.Slices;

            // Анимируем появление всего контрола
            DoubleAnimation fadeInAnimation = new DoubleAnimation(this.Opacity, 1, TimeSpan.FromSeconds(0.4));
            fadeInAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            this.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }
    }
}