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
    public partial class EditWindow2 : Window
    {
        CrossDocument docData = null;
        string propertyField = string.Empty;

        EditWindow2()
        {
            InitializeComponent();
        }
        public EditWindow2(CrossDocument document)
        {
            DataContext = this;
        
            docData = document;

            docData.BeginEdit();

            this.Resources.Add("Title", string.Format("{0} ({1})", docData.DocTitle, docData.IsReadOnly ? "Просмотр" : "Редактирование"));
            this.Resources.Add("Info", docData.DocInfo);
            
            Dictionary<int, string> map = new Dictionary<int,string>();
            foreach(DataRow row in docData.Rows.Rows)
            {
                map[(int)row["RowKey"]] = (string)row["RowName"];
            }
            RowToString converter = new RowToString(map);
            this.Resources.Add("RowConverter", converter);

            InitializeComponent();

            btnSave.IsEnabled = !docData.IsReadOnly;
            gridData.IsReadOnly = docData.IsReadOnly;
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
            if (Resources.Contains("Data"))
                Resources["Data"] = docData;
            else
                this.Resources.Add("Data", docData);

            
            foreach (DataColumn row in docData.Body.Columns)
            {
                DataGridColumn column = null;
                Binding binding = new Binding(row.ColumnName);
                if (row.ColumnName == "RowKey")
                {
                    DataGridTextColumn rowkey = new DataGridTextColumn();
                    rowkey.IsReadOnly = true;
                    binding.Converter = this.Resources["RowConverter"] as IValueConverter;
                    rowkey.Binding = binding;
                    column = rowkey;
                }
                else
                {
                    DataGridCheckBoxColumn entitykey = new DataGridCheckBoxColumn();
                    entitykey.Binding = binding;
                    column = entitykey;
                }

                column.Header = FormatHeaderCaption(row.Caption, 60);
                column.SortMemberPath = row.ColumnName;
                gridData.Columns.Add(column);
            }
        }
        private string FormatHeaderCaption(string caption, double width)
        {
            List<string> list = new List<string>();
            list.Add(caption);
            while (true) //если текст в заголовке грида длинный, делим его попалам
            {
                FormattedText text = new FormattedText(string.Join("\n", list.ToArray()),
                    System.Globalization.CultureInfo.CurrentCulture, 
                    System.Windows.FlowDirection.LeftToRight,
                    new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
                    this.FontSize, Brushes.Black);
                if (text.Width < width)
                    break;
                double coef = 1 - (text.Width - width) / text.Width;
                string temp = list.Last();
                int pos = (int)(temp.Length * coef); //хранит растояние до элемента, где будет перенос строки
                int bspos = temp.IndexOf(" ", 0, pos);
                if ((bspos != -1) && (pos - bspos <= 2)) //если пробел присудствует в исходной строке и он находится на требуемом растоянии
                {
                    temp = temp.Remove(bspos, 1);
                    list[list.Count - 1] = temp.Substring(0, bspos);
                    list.Add(temp.Substring(bspos).Trim());
                }
                else
                {
                    list[list.Count - 1] = temp.Substring(0, pos-1);
                    list.Add(temp.Substring(pos-1).Trim());
                }
            }
            return string.Join("\n", list.ToArray());
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
