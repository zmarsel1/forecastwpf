using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using PluginInterface;
using System.Globalization;

namespace ZoneEditor
{
    public class CalendarDocument
    {
        bool bReadOnly = true;
        protected string strBodyTable = "dbo.Calendar";
        protected string sqlConnection = string.Empty;
        DBDocumentLocker bLock = new DBDocumentLocker();

        public bool IsReadOnly { get { return bReadOnly; } protected set { bReadOnly = value; } }
        public int Year {get; protected set;}
        public int Month { get; protected set; }
        public string DocTitle { get; protected set; }

        protected DataTable tableBody = new DataTable();

        public virtual DataTable Body { get { return tableBody; } }

        public bool BeginEdit()
        {
            if (!bLock.Locked)
            {
                try
                {
                    bLock.Lock("Cldr" + Year.ToString() + Month.ToString(), sqlConnection);
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
        public virtual bool Save()
        {
            if (bReadOnly) return false;
            using (var sql = new SqlConnection(sqlConnection))
            {
                sql.Open();
                var cmd = new SqlCommand();
                cmd.CommandText = "select Day, Type from " + strBodyTable + " WHERE Year(day) = @year and Month(day) = @month";
                cmd.Parameters.AddWithValue("@year", Year);
                cmd.Parameters.AddWithValue("@month", Month);
                cmd.Connection = sql;

                var adapter = new SqlDataAdapter(cmd);

                var data = new DataTable();
                adapter.Fill(data);
                data.PrimaryKey = new DataColumn[] { data.Columns["Day"] };

                //команда на вставку записи
                SqlCommand insCmd = new SqlCommand(
                "insert into " + strBodyTable + " (Day, Type) values(@day, @type)", sql);
                insCmd.Parameters.Add("@day", SqlDbType.Date, 3, "Day");
                insCmd.Parameters.Add("@type", SqlDbType.Bit, 1, "Type");
                adapter.InsertCommand = insCmd;

                //команда на обновление

                SqlCommand upCmd = new SqlCommand(
                    "update " + strBodyTable + " set Day = @day, Type = @type WHERE Day = @oldDay and Type = @oldType", sql);
                upCmd.Parameters.Add("@day", SqlDbType.Date, 3, "Day");
                upCmd.Parameters.Add("@type", SqlDbType.Bit, 1, "Type");
                
                SqlParameter parameter = upCmd.Parameters.Add("@oldDay", SqlDbType.Date, 3, "Day");
                parameter.SourceVersion = DataRowVersion.Original;
                parameter = upCmd.Parameters.Add("@oldType", SqlDbType.Bit, 1, "Type");
                parameter.SourceVersion = DataRowVersion.Original;

                adapter.UpdateCommand = upCmd;

                //команда на удаление
                SqlCommand delCmd = new SqlCommand(
                    "delete from " + strBodyTable + " where Day = @day and Type = @type", sql);
                delCmd.Parameters.Add("@day", SqlDbType.Date, 3, "Day");
                delCmd.Parameters.Add("@type", SqlDbType.Bit, 1, "Type");
                adapter.DeleteCommand = delCmd;

                data.Merge(tableBody);
                adapter.Update(data);
                return true;
            }
 
        }
        public virtual bool Load(string connection, int year, int month)
        {
            tableBody = new DataTable();

            sqlConnection = connection;
            Year = year;
            Month = month;
            DocTitle = string.Format("Календарь {0} {1} год", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month), Year);
            var sql = new SqlConnection(connection);
            //считываем тело документа
            var cmd = new SqlCommand("select Day, Type from " + strBodyTable + " WHERE Year(day) = @year and Month(day) = @month", sql);
            cmd.Parameters.AddWithValue("@year", Year);
            cmd.Parameters.AddWithValue("@month", Month);
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(tableBody);
            tableBody.PrimaryKey = new DataColumn[] {tableBody.Columns["Day"]};

            if (tableBody.Rows.Count != DateTime.DaysInMonth(Year, Month))
            {
                tableBody.Clear(); //опустошаем таблицу от некорректных данных
                for (DateTime date = new DateTime(Year, Month, 1);
                    date.Month == Month;
                    date = date.AddDays(1)) // перебираем все даты в заданном месяце
                {
                    DataRow row = tableBody.NewRow();
                    row["Day"] = date;
                    row["Type"] = (date.DayOfWeek != DayOfWeek.Saturday) && (date.DayOfWeek != DayOfWeek.Sunday); // по умолчанию суббота и воскресенье выходные дни

                    tableBody.Rows.Add(row);
                }
            }

            return true;
        }
    }
}
