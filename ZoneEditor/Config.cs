using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ZoneEditor
{
    public partial class Config : Form
    {
        public Config()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder strConnection = new SqlConnectionStringBuilder();
            strConnection.DataSource = txtServer.Text;
            strConnection.InitialCatalog = txtDB.Text;
            strConnection.IntegratedSecurity = rSSPI.Checked;
            if (!rSSPI.Checked)
            {
                strConnection.UserID = txtLogin.Text;
                strConnection.Password = txtPwd.Text;
            }
            try
            {
                using (SqlConnection sql = new SqlConnection(strConnection.ConnectionString))
                { 
                    sql.Open();
                    sql.Close();
                    MessageBox.Show("Соединение с сервером успешно установлено.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Соединение с сервером не установлено.\n" + exception.Message, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Error);   
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            SqlConnectionStringBuilder strConnection = new SqlConnectionStringBuilder();
            strConnection.DataSource = txtServer.Text;
            strConnection.InitialCatalog = txtDB.Text;
            strConnection.IntegratedSecurity = rSSPI.Checked;
            if (!rSSPI.Checked)
            {
                strConnection.UserID = txtLogin.Text;
                strConnection.Password = txtPwd.Text;
            }

            config.AppSettings.Settings["sqlConnection"].Value = strConnection.ConnectionString;
            ConfigurationManager.RefreshSection("appSettings");
            config.Save(ConfigurationSaveMode.Full);
            MessageBox.Show("Настройки сохранены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Config_Load(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder strConnection = new SqlConnectionStringBuilder(ConfigurationManager.AppSettings["sqlConnection"]);
            txtServer.Text = strConnection.DataSource;
            txtDB.Text = strConnection.InitialCatalog;
            rSSPI.Checked = strConnection.IntegratedSecurity;
            if (!rSSPI.Checked)
            {
                txtLogin.Text = strConnection.UserID;
                txtPwd.Text = strConnection.Password;
            }
            rSSPI_CheckedChanged(null, null);
        }

        private void rSSPI_CheckedChanged(object sender, EventArgs e)
        {
            bool state = !rSSPI.Checked;
            label3.Enabled = state;
            label4.Enabled = state;
            txtLogin.Enabled = state;
            txtPwd.Enabled = state;
        }
    }
}
