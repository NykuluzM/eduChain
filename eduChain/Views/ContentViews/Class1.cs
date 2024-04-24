using System;

public class FlyoutBehaviorToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is FlyoutBehavior flyoutBehavior)
        {
            return flyoutBehavior == FlyoutBehavior.Locked ? 0.0 : 1.0;
        }
        return 1.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
