using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using NLog;
using System.Xml;
using System.Configuration;

namespace Core
{
    /// <summary>
    /// <c>CoreConfig</c> клас хранит глобальные настройки программы
    /// </summary>
    class CoreConfig
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region Singleton
        CoreConfig() { }
 
        static CoreConfig instance = null;
        static object locker = new object();
        /// <summary>
        /// Возвращает глобальный экземпляр <c>CoreConfig</c>
        /// </summary>
        public static CoreConfig Instance
        {
            get 
            {
                lock(locker)
                {
                    if (instance == null) instance = new CoreConfig();
                    return instance;
                }
            }
        }
        #endregion

        Dictionary<string, BitmapImage> mapIcons = new Dictionary<string, BitmapImage>();
        Dictionary<string, string> mapDocs = new Dictionary<string, string>(); // список документов
        Dictionary<string, string> mapTabs = new Dictionary<string, string>(); // список вкладок
        Dictionary<string, string> mapUnits = new Dictionary<string, string>(); // список Объектов

        XmlDocument xmlPermissions = new XmlDocument(); //права доступа пользователя
        XmlDocument xmlReports = new XmlDocument(); //доступные отчёты
        XmlDocument xmlAssosiatedDocs = new XmlDocument(); //связанные документы

        /// <summary>
        /// Поле, хранит строку соединение с БД
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Поле, хранит адресс сервера отчетов
        /// </summary>
        public string ReportServer { get; private set; }
        /// <summary>
        /// Поле, хранит имя пользователя сервера отчетов
        /// </summary>
        public string ReportUser { get; private set; }
        /// <summary>
        /// Поле, хранит пароль сревера отчетов
        /// </summary>
        public string ReportPassword { get; private set; }
        /// <summary>
        /// Поле, хранит адресс сервера прогнозов
        /// </summary>
        public string ForecastServer { get; private set; }

        /// <summary>
        /// Поле, хранит вкладки доступные пользователю, в виде пары ключ-значение
        /// </summary>
        public Dictionary<string, string> Tabs { get { return mapTabs; } }
        /// <summary>
        /// Поле, хранит словарь доступных типов документов
        /// </summary>
        public Dictionary<string, string> Docs { get { return mapDocs; } }
        /// <summary>
        /// Поле, хранит узлы учёта, доступные пользователю
        /// </summary>
        public Dictionary<string, string> Units { get { return mapUnits; } }
        /// <summary>
        /// Поле, хранит словарь иконок
        /// </summary>
        public Dictionary<string, BitmapImage> Icons { get { return mapIcons; } }
        /// <summary>
        /// Поле, хранит права доступа пользователя, в виде XML-документа
        /// </summary>
        public XmlDocument Permissions { get { return xmlPermissions; } }
        /// <summary>
        /// Поле, хранит доступные отчеты, в виде XML-документа
        /// </summary>
        public XmlDocument Reports { get { return xmlReports; } }
        /// <summary>
        /// Поле, хранит типы связанных документов, в виде XML-документа
        /// </summary>
        public XmlDocument AssosiatedDocs { get { return xmlAssosiatedDocs; } }

