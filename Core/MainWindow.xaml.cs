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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;

namespace Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<string, DataTable> mapSelectedDocuments = new Dictionary<string, DataTable>(); // словарь отобранных документов по вкладкам
        private DataTable tableActionLog = new DataTable();

        private System.Windows.Forms.NotifyIcon TrayNotify; //Трей
        private System.Windows.Forms.ContextMenuStrip TrayMenu;

        public MainWindow()
        {
            InitializeComponent();
            //инициализация трея и контекстного меню
            #region Tray
            TrayNotify = new System.Windows.Forms.NotifyIcon();
            TrayNotify.Icon = Properties.Resources.TrayIcon;
            TrayNotify.Visible = true;

            System.Windows.Forms.ToolStripMenuItem mnuConfig = new System.Windows.Forms.ToolStripMenuItem();
            mnuConfig.Text = "Настройки...";
            mnuConfig.Click += new EventHandler(mnuConfig_Click);
            
            
            TrayMenu = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            mnuExit.Text = "Выход";
            mnuExit.Click += new EventHandler(mnuExit_Click);
                        
            TrayMenu.Items.Add(mnuConfig);
            TrayMenu.Items.Add(mnuExit);

            TrayNotify.ContextMenuStrip = TrayMenu;
            #endregion
        }
        #region Tray Menu Handlers
        void mnuConfig_Click(object sender, EventArgs e)
        {
            ConfigWindow wnd = new ConfigWindow();
            wnd.Show();
        } 
        void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        } 
        #endregion
        
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CoreConfig.Instance.Initialize())
            {
                foreach (string key in CoreConfig.Instance.Tabs.Keys)
                {
                    mapSelectedDocuments[key] = new DataTable();
                    TabItem tab = key == "forecast" ? CreateTabForecast(key) : CreateTab(key);
                    tabContainer.Items.Add(tab);
                }
                CreateActionTab();
                PluginManager.Instance.LoadPlugins(CoreConfig.Instance.ConnectionString);

                this.Title = string.Format("ППЭ - {0}", CoreConfig.Instance.ReportUser);
            }
            else
            {
                MessageBox.Show("Ошибка инициализации пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                tabContainer.Items.Clear();//убираем все вкладки, остаётся только трей для настроек
            }
        }

        private void CreateActionTab()
        {
            // типы документов
            StackPanel panelType = (StackPanel)this.FindName("panelDocTypes");
            foreach (string type in CoreConfig.Instance.Docs.Keys)
            {
                CheckBox chk = new CheckBox();
                chk.Tag = type;
                chk.IsChecked = true;
                chk.Content = CoreConfig.Instance.Docs[type];
                panelType.Children.Add(chk);
            }
            // типы объектов
            StackPanel panelObject = (StackPanel)this.FindName("panelUnits");
            foreach (string obj in CoreConfig.Instance.Units.Keys)
            {
                CheckBox chk = new CheckBox();
                chk.Tag = obj;
                chk.IsChecked = true;
                chk.Content = CoreConfig.Instance.Units[obj];
                panelObject.Children.Add(chk);
            }

            gridActionLog.ItemsSource = tableActionLog.DefaultView;
        }

        private TabItem CreateTab(string key)
        {
            TabItem tab = new TabItem();
            tab.Name = "tab_" + key;
            tab.Tag = key;
            
            StackPanel tabheader = new StackPanel();
            
            Image icon = new Image();
            icon.Source = CoreConfig.Instance.Icons[key];
            icon.Width = 24;
            icon.Height = 24;
            icon.Margin = new Thickness(0, 0, 2, 0);
            TextBlock caption = new TextBlock();
            caption.Text = CoreConfig.Instance.Tabs[key];
            caption.FontSize = 24;

            tabheader.Orientation = Orientation.Horizontal;
            tabheader.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            tabheader.Children.Add(icon);
            tabheader.Children.Add(caption);
            tab.Header = tabheader;

            #region Сетка отобранных документов

            DataGrid griddata = new DataGrid();
            griddata.Name = "gridData_" + key;
            griddata.Tag = key;
            griddata.IsReadOnly = true;
            griddata.CanUserAddRows = false;
            griddata.CanUserDeleteRows = false;
            griddata.Margin = new Thickness(205, 0, 0, 0);
            griddata.SelectionMode = DataGridSelectionMode.Single;
            griddata.ItemsSource = mapSelectedDocuments[key].DefaultView;
            griddata.GridLinesVisibility = DataGridGridLinesVisibility.None;
            griddata.SelectedValuePath = "Suffix";
            griddata.MouseDoubleClick += new MouseButtonEventHandler(gridData_MouseDoubleClick);
            griddata.SelectionChanged += new SelectionChangedEventHandler(gridData_SelectionChanged);
            //окрашивание строк таблицы в зависимости от чётности даты в году
            Binding binding = new Binding("DocumentDate");
            binding.Converter = new DateToColorConverter();
            griddata.RowStyle = new Style();
            griddata.RowStyle.Setters.Add(new Setter(DataGridRow.BackgroundProperty, binding));

            this.RegisterName(griddata.Name, griddata);

            DataGridTextColumn docid = new DataGridTextColumn();
            docid.Header = "№ Документа";
            docid.Binding = new Binding("DocumentId");
            griddata.Columns.Add(docid);

            DataGridTextColumn date = new DataGridTextColumn();
            date.Header = "Дата Документа";
            date.Binding = new Binding(@"DocumentDate");
            date.Binding.StringFormat = "{0:dd.MM.yyyy}";
            griddata.Columns.Add(date);

            DataGridTextColumn unit = new DataGridTextColumn();
            unit.Header = "Объект";
            unit.Binding = new Binding("UnitName");
            griddata.Columns.Add(unit);

            DataGridTextColumn doctype = new DataGridTextColumn();
            doctype.Header = "Тип Документа";
            doctype.Binding = new Binding("DocTypeName");
            griddata.Columns.Add(doctype);

            DataGridTextColumn doctotal = new DataGridTextColumn();
            doctotal.Header = "Сумма";
            doctotal.Binding = new Binding("Total");
            doctotal.Binding.StringFormat = "{0:0.00}";
            griddata.Columns.Add(doctotal);

            DataGridTextColumn unitsname = new DataGridTextColumn();
            unitsname.Header = "Объект";
            unitsname.Binding = new Binding("UnitSName");
            unitsname.Visibility = System.Windows.Visibility.Hidden;
            griddata.Columns.Add(unitsname);

            DataGridTextColumn suffix = new DataGridTextColumn();
            suffix.Header = "Тип Документа";
            suffix.Binding = new Binding("Suffix");
            suffix.Visibility = System.Windows.Visibility.Hidden;
            griddata.Columns.Add(suffix);
            #endregion

            Binding btnbinding = new Binding();
            btnbinding.ElementName = griddata.Name;
            btnbinding.Mode = BindingMode.OneWay;
            btnbinding.Path = new PropertyPath("SelectedItem");
            btnbinding.Converter = PermissionConverter.Instance;

            Binding contextbinding = new Binding();
            //contextbinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGrid), 5);
            contextbinding.Mode = BindingMode.OneWay;
            contextbinding.Path= new PropertyPath("SelectedItem");
            contextbinding.Converter = PermissionVisibilityConverter.Instance;
            
            #region Контекстное меню
            ContextMenu mainMenu = new ContextMenu();
            mainMenu.FontSize = 12;
            mainMenu.DataContext = griddata;
           
            MenuItem itemEdit = new MenuItem();
            itemEdit.Header = "Редактировать";
            itemEdit.Click += new RoutedEventHandler(btnEdit_Click);
            itemEdit.SetBinding(MenuItem.VisibilityProperty, contextbinding);
            itemEdit.Tag = key;
            mainMenu.Items.Add(itemEdit);

            MenuItem itemCreate = new MenuItem();
            itemCreate.Header = "Создать";
            itemCreate.Click += new RoutedEventHandler(btnNew_Click);
            itemCreate.Tag = key;

            XmlNodeList assosiateddocs = CoreConfig.Instance.AssosiatedDocs.SelectNodes("root/node/node[not(@name=preceding-sibling::node/@name)]");
            foreach (XmlNode node in assosiateddocs)
            {
                bool result = false;
                foreach (MenuItem item in itemCreate.Items)
                {
                    if (((string)item.Tag).Split(',')[1] == node.Attributes["name"].Value)
                    {
                        result = true;
                        break;
                    }
                }
                if (result) continue; // есть дубликат
                CreateMenuVisibilityConverter converter = new CreateMenuVisibilityConverter();
                //converter.AssosiatedDoc = CoreConfig.Instance.AssosiatedDocs;
                //converter.Permissions = CoreConfig.Instance.Permissions;
                converter.DocType = node.Attributes["name"].Value;
                
                Binding doctypebinding = new Binding();
                doctypebinding.Mode = BindingMode.OneWay;
                doctypebinding.Path = new PropertyPath("SelectedItem");
                doctypebinding.Converter = converter;
                        
                MenuItem itemChild = new MenuItem();
                itemChild.Header = node.Attributes["caption"].Value;
                itemChild.Click += new RoutedEventHandler(btnNew_Click);
                itemChild.Tag = key + "," + node.Attributes["name"].Value;
                itemChild.SetBinding(MenuItem.VisibilityProperty, doctypebinding); 
                itemCreate.Items.Add(itemChild);
            }
            if (!itemCreate.Items.IsEmpty) mainMenu.Items.Add(itemCreate);

            MenuItem itemDelete = new MenuItem();
            itemDelete.Header = "Удалить";
            itemDelete.Click += new RoutedEventHandler(btnDelete_Click);
            itemDelete.SetBinding(MenuItem.VisibilityProperty, contextbinding);
            itemDelete.Tag = key;
            mainMenu.Items.Add(itemDelete);

            Separator itemSeparator = new Separator();
            mainMenu.Items.Add(itemSeparator);

            griddata.ContextMenu = mainMenu;

            foreach (string type in CoreConfig.Instance.Docs.Keys)
            {
                string xpath = string.Format(@"node[@type='{0}']/node[@type='{1}']", key, type);
                var expr_node = CoreConfig.Instance.Permissions.DocumentElement.SelectSingleNode(xpath);
                if (expr_node == null) continue; //проверка пренадлежит ли тип документа текущей вкладке

                XmlNodeList listreports = CoreConfig.Instance.Reports.SelectNodes(string.Format("root/node[@name='{0}']/node", type));
                foreach (XmlNode node in listreports)
                {
                    MenuItem report = new MenuItem();
                    report.Header = node.Attributes["name"];
                    report.Tag = key + "," + node.Attributes["path"].Value;
                    report.Click += new RoutedEventHandler(Report_Click);

                    Binding reportbinding = new Binding();
                    reportbinding.Mode = BindingMode.OneWay;
                    reportbinding.Path = new PropertyPath("SelectedItem");
                    var converter = new ReportConverter();
                    converter.ReportType = type;
                    reportbinding.Converter = converter;


                    report.SetBinding(MenuItem.VisibilityProperty, reportbinding);
                    mainMenu.Items.Add(report);
                }
            }
            #endregion

            StackPanel stackpanel = new StackPanel();
            stackpanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stackpanel.Margin = new Thickness(1,0,0,0);
            stackpanel.Width = 203;
            stackpanel.Name = "stackPanel" + key;
            stackpanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            var margin = new Thickness(2, 2, 2, 2);

            #region Кнопка "Создать"
            
            Button btnnew = new Button();
            btnnew.Name = "btnNew_" + key;
            btnnew.Content = "Создать";
            btnnew.Margin = margin;
            btnnew.Click += new RoutedEventHandler(btnNew_Click);
            btnnew.Tag = key;
            this.RegisterName(btnnew.Name, btnnew);
            
            #endregion

            #region Кнопка "Редактировать"
            Button btnedit = null;
            btnedit = new Button();
            btnedit.Name = "btnEdit_" + key;
            btnedit.Content = "Редактировать";
            btnedit.Margin = margin;
            btnedit.Click += new RoutedEventHandler(btnEdit_Click);
            btnedit.Tag = key;
                
            btnedit.SetBinding(Button.IsEnabledProperty, btnbinding);
            this.RegisterName(btnedit.Name, btnedit);   
            #endregion  

            #region Кнопка "Удалить"

            Button btndelete = new Button();
            btndelete.Name = "btnDelete_" + key;
            btndelete.Content = "Удалить";
            btndelete.Margin = margin;
            btndelete.Click += new RoutedEventHandler(btnDelete_Click);
            btndelete.Tag = key;
            btndelete.SetBinding(Button.IsEnabledProperty, btnbinding);
            this.RegisterName(btndelete.Name, btndelete);

            #endregion
            
            #region Кнопка "Печать"
            StackPanel panelReports = new StackPanel();
            panelReports.Margin = new Thickness(10, 4, 0, 0);

            //Binding
            ComboBox cmbreports = new ComboBox();
            cmbreports.Margin = margin;
            cmbreports.Name = "cmbReports_" + key;
            cmbreports.DisplayMemberPath = "@name";
            cmbreports.SelectedValuePath = "@path";
            cmbreports.IsSynchronizedWithCurrentItem = true;
            
            Binding cmbbinding = new Binding();
            cmbbinding.ElementName = cmbreports.Name;
            cmbbinding.Path = new PropertyPath("Items.IsEmpty");
            cmbbinding.Converter = new ContainerNotEmptyConverter();
            cmbreports.SetBinding(ComboBox.IsEnabledProperty, cmbbinding);

            Binding cmbdatabinding = new Binding();
            cmbdatabinding.ElementName = griddata.Name;
            cmbdatabinding.Path = new PropertyPath("SelectedValue");
            cmbdatabinding.Converter = ReportListConverter.Instance;
            cmbreports.SetBinding(ComboBox.ItemsSourceProperty, cmbdatabinding);

            this.RegisterName(cmbreports.Name, cmbreports);

            Button btnprint = new Button();
            btnprint.Name = "btnPrint_" + key;
            btnprint.Content = "Печать";
            btnprint.Margin = margin;
            btnprint.Click += new RoutedEventHandler(btnPrint_Click);
            btnprint.Tag = key;

            Binding printenable = new Binding();
            printenable.ElementName = cmbreports.Name;
            printenable.Path = new PropertyPath("IsEnabled");
            btnprint.SetBinding(Button.IsEnabledProperty, printenable);

            panelReports.Children.Add(cmbreports);
            panelReports.Children.Add(btnprint);
            
            Expander reports = new Expander();
            reports.Header = "Отчёты";
            reports.Content = panelReports;
            reports.IsExpanded = true;

            #endregion

            #region Кнопка "Поиск"
            Button btnsearch = new Button();
            btnsearch.Name = "btnSearch_" + key;
            btnsearch.Content = "Поиск";
            btnsearch.Margin = margin;
            btnsearch.Click += new RoutedEventHandler(btnSearch_Click);
            btnsearch.Tag = key;
            this.RegisterName(btnsearch.Name, btnsearch);
            #endregion

            #region Календарь
            Calendar cldr = new Calendar();
            cldr.Name = "cldr_" + key;
            cldr.SelectionMode = CalendarSelectionMode.SingleRange;
            cldr.SelectedDates.AddRange(DateTime.Today.AddDays(-3), DateTime.Today.AddDays(5));
            this.RegisterName(cldr.Name, cldr);

            Expander expanderCldr = new Expander();
            expanderCldr.Header = "Календарь";
            expanderCldr.Content = cldr;
            expanderCldr.IsExpanded = true;
            #endregion

            #region Тип документов
            StackPanel panelType = new StackPanel();
            panelType.Name = "panelType_" + key;
            panelType.Margin = new Thickness(10, 4, 0,0);
            this.RegisterName(panelType.Name, panelType);

            foreach (string type in CoreConfig.Instance.Docs.Keys)
            {
                string xpath = string.Format(@"node[@type='{0}']/node[@type='{1}']", key, type);
                var node = CoreConfig.Instance.Permissions.DocumentElement.SelectSingleNode(xpath);
                if (node == null) continue;
                CheckBox chk = new CheckBox();
                chk.Tag = type;
                chk.IsChecked = true;
                chk.Content = CoreConfig.Instance.Docs[type];
                panelType.Children.Add(chk);
            }
            Expander expanderType = new Expander();
            expanderType.Header = "Типы Документов";
            expanderType.Content = panelType;
            expanderType.IsExpanded = (panelType.Children.Count > 1);
            #endregion

            #region Объекты
            StackPanel panelObject = new StackPanel();
            panelObject.Name = "panelObject_" + key;
            panelObject.Margin = new Thickness(10, 4, 0, 0);
            this.RegisterName(panelObject.Name, panelObject);

            foreach (string obj in CoreConfig.Instance.Units.Keys)
            {
                string xpath = string.Format(@"node[@type='{0}']/node/node[@type='{1}']", key, obj);
                var node = CoreConfig.Instance.Permissions.DocumentElement.SelectSingleNode(xpath);
                if (node == null) continue;
                CheckBox chk = new CheckBox();
                chk.Tag = obj;
                chk.IsChecked = true;
                chk.Content = CoreConfig.Instance.Units[obj];
                panelObject.Children.Add(chk);
            }
            Expander expanderObject = new Expander();
            expanderObject.Header = "Объекты";
            expanderObject.Content = panelObject;
            expanderObject.IsExpanded = (panelObject.Children.Count > 1);
            #endregion

            stackpanel.Children.Add(btnnew);
            stackpanel.Children.Add(btnedit);
            stackpanel.Children.Add(btndelete);
            stackpanel.Children.Add(reports);
            stackpanel.Children.Add(expanderCldr);
            stackpanel.Children.Add(expanderType);
            stackpanel.Children.Add(expanderObject);
            stackpanel.Children.Add(btnsearch);

            Grid grid = new Grid();
            grid.Children.Add(stackpanel);
            grid.Children.Add(griddata);

            tab.Content = grid;
            return tab;
        }

        private TabItem CreateTabForecast(string key)
        {
            TabItem tab = new TabItem();
            tab.Name = "tab_" + key;
            tab.Tag = key;

            StackPanel tabheader = new StackPanel();

            Image icon = new Image();
            icon.Source = CoreConfig.Instance.Icons[key];
            icon.Width = 24;
            icon.Height = 24;
            icon.Margin = new Thickness(0, 0, 2, 0);
            TextBlock caption = new TextBlock();
            caption.Text = CoreConfig.Instance.Tabs[key];
            caption.FontSize = 24;

            tabheader.Orientation = Orientation.Horizontal;
            tabheader.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            tabheader.Children.Add(icon);
            tabheader.Children.Add(caption);
            tab.Header = tabheader;

            #region Сетка отобранных документов

            DataGrid griddata = new DataGrid();
            griddata.Name = "gridData_" + key;
            griddata.Tag = key;
            griddata.IsReadOnly = true;
            griddata.CanUserAddRows = false;
            griddata.CanUserDeleteRows = false;
            griddata.Margin = new Thickness(205, 0, 0, 0);
            griddata.SelectionMode = DataGridSelectionMode.Single;
            griddata.ItemsSource = mapSelectedDocuments[key].DefaultView;
            griddata.GridLinesVisibility = DataGridGridLinesVisibility.None;
            griddata.SelectedValuePath = "Suffix";
            griddata.MouseDoubleClick += new MouseButtonEventHandler(gridData_MouseDoubleClick);
            griddata.SelectionChanged += new SelectionChangedEventHandler(gridData_SelectionChanged);
            //окрашивание строк таблицы в зависимости от чётности даты в году
            Binding binding = new Binding("DocumentDate");
            binding.Converter = new DateToColorConverter();
            griddata.RowStyle = new Style();
            griddata.RowStyle.Setters.Add(new Setter(DataGridRow.BackgroundProperty, binding));

            this.RegisterName(griddata.Name, griddata);

            DataGridTextColumn docid = new DataGridTextColumn();
            docid.Header = "№ Документа";
            docid.Binding = new Binding("DocumentId");
            griddata.Columns.Add(docid);

            DataGridTextColumn date = new DataGridTextColumn();
            date.Header = "Дата Документа";
            date.Binding = new Binding(@"DocumentDate");
            date.Binding.StringFormat = "{0:dd.MM.yyyy}";
            griddata.Columns.Add(date);

            DataGridTextColumn unit = new DataGridTextColumn();
            unit.Header = "Объект";
            unit.Binding = new Binding("UnitName");
            griddata.Columns.Add(unit);

            DataGridTextColumn doctype = new DataGridTextColumn();
            doctype.Header = "Тип Документа";
            doctype.Binding = new Binding("DocTypeName");
            griddata.Columns.Add(doctype);

            DataGridTextColumn doctotal = new DataGridTextColumn();
            doctotal.Header = "Сумма";
            doctotal.Binding = new Binding("Total");
            doctotal.Binding.StringFormat = "{0:0.00}";
            griddata.Columns.Add(doctotal);

            DataGridTextColumn unitsname = new DataGridTextColumn();
            unitsname.Header = "Объект";
            unitsname.Binding = new Binding("UnitSName");
            unitsname.Visibility = System.Windows.Visibility.Hidden;
            griddata.Columns.Add(unitsname);

            DataGridTextColumn suffix = new DataGridTextColumn();
            suffix.Header = "Тип Документа";
            suffix.Binding = new Binding("Suffix");
            suffix.Visibility = System.Windows.Visibility.Hidden;
            griddata.Columns.Add(suffix);
            #endregion

            //Binding btnbinding = new Binding();
            //btnbinding.ElementName = griddata.Name;
            //btnbinding.Mode = BindingMode.OneWay;
            //btnbinding.Path = new PropertyPath("SelectedItem");
            //btnbinding.Converter = PermissionConverter.Instance;

            //Binding contextbinding = new Binding();
            //contextbinding.Mode = BindingMode.OneWay;
            //contextbinding.Path = new PropertyPath("SelectedItem");
            //contextbinding.Converter = PermissionVisibilityConverter.Instance;

            #region Контекстное меню
            ContextMenu mainMenu = new ContextMenu();
            mainMenu.FontSize = 12;
            mainMenu.DataContext = griddata; 

            MenuItem itemDelete = new MenuItem();
            itemDelete.Header = "Удалить";
            itemDelete.Click += new RoutedEventHandler(btnDelete_Click);
            //itemDelete.SetBinding(MenuItem.VisibilityProperty, contextbinding);
            itemDelete.Tag = key;
            mainMenu.Items.Add(itemDelete);

            MenuItem itemCreate = new MenuItem();
            itemCreate.Header = "Создать";
            itemCreate.Click += new RoutedEventHandler(btnNew_Click);
            itemCreate.Tag = key;

            XmlNodeList assosiateddocs = CoreConfig.Instance.AssosiatedDocs.SelectNodes("root/node/node[not(@name=preceding-sibling::node/@name)]");
            foreach (XmlNode node in assosiateddocs)
            {
                bool result = false;
                foreach (MenuItem item in itemCreate.Items)
                {
                    if (((string)item.Tag).Split(',')[1] == node.Attributes["name"].Value)
                    {
                        result = true;
                        break;
                    }
                }
                if (result) continue; // есть дубликат
                CreateMenuVisibilityConverter converter = new CreateMenuVisibilityConverter();
                //converter.AssosiatedDoc = CoreConfig.Instance.AssosiatedDocs;
                //converter.Permissions = CoreConfig.Instance.Permissions;
                converter.DocType = node.Attributes["name"].Value;

                Binding doctypebinding = new Binding();
                doctypebinding.Mode = BindingMode.OneWay;
                doctypebinding.Path = new PropertyPath("SelectedItem");
                doctypebinding.Converter = converter;

                MenuItem itemChild = new MenuItem();
                itemChild.Header = node.Attributes["caption"].Value;
                itemChild.Click += new RoutedEventHandler(btnNew_Click);
                itemChild.Tag = key + "," + node.Attributes["name"].Value;
                //itemChild.SetBinding(MenuItem.VisibilityProperty, doctypebinding);
                itemCreate.Items.Add(itemChild);
            }
            if (!itemCreate.Items.IsEmpty) mainMenu.Items.Add(itemCreate);

            Separator itemSeparator = new Separator();
            mainMenu.Items.Add(itemSeparator);

            griddata.ContextMenu = mainMenu;

            foreach (string type in CoreConfig.Instance.Docs.Keys)
            {
                string xpath = string.Format(@"node[@type='{0}']/node[@type='{1}']", key, type); 
                var expr_node = CoreConfig.Instance.Permissions.DocumentElement.SelectSingleNode(xpath);
                if (expr_node == null) continue; //проверка пренадлежит ли тип документа текущей вкладке
                //находим связанные с типом документа отчеты
                XmlNodeList listreports = CoreConfig.Instance.Reports.SelectNodes(string.Format("root/node[@name='{0}']/node", type));
                foreach (XmlNode node in listreports)
                {
                    MenuItem report = new MenuItem();
                    report.Header = node.Attributes["name"];
                    report.Tag = key + "," + node.Attributes["path"].Value;
                    report.Click += new RoutedEventHandler(Report_Click);

                    Binding reportbinding = new Binding();
                    reportbinding.Mode = BindingMode.OneWay;
                    reportbinding.Path = new PropertyPath("SelectedItem");
                    var converter = new ReportConverter();
                    converter.ReportType = type;
                    reportbinding.Converter = converter;


                    report.SetBinding(MenuItem.VisibilityProperty, reportbinding);
                    mainMenu.Items.Add(report);
                }
            }
            #endregion

            StackPanel stackpanel = new StackPanel();
            stackpanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stackpanel.Margin = new Thickness(1, 0, 0, 0);
            stackpanel.Width = 203;
            stackpanel.Name = "stackPanel" + key;
            stackpanel.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            var margin = new Thickness(2, 2, 2, 2);

            #region Кнопка "Создать"

            Button btnnew = new Button();
            btnnew.Name = "btnNew_" + key;
            btnnew.Content = "Создать";
            btnnew.Margin = margin;
            btnnew.Click += new RoutedEventHandler(btnNew_Click);
            btnnew.Tag = key;
            this.RegisterName(btnnew.Name, btnnew);

            #endregion

            #region Кнопка "Удалить"

            Button btndelete = new Button();
            btndelete.Name = "btnDelete_" + key;
            btndelete.Content = "Удалить";
            btndelete.Margin = margin;
            btndelete.Click += new RoutedEventHandler(btnDelete_Click);
            btndelete.Tag = key;
            //btndelete.SetBinding(Button.IsEnabledProperty, btnbinding);
            this.RegisterName(btndelete.Name, btndelete);

            #endregion

            #region Кнопка "Печать"
            StackPanel panelReports = new StackPanel();
            panelReports.Margin = new Thickness(10, 4, 0, 0);

            //Binding
            ComboBox cmbreports = new ComboBox();
            cmbreports.Margin = margin;
            cmbreports.Name = "cmbReports_" + key;
            cmbreports.DisplayMemberPath = "@name";
            cmbreports.SelectedValuePath = "@path";
            cmbreports.IsSynchronizedWithCurrentItem = true;

            Binding cmbbinding = new Binding();
            cmbbinding.ElementName = cmbreports.Name;
            cmbbinding.Path = new PropertyPath("Items.IsEmpty");
            cmbbinding.Converter = new ContainerNotEmptyConverter();
            cmbreports.SetBinding(ComboBox.IsEnabledProperty, cmbbinding);

            Binding cmbdatabinding = new Binding();
            cmbdatabinding.ElementName = griddata.Name;
            cmbdatabinding.Path = new PropertyPath("SelectedValue");
            cmbdatabinding.Converter = ReportListConverter.Instance;
            cmbreports.SetBinding(ComboBox.ItemsSourceProperty, cmbdatabinding);

            this.RegisterName(cmbreports.Name, cmbreports);

            Button btnprint = new Button();
            btnprint.Name = "btnPrint_" + key;
            btnprint.Content = "Печать";
            btnprint.Margin = margin;
            btnprint.Click += new RoutedEventHandler(btnPrint_Click);
            btnprint.Tag = key;

            Binding printenable = new Binding();
            printenable.ElementName = cmbreports.Name;
            printenable.Path = new PropertyPath("IsEnabled");
            btnprint.SetBinding(Button.IsEnabledProperty, printenable);

            panelReports.Children.Add(cmbreports);
            panelReports.Children.Add(btnprint);

            Expander reports = new Expander();
            reports.Header = "Отчёты";
            reports.Content = panelReports;
            reports.IsExpanded = true;

            #endregion

            #region Кнопка "Поиск"
            Button btnsearch = new Button();
            btnsearch.Name = "btnSearch_" + key;
            btnsearch.Content = "Поиск";
            btnsearch.Margin = margin;
            btnsearch.Click += new RoutedEventHandler(btnSearch_Click);
            btnsearch.Tag = key;
            this.RegisterName(btnsearch.Name, btnsearch);
            #endregion

            #region Календарь
            Calendar cldr = new Calendar();
            cldr.Name = "cldr_" + key;
            cldr.SelectionMode = CalendarSelectionMode.SingleRange;
            cldr.SelectedDates.AddRange(DateTime.Today.AddDays(-3), DateTime.Today.AddDays(5));
            this.RegisterName(cldr.Name, cldr);

            Expander expanderCldr = new Expander();
            expanderCldr.Header = "Календарь";
            expanderCldr.Content = cldr;
            expanderCldr.IsExpanded = true;
            #endregion

            #region Тип документов
            StackPanel panelType = new StackPanel();
            panelType.Name = "panelType_" + key;
            panelType.Margin = new Thickness(10, 4, 0, 0);
            this.RegisterName(panelType.Name, panelType);

            foreach (string type in CoreConfig.Instance.Docs.Keys)
            {
                string xpath = string.Format(@"node[@type='{0}']/node[@type='{1}']", key, type);
                var node = CoreConfig.Instance.Permissions.DocumentElement.SelectSingleNode(xpath);
                if (node == null) continue;
                CheckBox chk = new CheckBox();
                chk.Tag = type;
                chk.IsChecked = true;
                chk.Content = CoreConfig.Instance.Docs[type];
                panelType.Children.Add(chk);
            }
            Expander expanderType = new Expander();
            expanderType.Header = "Типы Документов";
            expanderType.Content = panelType;
            expanderType.IsExpanded = (panelType.Children.Count > 1);
            #endregion

            #region Объекты
            StackPanel panelObject = new StackPanel();
            panelObject.Name = "panelObject_" + key;
            panelObject.Margin = new Thickness(10, 4, 0, 0);
            this.RegisterName(panelObject.Name, panelObject);

            foreach (string obj in CoreConfig.Instance.Units.Keys)
            {
                string xpath = string.Format(@"node[@type='{0}']/node/node[@type='{1}']", key, obj);
                var node = CoreConfig.Instance.Permissions.DocumentElement.SelectSingleNode(xpath);
                if (node == null) continue;
                CheckBox chk = new CheckBox();
                chk.Tag = obj;
                chk.IsChecked = true;
                chk.Content = CoreConfig.Instance.Units[obj];
                panelObject.Children.Add(chk);
            }
            Expander expanderObject = new Expander();
            expanderObject.Header = "Объекты";
            expanderObject.Content = panelObject;
            expanderObject.IsExpanded = (panelObject.Children.Count > 1);
            #endregion

            stackpanel.Children.Add(btnnew);
            stackpanel.Children.Add(btndelete);
            stackpanel.Children.Add(reports);
            stackpanel.Children.Add(expanderCldr);
            stackpanel.Children.Add(expanderType);
            stackpanel.Children.Add(expanderObject);
            stackpanel.Children.Add(btnsearch);

            Grid grid = new Grid();
            grid.Children.Add(stackpanel);
            grid.Children.Add(griddata);

            tab.Content = grid;
            return tab;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is MenuItem)
                {
                    MenuItem item = (MenuItem)sender;
                    string tab = ((string)item.Tag).Split(',')[0]; // тэг хранит инфу в виде вкладка,типдокумента
                    string destenationtype = ((string)item.Tag).Split(',')[1];
                    DataGrid grid = (DataGrid)this.FindName("gridData_" + tab);
                    DataRowView bc = (DataRowView)grid.SelectedItem;
                    int docnum = (int)bc["DocumentId"];
                    DateTime docdate = (DateTime)bc["DocumentDate"];
                    string docunit = ((string)bc["UnitSname"]).Trim();

                    CreateDocument wnd = new CreateDocument(string.Empty, destenationtype, docunit, docdate, docnum);
                    Point pos = item.PointToScreen(new Point(0, 0));//назначаем стартовую позицию окна поближе к кнопке создать
                    wnd.Top = pos.Y;
                    wnd.Left = pos.X;
                    bool result = (bool)wnd.ShowDialog();
                    if (result) // если документ успешно создан
                    {
                        if (MessageBox.Show(string.Format("Документ № {0} успешно создан.\n Редактировать документ?", wnd.DocumentId),
                            "Информация", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            Window editwnd = PluginManager.Instance.Plugins[wnd.DocType].EditDocument(CoreConfig.Instance.ConnectionString, wnd.DocumentId);
                            editwnd.Show();
                        };
                    }
                    e.Handled = true;
                }
                else
                {
                    Button btn = (Button)sender;
                    string tab = btn.Tag.ToString();
                    CreateDocument wnd = new CreateDocument(tab);
                    Point pos = btn.PointToScreen(new Point(0, 0));//назначаем стартовую позицию окна поближе к кнопке создать
                    wnd.Top = pos.Y;
                    wnd.Left = pos.X;
                    bool result = (bool)wnd.ShowDialog();
                    if (result) // если документ успешно создан
                    {
                        if (MessageBox.Show(string.Format("Документ № {0} успешно создан.\n Редактировать документ?", wnd.DocumentId),
                            "Информация", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            Window editwnd = PluginManager.Instance.Plugins[wnd.DocType].EditDocument(CoreConfig.Instance.ConnectionString, wnd.DocumentId);
                            editwnd.Show();
                        };
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Control)sender).Name, exception.ToString());
            }  
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FrameworkElement btn = (FrameworkElement)sender;
                string tab = btn.Tag.ToString();

                DataGrid grid = this.FindName("gridData_" + tab) as DataGrid;
                DataRowView bc = (DataRowView)grid.SelectedItem;
                int number = (int)bc["DocumentID"];
                string obj = ((string)bc["UnitSName"]).Trim();
                string plugin = ((string)bc["Suffix"]).Trim();
                //проверка доступности для редактирования
                string path = string.Format(@"node[@type='{0}']/node[@type='{1}']/node[@type='{2}']", tab, plugin, obj);
                var node = CoreConfig.Instance.Permissions.DocumentElement.SelectSingleNode(path);
                if (node != null)
                {
                    //если доступно для редатирования
                    Window wnd = PluginManager.Instance.Plugins[plugin].EditDocument(CoreConfig.Instance.ConnectionString, number);
                    wnd.Show();
                }
            }
            catch (PluginInterface.DocException exception)
            {
                MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Control)sender).Name, exception.ToString());
            }
            catch (Exception exception)
            {
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Control)sender).Name, exception.ToString());
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tab = ((FrameworkElement)sender).Tag.ToString();
                DataGrid grid = this.FindName("gridData_" + tab) as DataGrid;
                DataRowView bc = (DataRowView)grid.SelectedItem;
                int number = (int)bc["DocumentID"];

                ComboBox cmb = (ComboBox)this.FindName("cmbReports_" + tab);

                ReportWnd report = new ReportWnd(CoreConfig.Instance.ReportServer,
                                                 (string)cmb.SelectedValue,
                                                 CoreConfig.Instance.ReportUser,
                                                 CoreConfig.Instance.ReportPassword);
                report.AddParameter("DocID", number.ToString());
                report.Show();
            }
            catch (Exception exception)
            {
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Control)sender).Name, exception.ToString());
            }
        }

        void Report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem)e.OriginalSource;
                // в тэге хранится инормация в виде вкладка,путькотчёту
                string tab = ((string)item.Tag).Split(',')[0];
                string path = ((string)item.Tag).Split(',')[1];


                DataGrid grid = this.FindName("gridData_" + tab) as DataGrid;
                DataRowView bc = (DataRowView)grid.SelectedItem;
                int number = (int)bc["DocumentID"];

                ReportWnd report = new ReportWnd(CoreConfig.Instance.ReportServer,
                                                 path,
                                                 CoreConfig.Instance.ReportUser,
                                                 CoreConfig.Instance.ReportPassword);
                report.AddParameter("DocID", number.ToString());
                report.Show();
            }
            catch (Exception exception)
            {
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Control)sender).Name, exception.ToString());
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FrameworkElement btn = (FrameworkElement)sender;
                string tab = btn.Tag.ToString();

                DataGrid grid = this.FindName("gridData_" + tab) as DataGrid;
                DataRowView bc = (DataRowView)grid.SelectedItem;
                int number = (int)bc["DocumentID"];
                string plugin = ((string)bc["Suffix"]).Trim();
                if( MessageBox.Show(string.Format("Вы действительно хотите удалить Документ № {0} ?",number),
                    "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    PluginManager.Instance.Plugins[plugin].DeleteDocument(CoreConfig.Instance.ConnectionString, number);
                }
            }
            catch (Exception exception)
            {
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Control)sender).Name, exception.ToString());
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                string tab = btn.Tag as string;
                using (var sql = new SqlConnection(CoreConfig.Instance.ConnectionString))
                {
                    
                    //составляем список выделенных Типов документов
                    StackPanel types = this.FindName("panelType_" + tab) as StackPanel;
                    List<SqlParameter> parameters1 = new List<SqlParameter>();
                    StringBuilder strparams1 = new StringBuilder();
                    int i = 0;
                    foreach (UIElement control in types.Children)
                    {
                        CheckBox temp  = control as CheckBox;
                        if(temp != null && (temp.IsChecked ?? false))
                        {
                            parameters1.Add(new SqlParameter(string.Format("@p{0}", i), temp.Tag.ToString()));
                            strparams1.Append(string.Format("@p{0},", i));
                            i++;
                        }
                    }
                    //составляем сисок выделенных Объектов
                    StackPanel objects = this.FindName("panelObject_" + tab) as StackPanel;
                    List<SqlParameter> parameters2 = new List<SqlParameter>();
                    StringBuilder strparams2 = new StringBuilder();
                    i = 0;
                    foreach (UIElement control in objects.Children)
                    {
                        CheckBox temp = control as CheckBox;
                        if (temp != null && (temp.IsChecked ?? false))
                        {
                            parameters2.Add(new SqlParameter(string.Format("@o{0}", i), temp.Tag.ToString()));
                            strparams2.Append(string.Format("@o{0},", i));
                            i++;
                        }
                    }
                    Calendar cldr = this.FindName("cldr_" + tab) as Calendar;

                    var cmd = new SqlCommand();
                    cmd.CommandText = string.Format("SELECT * FROM dbo.fn_select_documents_by_tab(@tab) where DocumentDate >= @start and DocumentDate <=@end and Suffix in ({0}) and UnitSName in ({1})",
                        parameters1.Count == 0 ? @"''" : strparams1.ToString().TrimEnd(','),
                        parameters2.Count == 0 ? @"''" : strparams2.ToString().TrimEnd(','));
                    cmd.Connection = sql;
                    cmd.Parameters.AddWithValue("@tab", tab);
                    cmd.Parameters.AddWithValue("@start", cldr.SelectedDates.Min());
                    cmd.Parameters.AddWithValue("@end", cldr.SelectedDates.Max());
                    cmd.Parameters.AddRange(parameters1.ToArray());
                    cmd.Parameters.AddRange(parameters2.ToArray());
                    var adapter = new SqlDataAdapter(cmd);
                    
                    mapSelectedDocuments[tab].Clear();
                    adapter.Fill(mapSelectedDocuments[tab]);
                }
            }
            catch (Exception exception)
            {
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Control)sender).Name, exception.ToString());
            }
        }

        private void gridData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //search the object hierarchy for a datagrid row
            DependencyObject source = (DependencyObject)e.OriginalSource;
            var row = UIHelper.TryFindParent<DataGridRow>(source);

            //the user did not click on a row
            if (row == null) return;

            //[insert great code here...]
            try
            {
                DataRowView bc = (DataRowView)row.Item;
                int number = (int)bc["DocumentID"];
                string tab = ((Control)sender).Tag as string;
                string obj = ((string)bc["UnitSName"]).Trim();
                string plugin = ((string)bc["Suffix"]).Trim();

                Window wnd = PluginManager.Instance.Plugins[plugin].EditDocument(CoreConfig.Instance.ConnectionString, number);
                wnd.Show();
            }
            catch (PluginInterface.DocException exception)
            {
                MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Control)sender).Name, exception.ToString());

            }
            catch (Exception exception)
            {
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Control)sender).Name, exception.ToString());
            }
            e.Handled = true;
        }

        private void gridData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //to do
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TrayNotify.Visible = false;
        }

        private void FilterLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var sql = new SqlConnection(CoreConfig.Instance.ConnectionString))
                {
                    Calendar cldr = this.FindName("CldrLog") as Calendar;

                    if ((bool)radioWideSearch.IsChecked)
                    {
                        //составляем список выделенных Типов документов
                        StackPanel types = this.FindName("panelDocTypes") as StackPanel;
                        List<SqlParameter> parameters1 = new List<SqlParameter>();
                        StringBuilder strparams1 = new StringBuilder();
                        int i = 0;
                        foreach (UIElement control in types.Children)
                        {
                            CheckBox temp = control as CheckBox;
                            if (temp != null && (temp.IsChecked ?? false))
                            {
                                parameters1.Add(new SqlParameter(string.Format("@p{0}", i), temp.Tag.ToString()));
                                strparams1.Append(string.Format("@p{0},", i));
                                i++;
                            }
                        }
                        //составляем сисок выделенных Объектов
                        StackPanel objects = this.FindName("panelUnits") as StackPanel;
                        List<SqlParameter> parameters2 = new List<SqlParameter>();
                        StringBuilder strparams2 = new StringBuilder();
                        i = 0;
                        foreach (UIElement control in objects.Children)
                        {
                            CheckBox temp = control as CheckBox;
                            if (temp != null && (temp.IsChecked ?? false))
                            {
                                parameters2.Add(new SqlParameter(string.Format("@o{0}", i), temp.Tag.ToString()));
                                strparams2.Append(string.Format("@o{0},", i));
                                i++;
                            }
                        }

                        var cmd = new SqlCommand();
                        cmd.CommandText = string.Format("SELECT * FROM dbo.fn_get_actionlog() WHERE ActionDate >= @start and ActionDate <=@end and Suffix in ({0}) and UnitSName in ({1})",
                            parameters1.Count == 0 ? @"''" : strparams1.ToString().TrimEnd(','),
                            parameters2.Count == 0 ? @"''" : strparams2.ToString().TrimEnd(','));
                        cmd.Connection = sql;
                        cmd.Parameters.AddWithValue("@start", cldr.SelectedDates.Min());
                        cmd.Parameters.AddWithValue("@end", cldr.SelectedDates.Max().AddDays(1));
                        cmd.Parameters.AddRange(parameters1.ToArray());
                        cmd.Parameters.AddRange(parameters2.ToArray());
                        var adapter = new SqlDataAdapter(cmd);
                        tableActionLog.Clear();
                        adapter.Fill(tableActionLog);
                    }
                    else
                    { 
                        int docid = 0;
                        if(!int.TryParse(txtDocumentId.Text, out docid)) 
                        {
                            MessageBox.Show("Неправильный номер документа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        };
                        var cmd = new SqlCommand();
                        cmd.CommandText ="SELECT * FROM dbo.fn_get_actionlog() WHERE ActionDate >= @start and ActionDate <=@end and DocumentId = @docid";
                        cmd.Connection = sql;
                        cmd.Parameters.AddWithValue("@start", cldr.SelectedDates.Min());
                        cmd.Parameters.AddWithValue("@end", cldr.SelectedDates.Max().AddDays(1));
                        cmd.Parameters.AddWithValue("@docid", docid);
                        var adapter = new SqlDataAdapter(cmd);
                        tableActionLog.Clear();
                        adapter.Fill(tableActionLog);
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error("Ошибка при нажатиий {0}.\n{1}", ((Control)sender).Name, exception.ToString());
            }
        }
    }
}

