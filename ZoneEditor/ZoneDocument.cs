using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace ZoneEditor
{
    class ZoneDocument
    {
        bool bReadOnly = true;
        string strBodyTable = "dbo.Zone";
        string sqlConnection = string.Empty;
        PluginInterface.DBDocumentLocker bLock = new PluginInterface.DBDocumentLocker();

        public int Year { get; protected set; }
        public int Month { get; protected set; }
        public string DocTitle { get; protected set; }

        DataTable tableMatrixView = new DataTable();
        DataTable tableBody = new DataTable();
        DataTable tableRows = new DataTable();
        DataTable tableEntities = new DataTable();

        public DataTable Body { get { return tableMatrixView; } }
        public DataTable Rows { get { return tableRows; } }
        public DataTable Entities { get { return tableEntities; } }
        public bool IsReadOnly { get { return bReadOnly; } protected set { bReadOnly = value; } }
        public bool BeginEdit()
        {
            if (!bLock.Locked)
            {
                try
                {
                    bLock.Lock("Zone" + Year.ToString() + Month.ToString(), sqlConnection);
                }
                catch
                {
                    return false;
                }
            }
            bReadOnly = !bLock.Locked;
            return bLock.Locked;
        }
        public void EndEdit()
        {
            bLock.Unlock();
            IsReadOnly = true;
        }
        
        public bool Save()
        {
            if (IsReadOnly) return false;
            // обновляем tableBody из tableMatrixView
            foreach (DataRow row in tableBody.Rows)
            {
                DataRow source = tableMatrixView.Rows.Find(row["RowKey"]);
                row["Value"] = (bool)source[row["EntityKey"].ToString()] ? 1.0 : 0.0;
            }
            using (var sql = new SqlConnection(sqlConnection))
            {
                sql.Open();
                var cmd = new SqlCommand();
                cmd.CommandText = "select YearNum, MonthNum, RowKey, EntityKey, Value from " + strBodyTable + " WHERE YearNum = @year and MonthNum = @month";
                cmd.Parameters.AddWithValue("@year", Year);
                cmd.Parameters.AddWithValue("@month", Month);
                cmd.Connection = sql;

                var adapter = new SqlDataAdapter(cmd);

                var data = new DataTable();
                adapter.Fill(data);
                data.PrimaryKey = new DataColumn[] { data.Columns["YearNum"], data.Columns["MonthNum"], data.Columns["RowKey"], data.Columns["EntityKey"] };

                //команда на вставку записи
                SqlCommand insCmd = new SqlCommand(
                "insert into " + strBodyTable + " (YearNum, MonthNum, RowKey, EntityKey, Value) values(@year, @month, @RowKey, @EntityKey, @value)", sql);
                insCmd.Parameters.Add("@year", SqlDbType.Int, 4, "YearNum");
                insCmd.Parameters.Add("@month", SqlDbType.Int, 4, "MonthNum");
                insCmd.Parameters.Add("@RowKey", SqlDbType.Int, 4, "RowKey");
                insCmd.Parameters.Add("@EntityKey", SqlDbType.Int, 4, "EntityKey");
                insCmd.Parameters.Add("@value", SqlDbType.Int, 4, "Value");
                adapter.InsertCommand = insCmd;

                //команда на обновление

                SqlCommand upCmd = new SqlCommand(
                    "update " + strBodyTable + " set YearNum = @year, MonthNum=@month, Value = @value, EntityKey = @EntityKey, RowKey = @RowKey where YearNum = @oldyear and MonthNum=@oldmonth and RowKey = @oldRowKey and EntityKey = @oldEntityKey", sql);
                upCmd.Parameters.Add("@year", SqlDbType.Int, 4, "YearNum");
                upCmd.Parameters.Add("@month", SqlDbType.Int, 4, "MonthNum");
                upCmd.Parameters.Add("@RowKey", SqlDbType.Int, 4, "RowKey");
                upCmd.Parameters.Add("@EntityKey", SqlDbType.Int, 4, "EntityKey");
                upCmd.Parameters.Add("@value", SqlDbType.Int, 4, "Value");

                SqlParameter parameter = upCmd.Parameters.Add("@oldyear", SqlDbType.Int, 4, "YearNum");
                parameter.SourceVersion = DataRowVersion.Original;
                parameter = upCmd.Parameters.Add("@oldmonth", SqlDbType.Int, 4, "MonthNum");
                parameter.SourceVersion = DataRowVersion.Original;
                parameter = upCmd.Parameters.Add("@oldRowKey", SqlDbType.Int, 4, "RowKey");
                parameter.SourceVersion = DataRowVersion.Original;
                parameter = upCmd.Parameters.Add("@oldEntityKey", SqlDbType.Int, 4, "EntityKey");
                parameter.SourceVersion = DataRowVersion.Original;

                adapter.UpdateCommand = upCmd;

                //команда на удаление
                SqlCommand delCmd = new SqlCommand(
                    "delete from " + strBodyTable + " where YearNum = @year and MonthNum=@month and RowKey = @RowKey and EntityKey = @EntityKey", sql);
                delCmd.Parameters.Add("@year", SqlDbType.Int, 4, "YearNum");
                delCmd.Parameters.Add("@month", SqlDbType.Int, 4, "MonthNum");
                delCmd.Parameters.Add("@RowKey", SqlDbType.Int, 4, "RowKey");
                delCmd.Parameters.Add("@EntityKey", SqlDbType.Int, 4, "EntityKey");
                adapter.DeleteCommand = delCmd;

                data.Merge(tableBody);
                adapter.Update(data);
                return true;
            }

        }
        public bool Load(string connection, int year, int month)
        {
            tableMatrixView = new DataTable();
            tableBody = new DataTable();
            tableRows = new DataTable();
            tableEntities = new DataTable();

            Year = year;
            Month = month;
            sqlConnection = connection;
            
            var sql = new SqlConnection(connection);
            var cmd = new SqlCommand();
            cmd.Connection = sql;

            DocTitle = string.Format("{0} Год {1} месяц",Year, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month));

            cmd.CommandText = "select YearNum, MonthNum, RowKey, EntityKey, Value from " + strBodyTable + " WHERE YearNum = @year and MonthNum = @month";
            cmd.Parameters.AddWithValue("@month", Month);
            cmd.Parameters.AddWithValue("@year", Year);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(tableBody);
            tableBody.PrimaryKey = new DataColumn[] { tableBody.Columns["YearNum"],tableBody.Columns["MonthNum"], tableBody.Columns["RowKey"],tableBody.Columns["EntityKey"]};
            tableBody.Columns["YearNum"].ReadOnly = true;
            tableBody.Columns["MonthNum"].ReadOnly = true;

            cmd.CommandText = "select RowKey, RTRIM(RowName) as RowName from dbo.Row";
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(tableRows);

            cmd.CommandText = "select * from dbo.Entity order by EntityKey";
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
                    object[] key = new object[]{Year, Month, row["RowKey"], entity["EntityKey"]};
                    if (!tableBody.Rows.Contains(key))
                    {
                        DataRow r = tableBody.NewRow();
                        r["YearNum"] = Year;
                        r["MonthNum"] = Month;
                        r["RowKey"] = row["RowKey"];
                        r["EntityKey"] = entity["EntityKey"];
                        r["Value"] = 0.0;
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
                    DataRow s = tableBody.Rows.Find(new object[] { Year, Month, row["RowKey"], entity["EntityKey"] });
                    r[entity["EntityKey"].ToString()] = (int)s["Value"] != 0.0;
                }
                tableMatrixView.Rows.Add(r);
            }
            return true;
        }
    }
}
