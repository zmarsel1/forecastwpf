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
using Microsoft.Windows.Controls;
using System.Data;
using System.ComponentModel;
using System.Reflection;

namespace PluginInterface
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        Document docData = null;

        EditWindow()
        {
            InitializeComponent();
        }
        public EditWindow(Document document)
        {
            DataContext = this;
        
            docData = document;

            docData.BeginEdit(); // throw new PluginInterface.DocException("Не удалось открыть документ для редактирования.");

            this.Resources["Sum"] = "Сумма: " + ((double)docData.Body.Compute("SUM(EntityData)", "")).ToString("0.00");;
            this.Resources.Add("Title", string.Format("{0} ({1})",docData.DocTitle, docData.IsReadOnly ? "Просмотр" : "Редактирование"));
            this.Resources.Add("Info", docData.DocInfo);
            this.Resources.Add("Data", docData);

            Dictionary<int, string> map = new Dictionary<int, string>();
            foreach (DataRow row in docData.Rows.Rows)
            {
                map[(int)row["RowKey"]] = (string)row["RowName"];
            }
            RowToString converter = new RowToString(map);
            this.Resources.Add("RowConverter", converter);

            InitializeComponent();

            gridData.IsReadOnly = docData.IsReadOnly;
            btnSave.IsEnabled = !docData.IsReadOnly;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                docData.Save();
                MessageBox.Show("Изменения сохранены.");
            }
            catch (Exception exception)
            { 
                MessageBox.Show("Ошибка сохранения.\n" + exception.ToString());
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void gridData_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            if ((bool)chkCascade.IsChecked &&
                e.EditAction == DataGridEditAction.Commit)
            {
                try
                {
                    Binding binding = gridData.CurrentColumn.ClipboardContentBinding as Binding; // col.Binding as Binding;
                    string boundPropertyName = binding.Path.Path;

                    object value = ((DataRowView)e.Row.Item)[boundPropertyName];

                    int index = e.Row.GetIndex() + 1;// gridData.SelectedIndex;
                    for (; index < gridData.Items.Count; index++)
                    {
                        object row = gridData.Items[index];

                        PropertyDescriptorCollection properties =
                        TypeDescriptor.GetProperties(row);

                        PropertyDescriptor property = properties[boundPropertyName];
                        object destenation = property.GetValue(row);
                        property.SetValue(row, value);
                    }
                }
                catch (Exception)
                {

                }
            }
            this.Resources["Sum"] = "Сумма: " + ((double)docData.Body.Compute("SUM(EntityData)", "")).ToString("0.00");
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            docData.EndEdit();
        }
        //
        // SINGLE CLICK EDITING
        //
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }
            }
        }

        static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
    }
}
