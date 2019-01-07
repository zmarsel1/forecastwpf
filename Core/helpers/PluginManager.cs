using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using NLog;
using PluginInterface;
using System.Reflection;

namespace Core
{
    /// <summary>
    /// класс <c>PluginManager</c> отвечает за згруженные плагины
    /// </summary>
    class PluginManager
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region Singleton
        PluginManager() { }
 
        static PluginManager instance = null;
        static object locker = new object();
        /// <summary>
        /// Возвращает глобальный экземпляр <c>PluginManager</c>
        /// </summary>
        public static PluginManager Instance
        {
            get 
            {
                lock(locker)
                {
                    if (instance == null) instance = new PluginManager();
                    return instance;
                }
            }
        }
        #endregion

        Dictionary<string, IDocPlugin> mapPlugins = new Dictionary<string, IDocPlugin>();
        /// <summary>
        /// Поле хранит словарь плагинов
        /// </summary>
        public Dictionary<string, IDocPlugin> Plugins { get { return mapPlugins; } }
        /// <summary>
        /// Метод загружает плагины
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <returns>Возвращает успешность выполнения метода</returns>
        public bool LoadPlugins(string connection)
        {
            var sql = new SqlConnection(connection);
            var cmd = new SqlCommand("SELECT RTRIM(Suffix) as Suffix, RTRIM(DocTypeLib) as DocTypeLib FROM dbo.DocType", sql);
            var adapter = new SqlDataAdapter(cmd);
            var data = new DataTable();

            try
            {
                adapter.Fill(data);
            }
            catch (Exception e)
            {
                logger.Fatal("Ошибка загрузки списка плагинов.\r\n Exception: {0}", e);
                return false;
            }
            if (data.Rows.Count == 0)
            {
                logger.Error("Список плагинов пуст.");
                return false;
            }
            bool result = false;
            foreach (DataRow row in data.Rows)
            {
                result = true;
                var suffix = row["Suffix"] as string;
                var lib = row["DocTypeLib"] as string;

                try
                {
                    Assembly assembly = Assembly.LoadFile(string.Format(@"{0}\{1}", Environment.CurrentDirectory,lib));
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.IsClass || type.IsNotPublic) continue;

                        Type iface = type.GetInterface("IDocPlugin");
                        if (iface != null)
                        {
                            IDocPlugin obj = Activator.CreateInstance(type) as IDocPlugin;
                            mapPlugins[suffix] = obj;
                        }
                    }
                }
                catch (Exception e)
                {
                    result = false;
                    logger.Error("Ошибка импорта плагина {0} : {1}. \r\n Exception: {2}", suffix, lib, e);
                }
            }
            return result;
        }
    }
}
