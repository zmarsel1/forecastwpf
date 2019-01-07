using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;

namespace Core
{
    /// <summary>
    /// Interaction logic for CreateDocument.xaml
    /// </summary>
    public partial class CreateDocument : Window
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public int DocumentId { get; private set; }
        public string DocType { get; private set; }

        CreateDocument()
        {
            InitializeComponent();
        }
        public CreateDocument(string tab)
        {
            XmlDataProvider provider = new XmlDataProvider();
            provider.Document = CoreConfig.Instance.Permissions;
            string xpath = tab == string.Empty ? "root/node/node" : string.Format(@"root/node[@type='{0}']/node", tab);
            provider.XPath = xpath;
            this.Resources["Permissions"] = provider;
            
            InitializeComponent();
            if (tab == "forecast") //если вкладка прогнозы, то можно создавать прогнозы, даже если права доступа только на просмотр
            {
                Binding forecast = new Binding();
                forecast.XPath = "node";
                cmbDocObject.SetBinding(ComboBox.ItemsSourceProperty, forecast);
            }
            chkAssociated.IsEnabled = (tab != "forecast"); //прогноз нельзя создавать из связанного докумаента
            
        }
        public CreateDocument(string tab, string type, string unit, DateTime date, int assosiateddoc) : this(tab)
        {
            cmbDocType.SelectedValue = type;
            dtpDocDate.SelectedDate = date;
            txtParentDocument.Text = assosiateddoc.ToString();
            chkAssociated.IsChecked = true;
            cmbDocObject.SelectedValue = unit;
        }

        private void btnAssociated_Click(object sender, RoutedEventArgs e)
        {
            string msg = string.Empty;
            if (cmbDocObject.SelectedValue == null) msg = "Не выбран ОБЪЕКТ.";
            if (cmbDocType.SelectedValue == null) msg = "Не выбран ТИП документа";
            if (msg != string.Empty)
            {
                MessageBox.Show(msg, "Ошибка");
                return;
            }
            string doctype = cmbDocType.SelectedValue.ToString();
            string obj = cmbDocObject.SelectedValue.ToString();
            ChooseParent dlg = new ChooseParent(doctype, obj);
            if (dlg.ShowDialog() == true)
            {
                txtParentDocument.Text = dlg.SelectedDocument.ToString();
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string msg = string.Empty;
            if (dtpDocDate.SelectedDate == null) msg = "Не выбрана ДАТА";
            if (cmbDocObject.SelectedValue == null) msg = "Не выбран ОБЪЕКТ.";
            if (cmbDocType.SelectedValue == null) msg = "Не выбран ТИП документа";
            if ((bool)chkAssociated.IsChecked && txtParentDocument.Text.Length == 0) msg = "Связанный документ не выбран";
            if (msg != string.Empty)
            {
                MessageBox.Show(msg, "Ошибка");
                return;
            }
            string plugin = cmbDocType.SelectedValue.ToString();
            string obj = cmbDocObject.SelectedValue.ToString();
            try
            {
                if ((bool)chkAssociated.IsChecked)
                    DocumentId = PluginManager.Instance.Plugins[plugin].CreateDocument(CoreConfig.Instance.ConnectionString, plugin, obj, (DateTime)dtpDocDate.SelectedDate, int.Parse(txtParentDocument.Text));
                else
                    DocumentId = PluginManager.Instance.Plugins[plugin].CreateDocument(CoreConfig.Instance.ConnectionString, plugin, obj, (DateTime)dtpDocDate.SelectedDate);
                DocType = plugin;
                this.DialogResult = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Ошибка создания документа.\n" + exception.Message);
                logger.Error("Ошибка создания документа.\n{0}", exception);
                this.DialogResult = false;
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (dtpDocDate.SelectedDate == null)
            {
                dtpDocDate.SelectedDate = DateTime.Today;
                cmbDocType.SelectedIndex = 0;
                cmbDocObject.SelectedIndex = 0;
            }
        }
    }
}
