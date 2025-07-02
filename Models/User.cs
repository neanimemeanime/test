// Файл: Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace RepairServiceAppMVVM.Models
{
    // Класс пользователя системы (для авторизации)
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; }
    }
}