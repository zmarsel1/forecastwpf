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
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using ForecastConnection;

namespace Core
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        bool modified = false;

        public ConfigWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Загрузка настроек БД
            txtServer.Text = ConfigurationManager.AppSettings["Server"];
            txtDB.Text = ConfigurationManager.AppSettings["DataBase"];
            string sequrity = ConfigurationManager.AppSettings["IntegratedSecurity"];
            if (sequrity == "SSPI")
            {
                radioSSPI.IsChecked = true;
                txtLogin.Text= "";
                txtPwd.Password = "";
            }
            else
            {
                radioSQLServer.IsChecked = true;
                txtLogin.Text = ConfigurationManager.AppSettings["UID"];
                txtPwd.Password = Cryptography.Decrypt("secret_top_secret", ConfigurationManager.AppSettings["Password"]);
            }
            #endregion

            #region Загрузка настроек Сервера отчётов
            txtReportServer.Text = ConfigurationManager.AppSettings["ReportServer"];
            txtReportLogin.Text = ConfigurationManager.AppSettings["ReportServerUser"];
            txtReportPwd.Password = Cryptography.Decrypt("secret_top_secret", ConfigurationManager.AppSettings["ReportServerPassword"]);
            #endregion

            #region Загрузка настроек Сервера Прогнозов
            txtForecastServer.Text = ConfigurationManager.AppSettings["ForecastServer"];
            #endregion
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder();
            connection["Server"] = txtServer.Text;
            connection["Database"] = txtDB.Text;
            if ((bool)radioSSPI.IsChecked)
            {
                connection["integrated Security"] = true;
            }
            else
            {
                connection["UID"] = txtLogin.Text;
                connection["Password"] = txtPwd.Password;
            }

            SqlConnection sql = new SqlConnection(connection.ConnectionString);
            
            try
            {
                sql.Open();
                sql.Close();
                MessageBox.Show("Соединение с сервером успешно установлено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Соединение с сервером не установлено.\n" + exception.Message, "Информация", MessageBoxButton.OK, MessageBoxImage.Error);    
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            #region Сохранение Настроек БД
            config.AppSettings.Settings["Server"].Value = txtServer.Text;
            config.AppSettings.Settings["DataBase"].Value = txtDB.Text;
            if ((bool)radioSSPI.IsChecked)
            {
                config.AppSettings.Settings["IntegratedSecurity"].Value = "SSPI";
                config.AppSettings.Settings["UID"].Value = "";
                config.AppSettings.Settings["Password"].Value = "";
            }
            else
            {
                config.AppSettings.Settings["IntegratedSecurity"].Value = "NO";
                config.AppSettings.Settings["UID"].Value = txtLogin.Text;
                config.AppSettings.Settings["Password"].Value = Cryptography.Encrypt("secret_top_secret", txtPwd.Password);
            }
            #endregion
            
            #region Сохранение настроек Сервера отчетов
            config.AppSettings.Settings["ReportServer"].Value = txtReportServer.Text;
            config.AppSettings.Settings["ReportServerUser"].Value = txtReportLogin.Text;
            config.AppSettings.Settings["ReportServerPassword"].Value = Cryptography.Encrypt("secret_top_secret", txtReportPwd.Password);
            #endregion

            #region Сохранение настроек Сервера Прогнозов
            config.AppSettings.Settings["ForecastServer"].Value = txtForecastServer.Text;
            #endregion

            ConfigurationManager.RefreshSection("appSettings");
            config.Save(ConfigurationSaveMode.Full);

            modified = true;
            MessageBox.Show("Изменения сохранены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (modified)
            {
                MessageBox.Show("Требуется перезапустить приложение.","Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.MainWindow.Close();
            }
            this.Close();
        }

        private void btnReportTest_Click(object sender, RoutedEventArgs e)
        {
            ReportWnd wnd = new ReportWnd();
            wnd.Server = txtReportServer.Text;
            wnd.UserName = txtReportLogin.Text;
            wnd.Password = txtReportPwd.Password;
            wnd.ReportPath = @"/TestReport";
            try
            {
                wnd.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Возникла ошибка при попытке соединиться с Сервером отчётов.\n {0}", exception.Message), "Информация", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnForecastTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FConnection conn = new FConnection(txtForecastServer.Text);
                FCommand cmd = new FCommand("test", conn);
                conn.Open();
                string response = cmd.Execute();
                conn.Close();

                if (response == "done") MessageBox.Show("Соединение с сервером прогнозов успешно установлено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show("Соединение с сервером пронозов установить не удалось.", "Информация", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Возникла ошибка при попытке соединиться с Сервером Прогнозов.\n {0}", exception.Message), "Информация", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
