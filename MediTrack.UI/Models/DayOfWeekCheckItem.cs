using CommunityToolkit.Mvvm.ComponentModel;

namespace MediTrack.UI.Models;

public partial class DayOfWeekCheckItem : ObservableObject
{
    public DayOfWeek Day { get; set; }
    public string ShortLabel { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}
