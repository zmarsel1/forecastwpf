using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Security.Principal;
using Microsoft.Reporting.WinForms;

namespace Core
{
    public partial class ReportWnd : Form
    {
        string username = string.Empty;
        List<ReportParameter> parameters = new List<ReportParameter>();

        public string Server { get; set;}
        public string ReportPath { get; set; }
        public string UserName
        {
            get {return this.username;}
            set {
                string username = value;
                if (username.Contains("\\"))
                {
                    this.Domain = username.Substring(0, username.IndexOf("\\"));
                    this.username = username.Substring(username.IndexOf("\\") + 1);
                }
                else
                {
                    this.username = username;
                }
            }
        }
        public string Password { get; set; }
        public string Domain { get; set; }

        public ReportWnd()
        {
            InitializeComponent();
        }
        public ReportWnd(string server, string path, string user, string pwd) :
            this(server, path, user, pwd, "")
        {
            this.Domain = WindowsIdentity.GetCurrent().Name.Split('\\')[0];
        }

        public ReportWnd(string server, string path, string user, string pwd, string domain)
        {
            InitializeComponent();
            Server = server;
            ReportPath = path;
            UserName = user;
            Password = pwd;
            Domain = domain;
        }

        private void ReportWnd_Load(object sender, EventArgs e)
        {
            try
            {
                var ReportServer = Report.ServerReport;
                ReportServer.ReportServerUrl = new Uri(Server);
                ReportServer.ReportPath = ReportPath;

                NetworkCredential nc = new NetworkCredential(UserName, Password, Domain);
                ReportServer.ReportServerCredentials.NetworkCredentials = nc;
                ReportServer.SetParameters(parameters);
                this.Report.RefreshReport();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Ошибка выполнения отчёта.\n" + exception.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

        }
        public void AddParameter(string param, string value)
        {
            parameters.Add(new ReportParameter(param, value, false));
        }
        public void AddParameter(string param, string[] value)
        {
            parameters.Add(new ReportParameter(param, value, false));
        }
        public void ClearParameters()
        {
            parameters.Clear();
        }
    }
}
