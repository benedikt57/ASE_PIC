using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PicSimulator
{
    public class LEDArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string str)
            {
                if ((value is int intValue))
                {
                    if((intValue & (1 << int.Parse(str))) != 0)
                    {
                        return Brushes.Red;
                    }
                    else
                    {
                        return Brushes.Gray;
                    }
                }
            }
            return Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
