using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Xml;
using System.Data;

namespace Core
{
    class CreateMenuVisibilityConverter : IValueConverter
    {
        //XmlDocument assosiations = new XmlDocument();
        //XmlDocument permissions = new XmlDocument();
        public string DocType { get; set; }

        //public XmlDocument AssosiatedDoc { get { return assosiations; } set { assosiations = value; } }
        //public XmlDocument Permissions { get { return permissions; } set { permissions = value; } }
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                DataRowView row = (DataRowView)value;
                string type = ((string)row["Suffix"]).Trim();
                string unit = ((string)row["UnitSName"]).Trim();
                
                string xpath1 = string.Format("root/node[@name='{0}']/node[@name='{1}']",type, DocType);
                XmlNode node1 = CoreConfig.Instance.AssosiatedDocs.SelectSingleNode(xpath1);

                string xpath2 = string.Format("root/node/node[@type='{0}']/node[@type='{1}' and @permission='EDIT']", type, unit);
                XmlNode node2 = CoreConfig.Instance.Permissions.SelectSingleNode(xpath2);
                
                return (node1 != null && node2 != null) ? Visibility.Visible : Visibility.Collapsed; 
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}
