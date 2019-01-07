using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using PluginInterface;
using System.Windows;
using System.ComponentModel;

namespace PluginInterface
{
    /// <summary>
    /// <c>Document</c> класс для хранения, редактриования документа ы плоском виде
    /// </summary>
    public class Document
    {
        bool bReadOnly = true;
        protected string strBodyTable = string.Empty;
        protected string sqlConnection = string.Empty;
        DBDocumentLocker bLock = new DBDocumentLocker();

        /// <summary>
        /// Поле указывает можно ли редактировать документ
        /// </summary>
        public bool IsReadOnly { get { return bReadOnly; } protected set { bReadOnly = value; } }
        /// <summary>
        /// Поле хранит номер загруженного документа
        /// </summary>
        public int DocumentId {get; protected set;}
        /// <summary>
        /// Поле хранит свойства документа
        /// </summary>
        public string DocInfo { get; protected set; }
        /// <summary>
        /// Поле хранить шапку документа
        /// </summary>
        public string DocTitle { get; protected set; }

        protected DataTable tableBody = new DataTable();
        protected DataTable tableRows = new DataTable();
        protected DataTable tableEntities = new DataTable();

        /// <summary>
        /// Поле хранит представление документа
        /// </summary>
        public virtual DataTable Body { get { return tableBody; } }
        /// <summary>
        /// Поле Хранит нумерацию документа
        /// </summary>
        public DataTable Rows { get { return tableRows; } }
        /// <summary>
        /// Поле хранит номенклатуру документа
        /// </summary>
        public DataTable Entities { get { return tableEntities; } }
        /// <summary>
        /// Метод переводит документ в состояние редактирования
        /// </summary>
        /// <returns>Возвращает успешность перехода</returns>
        public bool BeginEdit()
        {
            //написать проверку на права доступа проверку на права доступа
            bool result = false;
            try
            {
                using (var sql = new SqlConnection(sqlConnection))
                {
                    var cmd = new SqlCommand("select dbo.fn_can_edit_document(@docid)", sql);
                    cmd.Parameters.Add(new SqlParameter("@docid", DocumentId));

                    sql.Open();
                    result = (bool)cmd.ExecuteScalar();
                    sql.Close();
                }
            }
            catch
            {
                return false;
            }
            if (!result) return false; // документ не доступен для редактирования
            if (!bLock.Locked)
            {
                try
                {
                    bLock.Lock(DocumentId.ToString(), sqlConnection);
                }
                catch
                {
                    return false;
                }
            }
            bReadOnly = !bLock.Locked;
            return bLock.Locked;
        }
        /// <summary>
        /// Метод заканчивает редактирование
        /// </summary>
        public void EndEdit()
        {
            bLock.Unlock();
            IsReadOnly = true;
        }
        /// <summary>
        /// Метод сохраняет документ
        /// </summary>
        /// <returns></returns>
        public virtual bool Save()
        {
            if (bReadOnly) return false;
            using (var sql = new SqlConnection(sqlConnection))
            {
                sql.Open();
                var cmd = new SqlCommand();
                cmd.CommandText = "select DocumentId, RowKey, EntityKey, EntityData from " + strBodyTable + " WHERE DocumentId = @document";
                cmd.Parameters.Add(new SqlParameter("@document", SqlDbType.Int));
                cmd.Parameters["@document"].Value = DocumentId;
                cmd.Connection = sql;

                var adapter = new SqlDataAdapter(cmd);

                var data = new DataTable();
                adapter.Fill(data);
                data.PrimaryKey = new DataColumn[] { data.Columns["DocumentId"], data.Columns["RowKey"], data.Columns["EntityKey"] };

                //команда на вставку записи
                SqlCommand insCmd = new SqlCommand(
                "insert into " + strBodyTable + " (DocumentId, RowKey, EntityKey, EntityData) values(@DocumentId, @RowKey, @EntityKey, @EntityData)", sql);
                insCmd.Parameters.Add("@DocumentId", SqlDbType.Int, 4, "DocumentId");
                insCmd.Parameters.Add("@RowKey", SqlDbType.Int, 4, "RowKey");
                insCmd.Parameters.Add("@EntityKey", SqlDbType.Int, 4, "EntityKey");
                insCmd.Parameters.Add("@EntityData", SqlDbType.Float, 8, "EntityData");
                adapter.InsertCommand = insCmd;

                //команда на обновление

                SqlCommand upCmd = new SqlCommand(
                    "update " + strBodyTable + " set DocumentId = @DocumentId, EntityData = @EntityData, EntityKey = @EntityKey, RowKey = @RowKey where DocumentId = @oldDocumentId and RowKey = @oldRowKey and EntityKey = @oldEntityKey", sql);
                upCmd.Parameters.Add("@DocumentId", SqlDbType.Int, 4, "DocumentId");
                upCmd.Parameters.Add("@RowKey", SqlDbType.Int, 4, "RowKey");
                upCmd.Parameters.Add("@EntityKey", SqlDbType.Int, 4, "EntityKey");
                upCmd.Parameters.Add("@EntityData", SqlDbType.Float, 8, "EntityData");

                SqlParameter parameter = upCmd.Parameters.Add("@oldDocumentId", SqlDbType.Int, 4, "DocumentId");
                parameter.SourceVersion = DataRowVersion.Original;
                parameter = upCmd.Parameters.Add("@oldRowKey", SqlDbType.Int, 4, "RowKey");
                parameter.SourceVersion = DataRowVersion.Original;
                parameter = upCmd.Parameters.Add("@oldEntityKey", SqlDbType.Int, 4, "EntityKey");
                parameter.SourceVersion = DataRowVersion.Original;

                adapter.UpdateCommand = upCmd;

                //команда на удаление
                SqlCommand delCmd = new SqlCommand(
                    "delete from " + strBodyTable + " where DocumentId = @DocumentId and RowKey = @RowKey and EntityKey = @EntityKey", sql);
                delCmd.Parameters.Add("@DocumentId", SqlDbType.Int, 4, "DocumentId");
                delCmd.Parameters.Add("@RowKey", SqlDbType.Int, 4, "RowKey");
                delCmd.Parameters.Add("@EntityKey", SqlDbType.Int, 4, "EntityKey");
                adapter.DeleteCommand = delCmd;

                data.Merge(tableBody);
                adapter.Update(data);
                return true;
            }
 
        }
        /// <summary>
        /// Метод загружает документ
        /// </summary>
        /// <param name="connection">Строка соединения с БД</param>
        /// <param name="num">Номер документа</param>
        /// <returns>Возвращает успешность загрузки</returns>
        public virtual bool Load(string connection, int num)
        {
            DocumentId = num;
            sqlConnection = connection;
            var sql = new SqlConnection(connection);
            //узнаем название View, в котором хранится тело документа
            var cmd = new SqlCommand("select dbo.fn_get_document_table(@document)", sql);
            cmd.Parameters.AddWithValue("@document", DocumentId);
            sql.Open();
            strBodyTable = cmd.ExecuteScalar().ToString();
            sql.Close();
            // считывваем шапку документа
            cmd.CommandText = "select 'Документ №: ' + CAST(DocumentId as varchar(20)) + CHAR(13) + CHAR(10) + 'Дата Документа: ' + CONVERT(char(12), DocumentDate, 102) + CHAR(13)+ CHAR(10) + 'Объект: ' + dbo.Trim(UnitName) from dbo.fn_get_documents() where DocumentId = @document";
            sql.Open();
            DocInfo = cmd.ExecuteScalar().ToString();
            sql.Close();
            // считываем название типа документа
            cmd.CommandText = "select dbo.Trim(DocTypeName) + ' № ' + CAST(DocumentId as varchar(20)) from dbo.fn_get_documents() where DocumentId = @document";
            sql.Open();
            DocTitle = cmd.ExecuteScalar().ToString();
            sql.Close();
            //считываем тело документа
            cmd.CommandText = "select DocumentId, RowKey, EntityKey, EntityData from " + strBodyTable + " WHERE DocumentId = @document";
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(tableBody);
            tableBody.PrimaryKey = new DataColumn[] { tableBody.Columns["DocumentId"],tableBody.Columns["RowKey"],tableBody.Columns["EntityKey"]};
            tableBody.Columns["DocumentId"].ReadOnly = true;
            // считываем нумерацию документа
            cmd.CommandText = "select RowKey, RTRIM(RowName) as RowName from fn_get_document_rows(@document)";
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(tableRows);
            // считываем номенклатуру документа
            cmd.CommandText = "select * from fn_get_document_entities(@document) order by EntityKey";
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(tableEntities);

            return true;
        }
    }
}
