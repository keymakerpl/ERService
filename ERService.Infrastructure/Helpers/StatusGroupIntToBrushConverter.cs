using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ERService.Infrastructure.Helpers
{
    //TODO: Wybór koloru z opcji
    [ValueConversion(typeof(int), typeof(Brush))]
    public class StatusGroupIntToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target must be a System.Windows.Media.Brush");
            
            switch ((int) value)
            {
                case 0:
                    return new SolidColorBrush(Colors.LightGreen);
                case 1:
                    return new SolidColorBrush(Colors.Orange);
                case 2:
                    return new SolidColorBrush(Colors.CadetBlue);

                default:
                    return new SolidColorBrush(Colors.Transparent);                    
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(Colors.Transparent);
        }
    }
}
