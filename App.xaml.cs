// Файл: App.xaml.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics; // <-- Добавь этот using
using Microsoft.Extensions.DependencyInjection;
using RepairServiceAppMVVM.Data;
using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Services;
using RepairServiceAppMVVM.ViewModels;
using RepairServiceAppMVVM.Views;
using System;
using System.IO;
using System.Windows;

namespace RepairServiceAppMVVM
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            string dbPath = Path.Combine(AppContext.BaseDirectory, "repairs_mvvm.db");

            // ИСПРАВЛЕНО: Добавляем .ConfigureWarnings(...)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}")
                       .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            );

            // Регистрация сервисов
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IServiceTypeService, ServiceTypeService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IDeviceService, DeviceService>();
            services.AddTransient<IRepairService, RepairService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<ICsvExportService, CsvExportService>();
            services.AddTransient<IPrintingService, PrintingService>();
            services.AddTransient<IDataSeedingService, DataSeedingService>();

            // Регистрация ViewModel
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();

                var seeder = scope.ServiceProvider.GetRequiredService<IDataSeedingService>();
                await seeder.SeedDataAsync();
            }

            var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
            var loginView = new LoginView { DataContext = loginViewModel };

            loginViewModel.LoginSuccess += (user) =>
            {
                var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
                mainViewModel.Initialize(user);

                var mainView = new MainView { DataContext = mainViewModel };
                mainView.Show();
                loginView.Close();
            };

            loginView.Show();
        }
    }
}