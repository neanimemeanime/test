// Файл: Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using RepairServiceAppMVVM.Models;
using BCrypt.Net;

namespace RepairServiceAppMVVM.Data
{
    public class ApplicationDbContext : DbContext
    {
        // ИСПРАВЛЕНО: Убираем пустой конструктор и OnConfiguring,
        // так как конфигурация теперь происходит снаружи.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<Repair> Repairs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<ServiceType>().HasData(
                new ServiceType { Id = 1, Name = "Диагностика", DefaultPrice = 500.00m, Description = "Первичная диагностика неисправности устройства." },
                new ServiceType { Id = 2, Name = "Замена экрана", DefaultPrice = 5500.00m, Description = "Замена поврежденного дисплейного модуля." }
            );

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin");
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", PasswordHash = hashedPassword, Role = "Администратор" }
            );
        }
    }
}