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
using System.Data.SqlClient;
using System.Data;
using Microsoft.Windows.Controls;

namespace Core
{
    /// <summary>
    /// Interaction logic for ChooseParent.xaml
    /// </summary>
    public partial class ChooseParent : Window
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        string docType;
        string docObj;

        public int SelectedDocument { get; set; }
        DataTable Documents = new DataTable();
        ChooseParent()
        {
            InitializeComponent();
        }
        public ChooseParent(string type, string obj)
        {
            docType = type;
            docObj = obj;
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cldrDate.SelectedDates.AddRange(DateTime.Today.AddDays(-7), DateTime.Today);
            btnSearch_Click(null, null);
            this.Resources["Documents"] = Documents;
            //gridDocuments.ItemsSource = Documents.DefaultView;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var sql = new SqlConnection(CoreConfig.Instance.ConnectionString))
                {
                    var cmd = new SqlCommand();
                    cmd.CommandText = "SELECT * FROM dbo.fn_get_documents() WHERE UnitSName=@obj AND Associated = @type AND DocumentDate >=@start and DocumentDate <=@end";
                    cmd.Connection = sql;
                    cmd.Parameters.AddWithValue("@type", docType);
                    cmd.Parameters.AddWithValue("@obj", docObj);
                    cmd.Parameters.AddWithValue("@start", cldrDate.SelectedDates.First());
                    cmd.Parameters.AddWithValue("@end", cldrDate.SelectedDates.Last());

                    Documents.Clear();
                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(Documents);
                }
            }
            catch
            { 
            
            }
        }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)gridDocuments.SelectedItem;
            if (row == null)
            {
                MessageBox.Show("Не выбран документ.");
                return;
            }
            SelectedDocument = (int)row["DocumentId"];
            this.DialogResult = true;
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void gridDocuments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject source = (DependencyObject)e.OriginalSource;
            var row = UIHelper.TryFindParent<DataGridRow>(source);

            //the user did not click on a row
            if (row == null) return;

            //[insert great code here...]
            try
            {
                DataRowView bc = (DataRowView)gridDocuments.SelectedItem;
                SelectedDocument = (int)bc["DocumentId"];
                e.Handled = true;
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception exception)
            {
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Button)sender).Name, exception.ToString());
            }
            e.Handled = true;
        }
    }
}
