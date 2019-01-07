using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Data;
using System.Xml;

namespace Core
{
    //Конвертер определяет какие пункты меню,отвечающие за отчёты, соответствуют выделенному типу документа
    [ValueConversion(typeof(DataRowView),typeof(Visibility))]
    class ReportConverter : IValueConverter
    {
        public string ReportType { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                DataRowView row = (DataRowView)value;
                string doctype = ((string)row["Suffix"]).Trim();

                return (doctype == ReportType) ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
