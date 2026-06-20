using MediTrack.UI.ViewModels;
using System.Windows;

namespace MediTrack.UI.Views;

public partial class AlertWindow : Window
{
    public AlertWindow()
    {
        InitializeComponent();
    }

    public AlertWindow(AlertViewModel viewModel) : this()
    {
        DataContext = viewModel;
        PositionAlert();
    }

    private void PositionAlert()
    {
        var workingArea = SystemParameters.WorkArea;
        Left = workingArea.Right - Width - 20;
        Top = workingArea.Bottom - Height - 20;
    }
}
