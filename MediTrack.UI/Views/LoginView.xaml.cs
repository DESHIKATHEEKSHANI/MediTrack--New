using MediTrack.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MediTrack.UI.Views;

public partial class LoginView : System.Windows.Controls.UserControl
{
    public LoginView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(LoginViewModel.IsLoginMode))
                    UpdateLabels(vm);
            };
            UpdateLabels(vm);
        }
    }

    private void UpdateLabels(LoginViewModel vm)
    {
        ModeTitle.Text = vm.IsLoginMode ? "Welcome Back" : "Create Account";
        ModeSubtitle.Text = vm.IsLoginMode ? "Sign in to your MediTrack account" : "Join MediTrack to manage your health";
        ToggleButton.Content = vm.IsLoginMode ? "Create New Account" : "Already have an account? Sign In";
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
            vm.Password = PasswordBox.Password;
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
            vm.ConfirmPassword = ConfirmPasswordBox.Password;
    }
}
