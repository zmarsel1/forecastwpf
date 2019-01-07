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
    /// <c>CrossDocument</c> класс для представления документа в виде Кросс-таблицы
    /// </summary>
    public class CrossDocument : Document
    {
        //таблица хранит данные в кроссированном виде
        DataTable tableMatrixView = new DataTable();
        /// <summary>
        /// Представление тела документа
        /// </summary>
        public override DataTable Body { get { return tableMatrixView; } }
        /// <summary>
        /// Метод сохраняет документ
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            if (IsReadOnly) return false;
            // обновляем tableBody из tableMatrixView
            foreach (DataRow row in tableBody.Rows)
            {
                DataRow source = tableMatrixView.Rows.Find(row["RowKey"]);
                row["EntityData"] = (bool)source[row["EntityKey"].ToString()] ? 1.0 : 0.0;
            }
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
        /// <param name="connection">Строка соединения с базой данных</param>
        /// <param name="num">Номер документа</param>
        /// <returns>возварщает успешность загрузки документа.</returns>
        public override bool Load(string connection, int num)
        {
            DocumentId = num;
            sqlConnection = connection;
            var sql = new SqlConnection(connection);

            var cmd = new SqlCommand("select dbo.fn_get_document_table(@document)", sql);
            cmd.Parameters.AddWithValue("@document", DocumentId);
            sql.Open();
            strBodyTable = cmd.ExecuteScalar().ToString();
            sql.Close();

            cmd.CommandText = "select 'Документ №: ' + CAST(DocumentId as varchar(20)) + CHAR(13) + CHAR(10) + 'Дата Документа: ' + CONVERT(char(12), DocumentDate, 102) + CHAR(13)+ CHAR(10) + 'Объект: ' + dbo.Trim(UnitName) from dbo.fn_get_documents() where DocumentId = @document";
            sql.Open();
            DocInfo = cmd.ExecuteScalar().ToString();
            sql.Close();

            cmd.CommandText = "select dbo.Trim(DocTypeName) + ' № ' + CAST(DocumentId as varchar(20)) from dbo.fn_get_documents() where DocumentId = @document";
            sql.Open();
            DocTitle = cmd.ExecuteScalar().ToString();
            sql.Close();

            cmd.CommandText = "select DocumentId, RowKey, EntityKey, EntityData from " + strBodyTable + " WHERE DocumentId = @document";
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(tableBody);
            tableBody.PrimaryKey = new DataColumn[] { tableBody.Columns["DocumentId"],tableBody.Columns["RowKey"],tableBody.Columns["EntityKey"]};
            tableBody.Columns["DocumentId"].ReadOnly = true;

            cmd.CommandText = "select RowKey, RTRIM(RowName) as RowName from fn_get_document_rows(@document)";
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(tableRows);

            cmd.CommandText = "select * from fn_get_document_entities(@document) order by EntityKey";
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(tableEntities);
            //инициализация tableMatrixView
            //дополнение tableBody новыми Entity
            tableMatrixView.Columns.Add("RowKey", typeof(int)).Caption = "Время";
            foreach (DataRow entity in tableEntities.Rows)
            {
                tableMatrixView.Columns.Add(entity["EntityKey"].ToString(), typeof(bool)).Caption = (string)entity["EntityName"]; //добавляем колонки к матрице
                foreach(DataRow row in tableRows.Rows)
                {
                    object[] key = new object[]{DocumentId, row["RowKey"], entity["EntityKey"]};
                    if (!tableBody.Rows.Contains(key))
                    {
                        DataRow r = tableBody.NewRow();
                        r["DocumentId"] = DocumentId;
                        r["RowKey"] = row["RowKey"];
                        r["EntityKey"] = entity["EntityKey"];
                        r["EntityData"] = 0.0;
                        tableBody.Rows.Add(r);
                    };
                }
            }
            tableMatrixView.PrimaryKey = new DataColumn[] { tableMatrixView.Columns["RowKey"] };
            //преобразование tableBody в tableMatrixView
            foreach (DataRow row in tableRows.Rows)
            {
                DataRow r = tableMatrixView.NewRow();
                r["RowKey"] = row["RowKey"];
                foreach (DataRow entity in tableEntities.Rows)
                {
                    DataRow s = tableBody.Rows.Find(new object[] { DocumentId, row["RowKey"], entity["EntityKey"] });
                    r[entity["EntityKey"].ToString()] = (double)s["EntityData"] != 0.0;
                }
                tableMatrixView.Rows.Add(r);
            }
            return true;
        }
    }
}
