namespace ZoneEditor
{
    partial class ZoneEditor
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZoneEditor));
            this.gridData = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbMonth = new System.Windows.Forms.ComboBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtYear = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabTarif = new System.Windows.Forms.TabPage();
            this.tabCldr = new System.Windows.Forms.TabPage();
            this.btnCldrSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCldrYear = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCldrLoad = new System.Windows.Forms.Button();
            this.cmbCldrMonth = new System.Windows.Forms.ComboBox();
            this.gridCldr = new System.Windows.Forms.DataGridView();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.gridData)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabTarif.SuspendLayout();
            this.tabCldr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCldr)).BeginInit();
            this.TrayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridData
            // 
            this.gridData.AllowUserToAddRows = false;
            this.gridData.AllowUserToDeleteRows = false;
            this.gridData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridData.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridData.Location = new System.Drawing.Point(0, 32);
            this.gridData.Name = "gridData";
            this.gridData.Size = new System.Drawing.Size(659, 714);
            this.gridData.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Год:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Месяц:";
            // 
            // cmbMonth
            // 
            this.cmbMonth.FormattingEnabled = true;
            this.cmbMonth.Items.AddRange(new object[] {
            "январь",
            "февраль",
            "март",
            "апрель",
            "май",
            "июнь",
            "июль",
            "август",
            "сентябрь",
            "октябрь",
            "ноябрь",
            "декабрь"});
            this.cmbMonth.Location = new System.Drawing.Point(196, 6);
            this.cmbMonth.Name = "cmbMonth";
            this.cmbMonth.Size = new System.Drawing.Size(134, 21);
            this.cmbMonth.TabIndex = 4;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(336, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(88, 23);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "Загрузить";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtYear
            // 
            this.txtYear.Location = new System.Drawing.Point(41, 6);
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(100, 20);
            this.txtYear.TabIndex = 6;
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(430, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabTarif);
            this.tabControl.Controls.Add(this.tabCldr);
            this.tabControl.Location = new System.Drawing.Point(1, 2);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(667, 772);
            this.tabControl.TabIndex = 8;
            // 
            // tabTarif
            // 
            this.tabTarif.Controls.Add(this.gridData);
            this.tabTarif.Controls.Add(this.btnSave);
            this.tabTarif.Controls.Add(this.label1);
            this.tabTarif.Controls.Add(this.txtYear);
            this.tabTarif.Controls.Add(this.label2);
            this.tabTarif.Controls.Add(this.btnLoad);
            this.tabTarif.Controls.Add(this.cmbMonth);
            this.tabTarif.Location = new System.Drawing.Point(4, 22);
            this.tabTarif.Name = "tabTarif";
            this.tabTarif.Padding = new System.Windows.Forms.Padding(3);
            this.tabTarif.Size = new System.Drawing.Size(659, 746);
            this.tabTarif.TabIndex = 0;
            this.tabTarif.Text = "Тарифные зоны";
            this.tabTarif.UseVisualStyleBackColor = true;
            // 
            // tabCldr
            // 
            this.tabCldr.Controls.Add(this.btnCldrSave);
            this.tabCldr.Controls.Add(this.label3);
            this.tabCldr.Controls.Add(this.txtCldrYear);
            this.tabCldr.Controls.Add(this.label4);
            this.tabCldr.Controls.Add(this.btnCldrLoad);
            this.tabCldr.Controls.Add(this.cmbCldrMonth);
            this.tabCldr.Controls.Add(this.gridCldr);
            this.tabCldr.Location = new System.Drawing.Point(4, 22);
            this.tabCldr.Name = "tabCldr";
            this.tabCldr.Padding = new System.Windows.Forms.Padding(3);
            this.tabCldr.Size = new System.Drawing.Size(659, 746);
            this.tabCldr.TabIndex = 1;
            this.tabCldr.Text = "Календарь";
            this.tabCldr.UseVisualStyleBackColor = true;
            // 
            // btnCldrSave
            // 
            this.btnCldrSave.Enabled = false;
            this.btnCldrSave.Location = new System.Drawing.Point(430, 4);
            this.btnCldrSave.Name = "btnCldrSave";
            this.btnCldrSave.Size = new System.Drawing.Size(82, 23);
            this.btnCldrSave.TabIndex = 13;
            this.btnCldrSave.Text = "Сохранить";
            this.btnCldrSave.UseVisualStyleBackColor = true;
            this.btnCldrSave.Click += new System.EventHandler(this.btnCldrSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Год:";
            // 
            // txtCldrYear
            // 
            this.txtCldrYear.Location = new System.Drawing.Point(41, 6);
            this.txtCldrYear.Name = "txtCldrYear";
            this.txtCldrYear.Size = new System.Drawing.Size(100, 20);
            this.txtCldrYear.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(147, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Месяц:";
            // 
            // btnCldrLoad
            // 
            this.btnCldrLoad.Location = new System.Drawing.Point(336, 4);
            this.btnCldrLoad.Name = "btnCldrLoad";
            this.btnCldrLoad.Size = new System.Drawing.Size(88, 23);
            this.btnCldrLoad.TabIndex = 11;
            this.btnCldrLoad.Text = "Загрузить";
            this.btnCldrLoad.UseVisualStyleBackColor = true;
            this.btnCldrLoad.Click += new System.EventHandler(this.btnCldrLoad_Click);
            // 
            // cmbCldrMonth
            // 
            this.cmbCldrMonth.FormattingEnabled = true;
            this.cmbCldrMonth.Items.AddRange(new object[] {
            "январь",
            "февраль",
            "март",
            "апрель",
            "май",
            "июнь",
            "июль",
            "август",
            "сентябрь",
            "октябрь",
            "ноябрь",
            "декабрь"});
            this.cmbCldrMonth.Location = new System.Drawing.Point(196, 6);
            this.cmbCldrMonth.Name = "cmbCldrMonth";
            this.cmbCldrMonth.Size = new System.Drawing.Size(134, 21);
            this.cmbCldrMonth.TabIndex = 10;
            // 
            // gridCldr
            // 
            this.gridCldr.AllowUserToAddRows = false;
            this.gridCldr.AllowUserToDeleteRows = false;
            this.gridCldr.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCldr.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridCldr.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCldr.Location = new System.Drawing.Point(0, 32);
            this.gridCldr.Name = "gridCldr";
            this.gridCldr.Size = new System.Drawing.Size(659, 714);
            this.gridCldr.TabIndex = 0;
            this.gridCldr.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridCldr_CellFormatting);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.TrayMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Редактор зон";
            this.notifyIcon.Visible = true;
            // 
            // TrayMenu
            // 
            this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuConfig,
            this.menuExit});
            this.TrayMenu.Name = "TrayMenu";
            this.TrayMenu.Size = new System.Drawing.Size(153, 70);
            // 
            // menuConfig
            // 
            this.menuConfig.Name = "menuConfig";
            this.menuConfig.Size = new System.Drawing.Size(152, 22);
            this.menuConfig.Text = "Настройки...";
            this.menuConfig.Click += new System.EventHandler(this.menuConfig_Click);
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(152, 22);
            this.menuExit.Text = "Выход";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // ZoneEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 773);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ZoneEditor";
            this.Text = "Редактор Зон";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ZoneEditor_FormClosed);
            this.Load += new System.EventHandler(this.ZoneEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridData)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabTarif.ResumeLayout(false);
            this.tabTarif.PerformLayout();
            this.tabCldr.ResumeLayout(false);
            this.tabCldr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCldr)).EndInit();
            this.TrayMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbMonth;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox txtYear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabTarif;
        private System.Windows.Forms.TabPage tabCldr;
        private System.Windows.Forms.DataGridView gridCldr;
        private System.Windows.Forms.Button btnCldrSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCldrYear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCldrLoad;
        private System.Windows.Forms.ComboBox cmbCldrMonth;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip TrayMenu;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuConfig;
    }
}

