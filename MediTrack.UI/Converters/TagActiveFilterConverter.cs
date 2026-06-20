using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MediTrack.UI.Converters;

public class EqualityToBrushConverter : IMultiValueConverter
{
    public System.Windows.Media.Brush TrueBrush { get; set; } = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x10, 0x4C, 0x7F));
    public System.Windows.Media.Brush FalseBrush { get; set; } = System.Windows.Media.Brushes.Transparent;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0]?.ToString() == values[1]?.ToString())
            return TrueBrush;
        return FalseBrush;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class EqualityToForegroundConverter : IMultiValueConverter
{
    public System.Windows.Media.Brush TrueBrush { get; set; } = System.Windows.Media.Brushes.White;
    public System.Windows.Media.Brush FalseBrush { get; set; } = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x88, 0x88, 0x88));

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0]?.ToString() == values[1]?.ToString())
            return TrueBrush;
        return FalseBrush;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
