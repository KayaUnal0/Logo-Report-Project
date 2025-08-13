namespace UI.WinFormsApp
{
    partial class ReportPlannerUI
    {
        /// <summary>
        /// Required method for Designer support.
        /// </summary>
        private void InitializeComponent()
        {
            btnOnayla = new Button();
            lblReportTitle = new Label();
            txtReportTitle = new TextBox();
            lblSql = new Label();
            rtbSqlQuery = new RichTextBox();
            lblEmail = new Label();
            txtEmail = new TextBox();
            lblDirectory = new Label();
            txtDirectory = new TextBox();
            btnBrowse = new Button();
            lblFile = new Label();
            cmbFileType = new ComboBox();
            lblPeriod = new Label();
            cmbPeriod = new ComboBox();
            cbPazartesi = new CheckBox();
            cbSalı = new CheckBox();
            cbÇarşamba = new CheckBox();
            cbPerşembe = new CheckBox();
            cbCuma = new CheckBox();
            cbCumartesi = new CheckBox();
            cbPazar = new CheckBox();
            lblDay = new Label();
            dtpDate = new DateTimePicker();
            lblTime = new Label();
            dtpTime = new DateTimePicker();
            SuspendLayout();
            // 
            // btnOnayla
            // 
            btnOnayla.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOnayla.Location = new Point(330, 578);
            btnOnayla.Name = "btnOnayla";
            btnOnayla.Size = new Size(150, 40);
            btnOnayla.TabIndex = 0;
            btnOnayla.Text = "Onayla";
            btnOnayla.UseVisualStyleBackColor = true;
            // 
            // lblReportTitle
            // 
            lblReportTitle.AutoSize = true;
            lblReportTitle.Location = new Point(30, 20);
            lblReportTitle.Name = "lblReportTitle";
            lblReportTitle.Size = new Size(97, 20);
            lblReportTitle.TabIndex = 1;
            lblReportTitle.Text = "Rapor Başlığı";
            // 
            // txtReportTitle
            // 
            txtReportTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtReportTitle.Location = new Point(180, 20);
            txtReportTitle.Name = "txtReportTitle";
            txtReportTitle.Size = new Size(300, 27);
            txtReportTitle.TabIndex = 2;
            // 
            // lblSql
            // 
            lblSql.AutoSize = true;
            lblSql.Location = new Point(30, 55);
            lblSql.Name = "lblSql";
            lblSql.Size = new Size(106, 20);
            lblSql.TabIndex = 3;
            lblSql.Text = "Rapor Sorgusu";
            // 
            // rtbSqlQuery
            // 
            rtbSqlQuery.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            rtbSqlQuery.Location = new Point(180, 55);
            rtbSqlQuery.Name = "rtbSqlQuery";
            rtbSqlQuery.Size = new Size(300, 100);
            rtbSqlQuery.TabIndex = 4;
            rtbSqlQuery.Text = "";
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(30, 165);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(58, 20);
            lblEmail.TabIndex = 5;
            lblEmail.Text = "E-Posta";
            // 
            // txtEmail
            // 
            txtEmail.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEmail.Location = new Point(180, 165);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(300, 27);
            txtEmail.TabIndex = 6;
            txtEmail.Text = "xxx@logo.com.tr";
            // 
            // lblDirectory
            // 
            lblDirectory.AutoSize = true;
            lblDirectory.Location = new Point(30, 200);
            lblDirectory.Name = "lblDirectory";
            lblDirectory.Size = new Size(43, 20);
            lblDirectory.TabIndex = 7;
            lblDirectory.Text = "Dizin";
            // 
            // txtDirectory
            // 
            txtDirectory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDirectory.Location = new Point(180, 200);
            txtDirectory.Name = "txtDirectory";
            txtDirectory.PlaceholderText = "Zorunlu alan";
            txtDirectory.Size = new Size(220, 27);
            txtDirectory.TabIndex = 8;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(410, 200);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(70, 25);
            btnBrowse.TabIndex = 9;
            btnBrowse.Text = "Gözat...";
            btnBrowse.UseVisualStyleBackColor = true;
            // 
            // lblFile
            // 
            lblFile.AutoSize = true;
            lblFile.Location = new Point(30, 235);
            lblFile.Name = "lblFile";
            lblFile.Size = new Size(80, 20);
            lblFile.TabIndex = 10;
            lblFile.Text = "Dosya türü";
            // 
            // cmbFileType
            // 
            cmbFileType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbFileType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFileType.FormattingEnabled = true;
            cmbFileType.Items.AddRange(new object[] { "Excel", "HTML", "Excel, HTML" });
            cmbFileType.Location = new Point(180, 235);
            cmbFileType.Name = "cmbFileType";
            cmbFileType.Size = new Size(300, 28);
            cmbFileType.TabIndex = 11;
            // 
            // lblPeriod
            // 
            lblPeriod.AutoSize = true;
            lblPeriod.Location = new Point(30, 270);
            lblPeriod.Name = "lblPeriod";
            lblPeriod.Size = new Size(51, 20);
            lblPeriod.TabIndex = 12;
            lblPeriod.Text = "Period";
            // 
            // cmbPeriod
            // 
            cmbPeriod.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbPeriod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPeriod.ForeColor = SystemColors.WindowText;
            cmbPeriod.FormattingEnabled = true;
            cmbPeriod.Location = new Point(180, 270);
            cmbPeriod.Name = "cmbPeriod";
            cmbPeriod.Size = new Size(300, 28);
            cmbPeriod.TabIndex = 13;
            // 
            // cbPazartesi
            // 
            cbPazartesi.AutoSize = true;
            cbPazartesi.Location = new Point(180, 305);
            cbPazartesi.Name = "cbPazartesi";
            cbPazartesi.Size = new Size(89, 24);
            cbPazartesi.TabIndex = 14;
            cbPazartesi.Text = "Pazartesi";
            cbPazartesi.UseVisualStyleBackColor = true;
            // 
            // cbSalı
            // 
            cbSalı.AutoSize = true;
            cbSalı.Location = new Point(180, 330);
            cbSalı.Name = "cbSalı";
            cbSalı.Size = new Size(55, 24);
            cbSalı.TabIndex = 15;
            cbSalı.Text = "Salı";
            cbSalı.UseVisualStyleBackColor = true;
            // 
            // cbÇarşamba
            // 
            cbÇarşamba.AutoSize = true;
            cbÇarşamba.Location = new Point(180, 355);
            cbÇarşamba.Name = "cbÇarşamba";
            cbÇarşamba.Size = new Size(97, 24);
            cbÇarşamba.TabIndex = 16;
            cbÇarşamba.Text = "Çarşamba";
            cbÇarşamba.UseVisualStyleBackColor = true;
            // 
            // cbPerşembe
            // 
            cbPerşembe.AutoSize = true;
            cbPerşembe.Location = new Point(180, 380);
            cbPerşembe.Name = "cbPerşembe";
            cbPerşembe.Size = new Size(95, 24);
            cbPerşembe.TabIndex = 17;
            cbPerşembe.Text = "Perşembe";
            cbPerşembe.UseVisualStyleBackColor = true;
            // 
            // cbCuma
            // 
            cbCuma.AutoSize = true;
            cbCuma.Location = new Point(180, 405);
            cbCuma.Name = "cbCuma";
            cbCuma.Size = new Size(69, 24);
            cbCuma.TabIndex = 18;
            cbCuma.Text = "Cuma";
            cbCuma.UseVisualStyleBackColor = true;
            // 
            // cbCumartesi
            // 
            cbCumartesi.AutoSize = true;
            cbCumartesi.Location = new Point(180, 430);
            cbCumartesi.Name = "cbCumartesi";
            cbCumartesi.Size = new Size(97, 24);
            cbCumartesi.TabIndex = 19;
            cbCumartesi.Text = "Cumartesi";
            cbCumartesi.UseVisualStyleBackColor = true;
            // 
            // cbPazar
            // 
            cbPazar.AutoSize = true;
            cbPazar.Location = new Point(180, 455);
            cbPazar.Name = "cbPazar";
            cbPazar.Size = new Size(66, 24);
            cbPazar.TabIndex = 20;
            cbPazar.Text = "Pazar";
            cbPazar.UseVisualStyleBackColor = true;
            // 
            // lblDay
            // 
            lblDay.AutoSize = true;
            lblDay.Location = new Point(30, 510);
            lblDay.Name = "lblDay";
            lblDay.Size = new Size(35, 20);
            lblDay.TabIndex = 21;
            lblDay.Text = "Gün";
            // 
            // dtpDate
            // 
            dtpDate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Location = new Point(180, 510);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(200, 27);
            dtpDate.TabIndex = 22;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Location = new Point(30, 545);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(38, 20);
            lblTime.TabIndex = 23;
            lblTime.Text = "Saat";
            // 
            // dtpTime
            // 
            dtpTime.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dtpTime.Format = DateTimePickerFormat.Time;
            dtpTime.Location = new Point(180, 545);
            dtpTime.Name = "dtpTime";
            dtpTime.ShowUpDown = true;
            dtpTime.Size = new Size(200, 27);
            dtpTime.TabIndex = 24;
            // 
            // ReportPlannerUI
            // 
            ClientSize = new Size(582, 703);
            Controls.Add(dtpTime);
            Controls.Add(lblTime);
            Controls.Add(dtpDate);
            Controls.Add(lblDay);
            Controls.Add(cbPazar);
            Controls.Add(cbCumartesi);
            Controls.Add(cbCuma);
            Controls.Add(cbPerşembe);
            Controls.Add(cbÇarşamba);
            Controls.Add(cbSalı);
            Controls.Add(cbPazartesi);
            Controls.Add(cmbPeriod);
            Controls.Add(lblPeriod);
            Controls.Add(cmbFileType);
            Controls.Add(lblFile);
            Controls.Add(btnBrowse);
            Controls.Add(txtDirectory);
            Controls.Add(lblDirectory);
            Controls.Add(txtEmail);
            Controls.Add(lblEmail);
            Controls.Add(rtbSqlQuery);
            Controls.Add(lblSql);
            Controls.Add(txtReportTitle);
            Controls.Add(lblReportTitle);
            Controls.Add(btnOnayla);
            MinimumSize = new Size(600, 750);
            Name = "ReportPlannerUI";
            Text = "Rapor Planlayıcı";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }
        private TextBox txtReportTitle;
        private Button btnOnayla;
        private Label lblReportTitle;
        private Label lblSql;
        private RichTextBox rtbSqlQuery;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblDirectory;
        private TextBox txtDirectory;
        private Button btnBrowse;
        private Label lblFile;
        private ComboBox cmbFileType;
        private Label lblPeriod;
        private ComboBox cmbPeriod;
        private CheckBox cbPazartesi;
        private CheckBox cbSalı;
        private CheckBox cbÇarşamba;
        private CheckBox cbPerşembe;
        private CheckBox cbCuma;
        private CheckBox cbCumartesi;
        private CheckBox cbPazar;
        private Label lblDay;
        private DateTimePicker dtpDate;
        private Label lblTime;
        private DateTimePicker dtpTime;
    }
}
