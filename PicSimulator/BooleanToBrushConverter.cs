using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PicSimulator
{
    public class BooleanToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return Brushes.Yellow; // Hier können Sie die gewünschte Hintergrundfarbe festlegen
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
