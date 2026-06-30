using CommunityToolkit.Mvvm.ComponentModel;

namespace MediTrack.UI.Models;

public partial class ReminderTimeEntry : ObservableObject
{
    [ObservableProperty]
    private int _hour = 8;

    [ObservableProperty]
    private int _minute = 0;

    [ObservableProperty]
    private string _amPm = "AM";

    public string To24HourString()
    {
        int h = Hour;
        if (AmPm == "PM" && h != 12) h += 12;
        if (AmPm == "AM" && h == 12) h = 0;
        return $"{h:D2}:{Minute:D2}";
    }

    public static ReminderTimeEntry From24HourString(string time24)
    {
        if (TimeSpan.TryParse(time24.Trim(), out var ts))
        {
            var entry = new ReminderTimeEntry();
            int h = ts.Hours;
            if (h == 0)
            {
                entry.Hour = 12;
                entry.AmPm = "AM";
            }
            else if (h < 12)
            {
                entry.Hour = h;
                entry.AmPm = "AM";
            }
            else if (h == 12)
            {
                entry.Hour = 12;
                entry.AmPm = "PM";
            }
            else
            {
                entry.Hour = h - 12;
                entry.AmPm = "PM";
            }
            entry.Minute = ts.Minutes;
            return entry;
        }
        return new ReminderTimeEntry { Hour = 8, Minute = 0, AmPm = "AM" };
    }
}
