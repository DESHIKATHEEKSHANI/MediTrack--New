using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediTrack.Core.Services;
using MediTrack.UI.Services;
using System.Threading.Tasks;

namespace MediTrack.UI.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly DashboardViewModel _dashboardViewModel;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoginMode = true;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    public LoginViewModel(IAuthService authService, INavigationService navigationService, DashboardViewModel dashboardViewModel)
    {
        _authService = authService;
        _navigationService = navigationService;
        _dashboardViewModel = dashboardViewModel;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter both username and password.";
            return;
        }

        var user = await _authService.LoginAsync(Username, Password);
        if (user != null)
        {
            await _dashboardViewModel.LoadDataAsync();
            _navigationService.NavigateTo(_dashboardViewModel);
        }
        else
        {
            ErrorMessage = "Invalid credentials.";
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(FullName))
        {
            ErrorMessage = "All fields are required.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters.";
            return;
        }

        var user = await _authService.RegisterAsync(Username, Email, Password, FullName);
        if (user != null)
        {
            await _dashboardViewModel.LoadDataAsync();
            _navigationService.NavigateTo(_dashboardViewModel);
        }
        else
        {
            ErrorMessage = "Username or email already exists.";
        }
    }

    [RelayCommand]
    private void ToggleMode()
    {
        IsLoginMode = !IsLoginMode;
        ErrorMessage = string.Empty;
    }
}
