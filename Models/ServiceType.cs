// Файл: Models/ServiceType.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairServiceAppMVVM.Models
{
    // Класс для типов услуг
    public class ServiceType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DefaultPrice { get; set; }

        public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
    }
}