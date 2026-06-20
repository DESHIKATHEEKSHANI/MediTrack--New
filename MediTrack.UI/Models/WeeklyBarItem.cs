namespace MediTrack.UI.Models;

public class WeeklyBarItem
{
    public double Value { get; set; }
    public string Label { get; set; } = string.Empty;
    public double BarHeightSmall => Value * 1.2;
    public double BarHeightLarge => Value * 3.6;
}
