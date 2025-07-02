//PieSliceViewModel
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace RepairServiceAppMVVM.ViewModels
{
    public class PieSliceViewModel : INotifyPropertyChanged
    {
        private Brush _brush;
        private string _title;
        private double _percentage;
        private int _count;
        private bool _isLargeArc;
        private PointCollection _points = new PointCollection();
        private double _startAngle;
        public double StartAngle
        {
            get => _startAngle;
            set
            {
                if (_startAngle != value)
                {
                    _startAngle = value;
                    OnPropertyChanged(nameof(StartAngle));
                    // При изменении угла пересчитываем Points (для отрисовки)
                    UpdateGeometry();
                }
            }
        }

        private double _endAngle;
        public double EndAngle
        {
            get => _endAngle;
            set
            {
                if (_endAngle != value)
                {
                    _endAngle = value;
                    OnPropertyChanged(nameof(EndAngle));
                    // При изменении угла пересчитываем Points (для отрисовки)
                    UpdateGeometry();
                }
            }
        }


        public Brush Brush
        {
            get => _brush;
            set { _brush = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public double Percentage
        {
            get => _percentage;
            set { _percentage = value; OnPropertyChanged(); }
        }

        public int Count
        {
            get => _count;
            set { _count = value; OnPropertyChanged(); }
        }

        // Это свойство будет указывать, является ли дуга большей 180 градусов
        public bool IsLargeArc
        {
            get => _isLargeArc;
            set
            {
                if (_isLargeArc != value)
                {
                    _isLargeArc = value;
                    OnPropertyChanged(nameof(IsLargeArc));
                }
            }
        }

        // Коллекция для хранения точек, описывающих сектор
        public PointCollection Points
        {
            get => _points;
            set
            {
                if (_points != value)
                {
                    _points = value;
                    OnPropertyChanged(nameof(Points));
                }
            }
        }

        private void UpdateGeometry()
        {
            double radius = 150; // Должен совпадать с радиусом в PieChartControl
            Point center = new Point(radius, radius);

            // Используем текущие анимированные углы для расчета точек
            Point startPoint = new Point(center.X + radius * Math.Cos(StartAngle * Math.PI / 180), center.Y + radius * Math.Sin(StartAngle * Math.PI / 180));
            Point endPoint = new Point(center.X + radius * Math.Cos(EndAngle * Math.PI / 180), center.Y + radius * Math.Sin(EndAngle * Math.PI / 180));

            Points = new PointCollection { startPoint, endPoint };
            IsLargeArc = (EndAngle - StartAngle) > 180;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
