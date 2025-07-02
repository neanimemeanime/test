// Файл: Services/DataSeedingService.cs
using Microsoft.EntityFrameworkCore;
using RepairServiceAppMVVM.Data;
using RepairServiceAppMVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public class DataSeedingService : IDataSeedingService
    {
        private readonly ApplicationDbContext _dbContext;

        public DataSeedingService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SeedDataAsync()
        {
            if (await _dbContext.Clients.AnyAsync())
            {
                return;
            }

            var tech1 = new User { Username = "ivan_tech", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"), Role = "Техник" };
            var tech2 = new User { Username = "petr_tech", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"), Role = "Техник" };
            var manager1 = new User { Username = "anna_manager", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"), Role = "Менеджер" };
            var users = new List<User> { tech1, tech2, manager1 };
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            var clients = new List<Client>
            {
                new Client { FirstName = "Александр", LastName = "Иванов", PhoneNumber = "79161112233", Email = "ivanov@email.com" },
                new Client { FirstName = "Елена", LastName = "Петрова", PhoneNumber = "79262223344", Email = "petrova@email.com" },
                new Client { FirstName = "Дмитрий", LastName = "Сидоров", PhoneNumber = "79033334455", Email = "sidorov@email.com" },
                new Client { FirstName = "Ольга", LastName = "Смирнова", PhoneNumber = "79154445566", Email = "smirnova@email.com" },
                new Client { FirstName = "Сергей", LastName = "Кузнецов", PhoneNumber = "79255556677", Email = "kuznetsov@email.com" }
            };
            await _dbContext.Clients.AddRangeAsync(clients);
            await _dbContext.SaveChangesAsync();

            var devices = new List<Device>
            {
                new Device { ClientId = clients[0].Id, Type = "Смартфон", Manufacturer = "Apple", Model = "iPhone 14 Pro", SerialNumber = "SN-A14P-001", ProblemDescription = "Не заряжается" },
                new Device { ClientId = clients[0].Id, Type = "Ноутбук", Manufacturer = "Apple", Model = "MacBook Pro 16", SerialNumber = "SN-MBP16-002", ProblemDescription = "Залипает клавиатура" },
                new Device { ClientId = clients[1].Id, Type = "Смартфон", Manufacturer = "Samsung", Model = "Galaxy S23 Ultra", SerialNumber = "SN-SGS23-003", ProblemDescription = "Разбит экран" },
                new Device { ClientId = clients[2].Id, Type = "Планшет", Manufacturer = "Xiaomi", Model = "Pad 6", SerialNumber = "SN-XP6-004", ProblemDescription = "Быстро разряжается" },
                new Device { ClientId = clients[3].Id, Type = "Умные часы", Manufacturer = "Garmin", Model = "Fenix 7X", SerialNumber = "SN-GF7-005", ProblemDescription = "Не ловит GPS" },
                new Device { ClientId = clients[4].Id, Type = "Наушники", Manufacturer = "Sony", Model = "WH-1000XM5", SerialNumber = "SN-SWH5-006", ProblemDescription = "Не работает правый наушник" }
            };
            await _dbContext.Devices.AddRangeAsync(devices);
            await _dbContext.SaveChangesAsync();

            var serviceTypes = await _dbContext.ServiceTypes.ToListAsync();
            var newServiceTypes = new List<ServiceType>
            {
                new ServiceType { Name = "Замена аккумулятора", DefaultPrice = 2500.00m, Description = "Установка новой батареи" },
                new ServiceType { Name = "Чистка от пыли", DefaultPrice = 1500.00m, Description = "Полная внутренняя чистка системы охлаждения" },
                new ServiceType { Name = "Ремонт материнской платы", DefaultPrice = 8000.00m, Description = "Сложный компонентный ремонт" }
            };
            await _dbContext.ServiceTypes.AddRangeAsync(newServiceTypes);
            await _dbContext.SaveChangesAsync();
            serviceTypes.AddRange(newServiceTypes);

            var repairs = new List<Repair>();
            var random = new Random();
            var statuses = new[] { "В ожидании", "В работе", "Диагностика", "Ожидает запчасти", "Готов к выдаче", "Выдан", "Отменен" };

            for (int i = 0; i < 35; i++)
            {
                var randomDevice = devices[random.Next(devices.Count)];
                var randomService = serviceTypes[random.Next(serviceTypes.Count)];
                // ИСПРАВЛЕНО: statuses - это массив, используем .Length
                var randomStatus = statuses[random.Next(statuses.Length)];
                var assignedUser = random.Next(0, 2) == 1 ? users[random.Next(users.Count)] : null;
                var dateReceived = DateTime.Now.AddDays(-random.Next(1, 365));

                repairs.Add(new Repair
                {
                    ClientId = randomDevice.ClientId,
                    DeviceId = randomDevice.Id,
                    ServiceTypeId = randomService.Id,
                    DateReceived = dateReceived,
                    DateDue = dateReceived.AddDays(random.Next(3, 14)),
                    DateCompleted = randomStatus == "Выдан" ? dateReceived.AddDays(random.Next(2, 10)) : (DateTime?)null,
                    Status = randomStatus,
                    TotalCost = randomService.DefaultPrice + random.Next(-500, 1000),
                    AssignedToUserId = assignedUser?.Id,
                    ProblemDescription = randomDevice.ProblemDescription,
                    WorkPerformed = randomStatus == "Выдан" || randomStatus == "Готов к выдаче" ? "Работы выполнены согласно регламенту." : "В процессе...",
                    PartsUsed = "Запчасти не использовались",
                    Notes = "Без примечаний."
                });
            }
            await _dbContext.Repairs.AddRangeAsync(repairs);
            await _dbContext.SaveChangesAsync();
        }
    }
}