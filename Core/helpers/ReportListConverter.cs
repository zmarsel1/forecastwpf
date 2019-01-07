using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml;
using System.Data;

namespace Core
{
    //[ValueConversion(typeof(DataRowView), typeof(XmlNode))]
    class ReportListConverter : IValueConverter
    {
        #region Singleton
        ReportListConverter() { }
        static ReportListConverter instance = null;
        static object locker = new object();
        static public ReportListConverter Instance
        { 
            get
            {
                lock (locker)
                {
                    if (instance == null) instance = new ReportListConverter();
                    return instance;
                }
            }
        }
        #endregion

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string doctype = (string)value;
                string xpath = string.Format("root/node[@name='{0}']/node", doctype.Trim());

                XmlNodeList list = CoreConfig.Instance.Reports.SelectNodes(xpath);
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
