using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                if ((value is ObservableCollection<int> intCollection))
                {
                    if (intCollection[int.Parse(str)] == 1)
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
