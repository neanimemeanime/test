// Файл: Models/Device.cs
using System.ComponentModel.DataAnnotations;

namespace RepairServiceAppMVVM.Models
{
    // Класс устройства, которое принесли в ремонт
    public class Device
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; }

        [Required]
        [MaxLength(100)]
        public string Manufacturer { get; set; }

        [Required]
        [MaxLength(100)]
        public string Model { get; set; }

        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        public string? ProblemDescription { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
    }
}