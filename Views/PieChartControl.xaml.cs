// Файл: Views/PieChartControl.xaml.cs
using RepairServiceAppMVVM.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RepairServiceAppMVVM.Views
{
    public partial class PieChartControl : UserControl
    {
        public PieChartControl()
        {
            InitializeComponent();
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
                // Отписываемся от старой коллекции, если она была
                if (e.OldValue is ObservableCollection<PieSliceViewModel> oldCollection)
                {
                    oldCollection.CollectionChanged -= control.OnSlicesCollectionChanged;
                }
                // Подписываемся на новую коллекцию
                if (e.NewValue is ObservableCollection<PieSliceViewModel> newCollection)
                {
                    newCollection.CollectionChanged += control.OnSlicesCollectionChanged;
                }
                control.UpdateChart();
            }
        }

        // Этот метод будет вызываться при любом изменении коллекции
        private void OnSlicesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            if (Slices == null || Slices.Count == 0)
            {
                // Очищаем Canvas, если данных нет
                if (this.Content is ItemsControl itemsControl)
                {
                    itemsControl.ItemsSource = null;
                    itemsControl.ItemsSource = Slices;
                }
                return;
            }

            double currentAngle = -90; // Начинаем сверху
            double radius = 150;
            Point center = new Point(radius, radius);
            double totalPercentage = 0;

            foreach (var slice in Slices)
            {
                totalPercentage += slice.Percentage;
                double angle = slice.Percentage * 360;

                Point startPoint = new Point(
                    center.X + radius * Math.Cos(currentAngle * Math.PI / 180),
                    center.Y + radius * Math.Sin(currentAngle * Math.PI / 180)
                );

                currentAngle += angle;

                Point endPoint = new Point(
                    center.X + radius * Math.Cos(currentAngle * Math.PI / 180),
                    center.Y + radius * Math.Sin(currentAngle * Math.PI / 180)
                );

                slice.Points.Clear();
                slice.Points.Add(startPoint);
                slice.Points.Add(endPoint);
                // Указываем, является ли дуга большей 180 градусов
                slice.IsLargeArc = angle > 180;
            }

            // Обновляем ItemsSource, чтобы WPF перерисовал элементы
            if (this.Content is ItemsControl ic)
            {
                ic.ItemsSource = null;
                ic.ItemsSource = Slices;
            }
        }
    }
}
