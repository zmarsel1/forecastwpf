using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PluginInterface
{
    /// <summary>
    /// Конвертер. Возвращает по ключу(rowkey) занчение(rowname) для более удобного отображения информации
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    class RowToString : IValueConverter
    {
        Dictionary<int, string> Map = null;
        RowToString() { }
        public RowToString(Dictionary<int, string> map) { Map = map; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return Map[(int)value];
            }
            catch
            {
            }
            return "н/д";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
