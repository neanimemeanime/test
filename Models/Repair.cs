// Файл: Models/Repair.cs
using RepairServiceAppMVVM.ViewModels; // Используем наш ViewModelBase
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairServiceAppMVVM.Models
{
    public class Repair : ViewModelBase
    {
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        private int _clientId;
        public int ClientId { get => _clientId; set => SetProperty(ref _clientId, value); }
        public Client? Client { get; set; }

        private int _deviceId;
        public int DeviceId { get => _deviceId; set => SetProperty(ref _deviceId, value); }
        public Device? Device { get; set; }

        private int _serviceTypeId;
        public int ServiceTypeId { get => _serviceTypeId; set => SetProperty(ref _serviceTypeId, value); }
        public ServiceType? ServiceType { get; set; }

        private DateTime _dateReceived;
        public DateTime DateReceived { get => _dateReceived; set => SetProperty(ref _dateReceived, value); }

        private DateTime? _dateDue;
        public DateTime? DateDue { get => _dateDue; set => SetProperty(ref _dateDue, value); }

        private DateTime? _dateCompleted;
        public DateTime? DateCompleted { get => _dateCompleted; set => SetProperty(ref _dateCompleted, value); }

        private string _status = string.Empty;
        public string Status { get => _status; set => SetProperty(ref _status, value); }

        private decimal _totalCost;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCost { get => _totalCost; set => SetProperty(ref _totalCost, value); }

        private int? _assignedToUserId;
        public int? AssignedToUserId { get => _assignedToUserId; set => SetProperty(ref _assignedToUserId, value); }
        public User? AssignedToUser { get; set; }

        private string? _problemDescription;
        public string? ProblemDescription { get => _problemDescription; set => SetProperty(ref _problemDescription, value); }

        private string? _workPerformed;
        public string? WorkPerformed { get => _workPerformed; set => SetProperty(ref _workPerformed, value); }

        private string? _partsUsed;
        public string? PartsUsed { get => _partsUsed; set => SetProperty(ref _partsUsed, value); }

        private string? _notes;
        public string? Notes { get => _notes; set => SetProperty(ref _notes, value); }
    }
}
