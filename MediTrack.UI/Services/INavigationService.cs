namespace MediTrack.UI.Services;

public interface INavigationService
{
    void NavigateTo(object viewModel);
    event EventHandler? Navigated;
    object? CurrentViewModel { get; }
}
