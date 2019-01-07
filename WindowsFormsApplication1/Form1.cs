using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReportWnd wnd = new ReportWnd("http://192.168.0.100:8080", "/program/standardform1", "m", "-1");
            wnd.AddParameter("DocID", "67");
            wnd.ShowDialog();
        }
    }
}
