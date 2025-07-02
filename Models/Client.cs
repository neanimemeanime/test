// Файл: Models/Client.cs
using RepairServiceAppMVVM.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RepairServiceAppMVVM.Models
{
    public class Client : ViewModelBase
    {
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        private string _firstName = string.Empty;
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }

        private string _lastName = string.Empty;
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }

        private string? _middleName;
        public string? MiddleName { get => _middleName; set => SetProperty(ref _middleName, value); }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            // ИСПРАВЛЕНО: Теперь модель всегда хранит только чистые цифры
            set => SetProperty(ref _phoneNumber, new string(value.Where(char.IsDigit).ToArray()));
        }

        private string? _email;
        public string? Email { get => _email; set => SetProperty(ref _email, value); }

        public ICollection<Device> Devices { get; set; } = new List<Device>();

        [NotMapped]
        public string FullName => !string.IsNullOrWhiteSpace(MiddleName) ? $"{LastName} {FirstName} {MiddleName}" : $"{LastName} {FirstName}";
    }
}
