using MediTrack.Core.Data;
using MediTrack.Core.Services;
using MediTrack.UI.ViewModels;
using MediTrack.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;

namespace MediTrack.UI;

public partial class App : System.Windows.Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += (_, args) =>
        {
            var ex = args.Exception;
            while (ex.InnerException != null) ex = ex.InnerException;
            System.IO.File.WriteAllText("startup-error.log", $"{DateTime.Now}: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            System.Windows.MessageBox.Show($"Error: {ex.GetType().Name}\n{ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
            Shutdown(1);
        };

        var collection = new ServiceCollection();
        ConfigureServices(collection);
        Services = collection.BuildServiceProvider();

        var mainWindow = new MainWindow(Services.GetRequiredService<MainViewModel>());
        MainWindow = mainWindow;

        var nav = Services.GetRequiredService<Services.INavigationService>();
        var splash = Services.GetRequiredService<SplashViewModel>();
        nav.NavigateTo(splash);

        var trayService = new Services.TrayService(mainWindow);
        var reminderEngine = Services.GetRequiredService<IReminderEngine>();
        var authService = Services.GetRequiredService<IAuthService>();
        var intakeLogService = Services.GetRequiredService<IIntakeLogService>();

        authService.AuthStateChanged += (_, _) =>
        {
            if (authService.CurrentUser != null)
            {
                if (reminderEngine is ReminderEngine engine)
                    engine.SetUser(authService.CurrentUser.Id);
                reminderEngine.Start();
            }
            else
            {
                reminderEngine.Stop();
            }
        };

        reminderEngine.ReminderTriggered += (_, args) =>
        {
            Dispatcher.BeginInvoke(() =>
            {
                var alertVm = new AlertViewModel(intakeLogService, args.Medication, args.ScheduledTime, () => { });
                var alertWindow = new AlertWindow(alertVm);
                alertWindow.Closed += (_, _) => alertWindow.DataContext = null;
                alertWindow.Show();
            });
        };

        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var connectionString = "server=localhost;database=meditrack-db;user=root;password=1234;";
        services.AddDbContext<MediTrackDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddSingleton<Services.INavigationService, Services.NavigationService>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IMedicationService, MedicationService>();
        services.AddSingleton<IIntakeLogService, IntakeLogService>();
        services.AddSingleton<IReminderEngine, ReminderEngine>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<SplashViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<MyMedicinesViewModel>();
        services.AddSingleton<ScheduleViewModel>();
        services.AddSingleton<RemindersViewModel>();
        services.AddSingleton<HistoryViewModel>();
        services.AddSingleton<SettingsViewModel>();
    }
}
