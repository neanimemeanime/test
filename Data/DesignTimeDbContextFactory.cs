// Файл: Data/DesignTimeDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics; // <-- Добавь этот using
using System;
using System.IO;

namespace RepairServiceAppMVVM.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            string dbPath = Path.Combine(AppContext.BaseDirectory, "repairs_mvvm.db");

            // ИСПРАВЛЕНО: Добавляем .ConfigureWarnings(...)
            optionsBuilder.UseSqlite($"Data Source={dbPath}")
                          .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}