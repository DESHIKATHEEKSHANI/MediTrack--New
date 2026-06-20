using CommunityToolkit.Mvvm.ComponentModel;
using MediTrack.Core.Services;
using MediTrack.UI.Services;

namespace MediTrack.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private object? _currentViewModel;

    [ObservableProperty]
    private bool _isAuthenticated;

    public MainViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _authService.AuthStateChanged += OnAuthStateChanged;
        _navigationService.Navigated += (_, _) => CurrentViewModel = _navigationService.CurrentViewModel;
    }

    private void OnAuthStateChanged(object? sender, EventArgs e)
    {
        IsAuthenticated = _authService.CurrentUser != null;
    }
}
