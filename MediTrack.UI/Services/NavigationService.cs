namespace MediTrack.UI.Services;

public class NavigationService : INavigationService
{
    private object? _currentViewModel;

    public object? CurrentViewModel => _currentViewModel;

    public event EventHandler? Navigated;

    public void NavigateTo(object viewModel)
    {
        _currentViewModel = viewModel;
        Navigated?.Invoke(this, EventArgs.Empty);
    }
}
