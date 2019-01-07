using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Windows;
using System.Data;
using System.Data.SqlClient;

namespace BasePlugin2
{
    /// <summary>Plugin класс для создания редактрования и удаления документов.
    /// </summary>
    public class Plugin : IDocPlugin
    {
        /// <summary>
        /// Метод создаёт документ в базе данных на основе родительского документа.
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="type">Тип документа</param>
        /// <param name="obj">Узел учёта</param>
        /// <param name="date">Дата Документа</param>
        /// <param name="basedoc">Номер Родительского документа</param>
        /// <returns>Возвращает номер документа</returns>
        public int CreateDocument(string connection, string type, string obj, DateTime date, int basedoc)
        {
            using (var sql = new SqlConnection(connection))
            {
                sql.Open();

                var str = string.Format("dbo.sp_create_document");
                var cmd = new SqlCommand(str, sql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.Date));
                cmd.Parameters.Add(new SqlParameter("@unitsname", SqlDbType.NChar));
                cmd.Parameters.Add(new SqlParameter("@doctype", SqlDbType.NChar));
                cmd.Parameters.Add(new SqlParameter("@parent", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Direction = ParameterDirection.Output;

                cmd.Parameters["@date"].Value = date;
                cmd.Parameters["@unitsname"].Value = obj;
                cmd.Parameters["@doctype"].Value = type;
                cmd.Parameters["@parent"].Value = basedoc;

                cmd.ExecuteNonQuery();
                int document = (int)cmd.Parameters["@id"].Value;
                sql.Close();
                return document;
            }
        }
        /// <summary>
        /// Метод создаёт документ в базе данных на основе шаблона.
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="type">Тип документа</param>
        /// <param name="obj">Узел учёта</param>
        /// <param name="date">Дата Документа</param>
        /// <returns>Возвращает номер документа</returns>
        public int CreateDocument(string connection, string type, string obj, DateTime date)
        {
            using (var sql = new SqlConnection(connection))
            {
                sql.Open();

                var str = string.Format("dbo.sp_create_document");
                var cmd = new SqlCommand(str, sql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.Date));
                cmd.Parameters.Add(new SqlParameter("@unitsname", SqlDbType.NChar));
                cmd.Parameters.Add(new SqlParameter("@doctype", SqlDbType.NChar));
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Direction = ParameterDirection.Output;

                cmd.Parameters["@date"].Value = date;
                cmd.Parameters["@unitsname"].Value = obj;
                cmd.Parameters["@doctype"].Value = type;

                cmd.ExecuteNonQuery();
                int document = (int)cmd.Parameters["@id"].Value;
                sql.Close();
                return document;
            }
        }
        /// <summary>
        /// Метод вызывает окно редактирования для заданного номера документа.
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="docnum">Номер редактируемого документа</param>
        /// <returns>Возвращает окно для редактирования</returns>
        public Window EditDocument(string connection, int docnum)
        {
            CrossDocument document = new CrossDocument();
            document.Load(connection, docnum);
            Window form = new EditWindow2(document);
            return form;
        }
        /// <summary>
        /// Удаляет документ по заданному номеру
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="docnum">Номер удаляемого документа</param>
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
        /// Метод отбирает документы заданного типа и узла учёта в заданном временном интервале.
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="type">Тип документа</param>
        /// <param name="obj">Узел учёта</param>
        /// <param name="start">Начальная дата поиска</param>
        /// <param name="end">Конечная дата поиска</param>
        /// <returns>Возвращает выборку в формате <c>DataTable</c></returns>
        public DataTable SelectDocuments(string connection, string type, string obj, DateTime start, DateTime end)
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
