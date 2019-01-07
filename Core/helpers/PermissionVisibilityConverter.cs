using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Windows.Data;
using System.Windows;

namespace Core
{
    [ValueConversion(typeof(DataRowView), typeof(Visibility))]
    class PermissionVisibilityConverter : IValueConverter
    {
        #region Singleton
        PermissionVisibilityConverter() { }
        static PermissionVisibilityConverter instance = null;
        static object locker = new object();
        static public PermissionVisibilityConverter Instance
        { 
            get
            {
                lock (locker)
                {
                    if (instance == null) instance = new PermissionVisibilityConverter();
                    return instance;
                }
            }
        }
        #endregion

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                DataRowView row = (DataRowView)value;
                string obj = ((string)row["UnitSName"]).Trim();
                string plugin = ((string)row["Suffix"]).Trim();
                //проверка доступности для редактирования
                string xpath = string.Format(@"root/node/node[@type='{0}']/node[@type='{1}' and @permission='EDIT']", plugin, obj);

                XmlNode node = CoreConfig.Instance.Permissions.SelectSingleNode(xpath);
                return (node != null) ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
