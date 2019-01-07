using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace ZoneEditor
{
    public partial class ZoneEditor : Form
    {
        ZoneDocument zoneDocument = new ZoneDocument();
        CalendarDocument cldrDocument = new CalendarDocument();
        
        bool initialized_zone = false;
        bool initialized_cldr = false;
        string sqlConnection = string.Empty;

        public ZoneEditor()
        {
            InitializeComponent();
            sqlConnection = ConfigurationManager.AppSettings["sqlConnection"];
        }

        private void ZoneEditor_Load(object sender, EventArgs e)
        {
            txtYear.Text = DateTime.Today.Year.ToString();
            cmbMonth.SelectedIndex = 0;

            txtCldrYear.Text = DateTime.Today.Year.ToString();
            cmbCldrMonth.SelectedIndex = 0;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (!zoneDocument.IsReadOnly)
            {
                if (MessageBox.Show("Документ не сохранён. Продолжить загрузку?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                {
                    zoneDocument.EndEdit();
                }
                else
                {
                    return;
                }
            }
            
            int Year = 0 ;
            if (!int.TryParse(txtYear.Text, out Year)) Year = DateTime.Today.Year; 
            int Month = cmbMonth.SelectedIndex + 1;

            try
            {
                zoneDocument.Load(sqlConnection, Year, Month);
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Не удалось открыть документ\n{0}",exception.Message));
                return;
            }
            bool editable= zoneDocument.BeginEdit();
            gridData.DataSource = zoneDocument.Body;
            gridData.ReadOnly = !editable;

            if (!initialized_zone)
            {
                DataGridViewComboBoxColumn rowcolumn = new DataGridViewComboBoxColumn();
                rowcolumn.ReadOnly = true;
                rowcolumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                rowcolumn.DataPropertyName = "RowKey";
                rowcolumn.HeaderText = "Время";
                rowcolumn.DataSource = zoneDocument.Rows;
                rowcolumn.ValueMember = "RowKey";
                rowcolumn.DisplayMember = "RowName";
                gridData.Columns.Add(rowcolumn);
                gridData.Columns["RowKey"].Visible = false;


                foreach (DataRow row in zoneDocument.Entities.Rows)
                {
                    DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn();
                    column.DataPropertyName = row["EntityKey"].ToString();
                    column.HeaderText = row["EntityName"].ToString();
                    gridData.Columns.Add(column);
                    gridData.Columns[row["EntityKey"].ToString()].Visible = false;
                }
                initialized_zone = true;
            }

            btnSave.Enabled = true;
            tabTarif.Text = string.Format("{0} ({1})", zoneDocument.DocTitle, editable ? "Редактирование" : "Просмотр"); 
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (zoneDocument.IsReadOnly) return;

            try
            {
                zoneDocument.Save();
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Не удалось сохранить документ\n{0}", exception.Message));
                return;
            }
            zoneDocument.EndEdit();

            btnSave.Enabled = false;
            gridData.ReadOnly = zoneDocument.IsReadOnly;
            tabTarif.Text = string.Format("{0} ({1})", zoneDocument.DocTitle, !zoneDocument.IsReadOnly ? "Редактирование" : "Просмотр"); 
        }

        private void ZoneEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            zoneDocument.EndEdit();
            cldrDocument.EndEdit();
        }

        private void btnCldrLoad_Click(object sender, EventArgs e)
        {
            if (!cldrDocument.IsReadOnly)
            {
                if (MessageBox.Show("Документ не сохранён. Продолжить загрузку?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                {
                    cldrDocument.EndEdit();
                }
                else
                {
                    return;
                }
            }

            int Year = 0;
            if (!int.TryParse(txtCldrYear.Text, out Year)) Year = DateTime.Today.Year;
            int Month = cmbCldrMonth.SelectedIndex + 1;

            try
            {
                cldrDocument.Load(sqlConnection, Year, Month);
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Не удалось открыть документ\n{0}", exception.Message));
                return;
            }

            bool editable = cldrDocument.BeginEdit();
            gridCldr.DataSource = cldrDocument.Body;
            gridCldr.ReadOnly = !editable;

            if (!initialized_cldr)
            {
                gridCldr.Columns["Day"].HeaderText = "День";
                gridCldr.Columns["Day"].ReadOnly = true;
                gridCldr.Columns["Type"].HeaderText = "Тип дня";
                initialized_cldr = true;
            }
            //this.Size = new Size(this.Size.Width + gridData.PreferredSize.Width - gridData.Size.Width,
            //                     this.Size.Height + gridData.PreferredSize.Height - gridData.Size.Height);

            //gridData.Size = gridData.PreferredSize;
            btnCldrSave.Enabled = true;
            tabCldr.Text = string.Format("{0} ({1})", cldrDocument.DocTitle, editable ? "Редактирование" : "Просмотр"); 
        
        }

        private void btnCldrSave_Click(object sender, EventArgs e)
        {
            if (cldrDocument.IsReadOnly) return;

            try
            {
                cldrDocument.Save();
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Не удалось сохранить документ\n{0}", exception.Message));
                return;
            }
            cldrDocument.EndEdit();

            btnCldrSave.Enabled = false;
            gridCldr.ReadOnly = cldrDocument.IsReadOnly;
            tabCldr.Text = string.Format("{0} ({1})", cldrDocument.DocTitle, !cldrDocument.IsReadOnly ? "Редактирование" : "Просмотр"); 
        }

        private void gridCldr_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DateTime day = (DateTime)gridCldr.Rows[e.RowIndex].Cells["Day"].Value;
            try
            {
                e.CellStyle.BackColor = (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday) ? Color.LightBlue : Color.White;
            }
            catch
            {

            }
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuConfig_Click(object sender, EventArgs e)
        {
            Config wnd = new Config();
            wnd.Show();
        }
    }
}
