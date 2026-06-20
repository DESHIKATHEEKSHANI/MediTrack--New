using CommunityToolkit.Mvvm.ComponentModel;

namespace MediTrack.UI.Models;

public partial class NavItem : ObservableObject
{
    [ObservableProperty]
    private string _label = string.Empty;

    [ObservableProperty]
    private string _icon = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}
