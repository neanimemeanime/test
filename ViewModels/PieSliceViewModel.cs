// Файл: ViewModels/PieSliceViewModel.cs
using System.Windows.Media;

namespace RepairServiceAppMVVM.ViewModels
{
    public class PieSliceViewModel : ViewModelBase
    {
        private string _title = string.Empty;
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        private double _percentage;
        public double Percentage { get => _percentage; set => SetProperty(ref _percentage, value); }

        private Brush _fill = Brushes.Transparent;
        public Brush Fill { get => _fill; set => SetProperty(ref _fill, value); }

        // Новое свойство для правильной отрисовки дуг > 180 градусов
        public bool IsLargeArc { get; set; }

        public PointCollection Points { get; set; } = new PointCollection();
    }
}