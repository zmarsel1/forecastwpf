using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml;
using System.Data;

namespace Core
{
    //Определяет права доступа на документ, из xml файла, источником документа служит выбранная строка Grid'a
    [ValueConversion(typeof(DataRowView), typeof(bool))]
    class PermissionConverter : IValueConverter
    {
        #region Singleton
        PermissionConverter() { }
        static PermissionConverter instance = null;
        static object locker = new object();
        static public PermissionConverter Instance { 
            get
            {
                lock (locker)
                {
                    if (instance == null) instance = new PermissionConverter();
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
                return (node != null);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