        /// <summary>
        /// Инициализация настроек приложения
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            try
            {
                //чтение настроек сервера отчетов
                ReportServer = ConfigurationManager.AppSettings["ReportServer"];
                ReportUser = ConfigurationManager.AppSettings["ReportServerUser"];
                ReportPassword = Cryptography.Decrypt("secret_top_secret", ConfigurationManager.AppSettings["ReportServerPassword"]);
                //чтение настроек Сервера Прогнозов
                ForecastServer = ConfigurationManager.AppSettings["ForecastServer"];
                //чтение настроек БД
                SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder();
                connection.DataSource = ConfigurationManager.AppSettings["Server"];
                connection.InitialCatalog = ConfigurationManager.AppSettings["DataBase"];

                string sequrity = ConfigurationManager.AppSettings["IntegratedSecurity"];
                if (sequrity == "NO")
                {
                    connection.UserID = ConfigurationManager.AppSettings["UID"];
                    connection.Password = Cryptography.Decrypt("secret_top_secret", ConfigurationManager.AppSettings["Password"]);
                }
                else
                {
                    connection.IntegratedSecurity = true;
                }
                ConnectionString = connection.ConnectionString;
            }
            catch(Exception exception)
            {
                logger.Fatal("Ошибка инициализации пользователя.\n{0}\n{1}", exception.Message, exception);
                return false;
            }
            return (InitTabs() && InitReports() && InitAssosiatedDocs());
        }
        //считываем таблицу связанных документов
        private bool InitAssosiatedDocs()
        {
            var sql = new SqlConnection(ConnectionString);
            //logger.Info("Строка соединения : {0}", strConnection);
            // доступные схемы текущему пользователю
            var str = @"select * from dbo.fn_get_assosiateddocs()";
            var cmd = new SqlCommand(str, sql);
            var adapter = new SqlDataAdapter(cmd);
            var data = new DataTable();

            try
            {
                adapter.Fill(data);
            }
            catch (Exception e)
            {
                logger.Fatal("Ошибка запроса доступных пользователю отчётов.\n Exception: {0}", e);
                return false;
            }

            XmlNode root = xmlAssosiatedDocs.CreateElement("root");
            xmlAssosiatedDocs.AppendChild(root);
            XmlNode typenode = null, reportnode = null;

            foreach (DataRow row in data.Rows)
            {
                string type = ((string)row["Parent"]).Trim();
                string child = ((string)row["Child"]).Trim();
                string caption = ((string)row["Caption"]).Trim();

                string xpath = string.Format("node[@name='{0}']", type);
                typenode = xmlAssosiatedDocs.DocumentElement.SelectSingleNode(xpath);
                if (typenode == null)
                {
                    typenode = xmlAssosiatedDocs.CreateElement("node");
                    XmlAttribute attr_type = xmlAssosiatedDocs.CreateAttribute("name");
                    attr_type.Value = type;
                    typenode.Attributes.Append(attr_type);
                    root.AppendChild(typenode);
                }
                reportnode = xmlAssosiatedDocs.CreateElement("node");
                XmlAttribute attr_name = xmlAssosiatedDocs.CreateAttribute("name");
                attr_name.Value = child;
                XmlAttribute attr_caption = xmlAssosiatedDocs.CreateAttribute("caption");
                attr_caption.Value = caption;
                reportnode.Attributes.Append(attr_name);
                reportnode.Attributes.Append(attr_caption);
                typenode.AppendChild(reportnode);
            }
            return true;
        }
        //считывает настройки вкладок и прав доступа к документам
        bool InitTabs()
        {
            var sql = new SqlConnection(ConnectionString);
            //logger.Info("Строка соединения : {0}", strConnection);
            // доступные схемы текущему пользователю
            var str = @"select * from dbo.fn_get_mypermissions() order by unitsname";
            var cmd = new SqlCommand(str, sql);
            var adapter = new SqlDataAdapter(cmd);
            var data = new DataTable();

            try
            {
                adapter.Fill(data);
            }
            catch (Exception e)
            {
                logger.Fatal("Ошибка запроса доступных пользователю прав.\n Exception: {0}", e);
                return false;
            }
            if (data.Rows.Count == 0)
            {
                logger.Error("Пользователь не обладает необходимыми правами базы данных.");
                return false;
            }

            XmlNode root = xmlPermissions.CreateElement("root");
            xmlPermissions.AppendChild(root);
            XmlNode tabnode = null, typenode = null, objnode = null;

            foreach (DataRow row in data.Rows)
            {
                string path = Environment.CurrentDirectory;
                string doctype = row["doctype"].ToString().Trim();
                string tab = row["tabname"].ToString().Trim();
                string unit = row["unitsname"].ToString().Trim();

                if (!mapDocs.ContainsKey(doctype)) mapDocs[doctype] = row["doctypename"].ToString().Trim();
                if (!mapUnits.ContainsKey(unit)) mapUnits[unit] = row["unitname"].ToString().Trim();

                string xpath = string.Format("node[@type='{0}']", tab);
                tabnode = xmlPermissions.DocumentElement.SelectSingleNode(xpath);
                if (tabnode == null)
                {
                    tabnode = xmlPermissions.CreateElement("node");
                    XmlAttribute attr1 = xmlPermissions.CreateAttribute("type");
                    attr1.Value = tab;
                    tabnode.Attributes.Append(attr1);
                    XmlAttribute attr2 = xmlPermissions.CreateAttribute("caption");
                    attr2.Value = row["tabcaption"].ToString().Trim();
                    tabnode.Attributes.Append(attr2);
                    root.AppendChild(tabnode);

                    mapTabs[tab] = row["tabcaption"].ToString().Trim();
                    try
                    {
                        mapIcons[tab] = new BitmapImage(new Uri(string.Format(@"{0}\Images\{1}", path, row["tabimage"])));
                    }
                    catch (Exception)
                    {
                        mapIcons[tab] = new BitmapImage(new Uri("/images/default.png", UriKind.Relative));
                    }
                }
           
                typenode = tabnode.SelectSingleNode("descendant::node[@type='" + doctype + "']");
                if (typenode == null)
                {
                    typenode = xmlPermissions.CreateElement("node");
                    XmlAttribute attr1 = xmlPermissions.CreateAttribute("type");
                    attr1.Value = doctype;
                    typenode.Attributes.Append(attr1);
                    XmlAttribute attr2 = xmlPermissions.CreateAttribute("caption");
                    attr2.Value = row["doctypename"].ToString().Trim();
                    typenode.Attributes.Append(attr2);
                    tabnode.AppendChild(typenode);
                }
                objnode = xmlPermissions.CreateElement("node");
                XmlAttribute attr3 = xmlPermissions.CreateAttribute("type");
                attr3.Value = unit;
                objnode.Attributes.Append(attr3);
                XmlAttribute attr4 = xmlPermissions.CreateAttribute("caption");
                attr4.Value = row["unitname"].ToString().Trim();
                objnode.Attributes.Append(attr4);
                XmlAttribute attr5 = xmlPermissions.CreateAttribute("permission");
                attr5.Value = row["permission"].ToString();
                objnode.Attributes.Append(attr5);
                typenode.AppendChild(objnode);
            }
            return true;
        }
        //считываем параметры отчётов
        bool InitReports()
        {

            var sql = new SqlConnection(ConnectionString);
            //logger.Info("Строка соединения : {0}", strConnection);
            // доступные схемы текущему пользователю
            var str = @"select RTRIM(Suffix) as Suffix, RTRIM(ReportName) as ReportName, RTRIM(ReportAddress) as ReportAddress from dbo.fn_get_reports()";
            var cmd = new SqlCommand(str, sql);
            var adapter = new SqlDataAdapter(cmd);
            var data = new DataTable();

            try
            {
                adapter.Fill(data);
            }
            catch (Exception e)
            {
                logger.Fatal("Ошибка запроса доступных пользователю отчётов.\n Exception: {0}", e);
                return false;
            }
            /*if (data.Rows.Count == 0)
            {
                logger.Error("Пользователь не обладает необходимыми правами базы данных.");
                return false;
            }*/

            XmlNode root = xmlReports.CreateElement("root");
            xmlReports.AppendChild(root);
            XmlNode typenode = null, reportnode = null;

            foreach (DataRow row in data.Rows)
            {
                string type = ((string)row["Suffix"]).Trim();
                string name = ((string)row["ReportName"]).Trim();
                string path = ((string)row["ReportAddress"]).Trim();

                string xpath = string.Format("node[@name='{0}']", type);
                typenode = xmlReports.DocumentElement.SelectSingleNode(xpath);
                if (typenode == null)
                {
                    typenode = xmlReports.CreateElement("node");
                    XmlAttribute attr_type = xmlReports.CreateAttribute("name");
                    attr_type.Value = type;
                    typenode.Attributes.Append(attr_type);
                    root.AppendChild(typenode);
                }
                reportnode = xmlReports.CreateElement("node");
                XmlAttribute attr_name = xmlReports.CreateAttribute("name");
                attr_name.Value = name;
                XmlAttribute attr_path = xmlReports.CreateAttribute("path");
                attr_path.Value = path;
                reportnode.Attributes.Append(attr_name);
                reportnode.Attributes.Append(attr_path);
                typenode.AppendChild(reportnode);
            }
            
            return true;
        }
    }
}
