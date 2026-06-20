using CommunityToolkit.Mvvm.ComponentModel;
using MediTrack.UI.Services;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MediTrack.UI.ViewModels;

public partial class SplashViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly LoginViewModel _loginViewModel;

    [ObservableProperty]
    private string _statusMessage = "Initializing MediTrack...";

    public SplashViewModel(INavigationService navigationService, LoginViewModel loginViewModel)
    {
        _navigationService = navigationService;
        _loginViewModel = loginViewModel;
        StartInitialization();
    }

    private void StartInitialization()
    {
        var timer = new Timer(2000) { AutoReset = false };
        timer.Elapsed += (_, _) =>
        {
            StatusMessage = "Ready";
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                _navigationService.NavigateTo(_loginViewModel);
            });
        };
        timer.Start();
    }
}
