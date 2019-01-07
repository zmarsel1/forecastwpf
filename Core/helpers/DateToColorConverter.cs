using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Core
{
    //Конвертер окрашивает строки таблицы в зависимости от чётности дня в году
    [ValueConversion(typeof(DateTime), typeof(SolidColorBrush))]
    class DateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                DateTime date = (DateTime)value;
                if (date.DayOfYear % 2 == 0)
                {
                    return Brushes.LightBlue;
                }
            }
            catch (Exception)
            {
                
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
