using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Net;
using System.Net.Sockets;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using ForecastConnection;

namespace ForecastPlugin
{
    /// <summary>
    /// Класс для создания, удаления прогнозов
    /// </summary>
    public class Plugin : IDocPlugin
    {
        /// <summary>
        /// Метод создаёт прогноз по заданному узлу на заданную дату. Метод не реализован.
        /// </summary>
        /// <param name="connection">строка соединения Сервером Прогнозов</param>
        /// <param name="type">Тип прогноза</param>
        /// <param name="obj">Узел учёта</param>
        /// <param name="date">Дата прогноза</param>
        /// <param name="basedoc">Родительский документа</param>
        /// <returns>Возвращает номер документа в виде <c>int</c></returns>
        public int CreateDocument(string connection, string type, string obj, DateTime date, int basedoc)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Метод создаёт прогноз по заданному узлу на заданную дату.
        /// </summary>
        /// <param name="connection">строка соединения Сервером Прогнозов</param>
        /// <param name="type">Тип прогноза</param>
        /// <param name="obj">Узел учёта</param>
        /// <param name="date">Дата прогноза</param>
        /// <returns>Возвращает номер документа в виде <c>int</c></returns>
        public int CreateDocument(string connection, string type, string obj, DateTime date)
        {
            FConnection conn = new FConnection(connection);
            FCommand cmd = new FCommand(string.Format("{0} {1:yyyy-MM-dd} {2}", obj, date, type), conn);
            conn.Open();
            string result = cmd.Execute();
            conn.Close();

            int r = 0;
            if (!int.TryParse(result, out r))
            {
                throw new Exception(result);
            }
            return r;
        }
        /// <summary>
        /// Вызывает окно для просмотра и редактирования(если достаточно прав) Прогноза
        /// </summary>
        /// <param name="connection">строка соединения Сервером Прогнозов</param>
        /// <param name="docnum">номер документа для просмотра</param>
        /// <returns>возвращает окно для просмотра(редатирования)</returns>
        public System.Windows.Window EditDocument(string connection, int docnum)
        {
            Document document = new Document();
            document.Load(connection, docnum);
            Window form = new EditWindow(document);
            return form;
        }
        /// <summary>
        /// Метод для удаления прогноза
        /// </summary>
        /// <param name="connection">строка соединения Сервером Прогнозов</param>
        /// <param name="docnum">номер документа для удаления</param>
        public void DeleteDocument(string connection, int docnum)
        {
            using (var sql = new SqlConnection(connection))
            {
                sql.Open();

                var str = string.Format("dbo.sp_delete_document");
                var cmd = new SqlCommand(str, sql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));

                cmd.Parameters["@id"].Value = docnum;

                cmd.ExecuteNonQuery();
                sql.Close();
            }
        }
        /// <summary>
        /// Метод отбирает документы заданного типа на заданный промежуток времени
        /// </summary>
        /// <param name="connection">строка соединения Сервером Прогнозов</param>
        /// <param name="type">Тип документа</param>
        /// <param name="obj">Узел учёта</param>
        /// <param name="start">Начальная дата поиска</param>
        /// <param name="end">Конечная дата поиска</param>
        /// <returns>Список в виде <c>DataTable</c>, отобранных документов</returns>
        public System.Data.DataTable SelectDocuments(string connection, string type, string obj, DateTime start, DateTime end)
        {
            using (var sql = new SqlConnection(connection))
            {
                var cmd = new SqlCommand();
                cmd.CommandText = "Select * from dbo.SelectDocuments() WHERE Suffix=@type AND UnitSName=@unit";
                cmd.Parameters.Add(new SqlParameter("@type", type));
                cmd.Parameters.Add(new SqlParameter("@unit", obj));
                cmd.Connection = sql;

                var data = new DataTable();
                var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(data);

                return data;
            }
        }
    }
}
