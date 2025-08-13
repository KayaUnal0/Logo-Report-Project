namespace Logo_Project
{
    partial class ReportListUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridViewReports = new DataGridView();
            colSubject = new DataGridViewTextBoxColumn();
            colPeriod = new DataGridViewTextBoxColumn();
            colEmail = new DataGridViewTextBoxColumn();
            colDirectory = new DataGridViewTextBoxColumn();
            colActive = new DataGridViewCheckBoxColumn();
            btnNew = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewReports).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewReports
            // 
            dataGridViewReports.AllowUserToAddRows = false;
            dataGridViewReports.AutoGenerateColumns = false;
            dataGridViewReports.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewReports.ColumnHeadersHeight = 29;
            dataGridViewReports.Columns.AddRange(new DataGridViewColumn[] { colSubject, colPeriod, colEmail, colDirectory, colActive });
            dataGridViewReports.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridViewReports.Location = new Point(20, 90);
            dataGridViewReports.MultiSelect = false;
            dataGridViewReports.Name = "dataGridViewReports";
            dataGridViewReports.RowHeadersWidth = 51;
            dataGridViewReports.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewReports.Size = new Size(740, 340);
            dataGridViewReports.TabIndex = 4;
            // 
            // colSubject
            // 
            colSubject.DataPropertyName = "Subject";
            colSubject.HeaderText = "Başlık";
            colSubject.MinimumWidth = 6;
            colSubject.Name = "colSubject";
            colSubject.ReadOnly = true;
            // 
            // colPeriod
            // 
            colPeriod.DataPropertyName = "Period";
            colPeriod.HeaderText = "Period";
            colPeriod.MinimumWidth = 6;
            colPeriod.Name = "colPeriod";
            colPeriod.ReadOnly = true;
            // 
            // colEmail
            // 
            colEmail.DataPropertyName = "Email";
            colEmail.HeaderText = "E-Posta";
            colEmail.MinimumWidth = 6;
            colEmail.Name = "colEmail";
            colEmail.ReadOnly = true;
            // 
            // colDirectory
            // 
            colDirectory.DataPropertyName = "Directory";
            colDirectory.HeaderText = "Dizin";
            colDirectory.MinimumWidth = 6;
            colDirectory.Name = "colDirectory";
            colDirectory.ReadOnly = true;
            // 
            // colActive
            // 
            colActive.DataPropertyName = "Active";
            colActive.HeaderText = "Aktif";
            colActive.MinimumWidth = 6;
            colActive.Name = "colActive";
            colActive.Resizable = DataGridViewTriState.True;
            colActive.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // btnNew
            // 
            btnNew.Location = new Point(20, 20);
            btnNew.Name = "btnNew";
            btnNew.Size = new Size(180, 40);
            btnNew.TabIndex = 1;
            btnNew.Text = "Yeni Rapor Oluştur";
            btnNew.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            btnEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnEdit.Location = new Point(20, 480);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(140, 35);
            btnEdit.TabIndex = 2;
            btnEdit.Text = "Düzenle";
            btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDelete.Location = new Point(180, 480);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(140, 35);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Sil";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // ReportListUI
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(782, 573);
            Controls.Add(btnDelete);
            Controls.Add(btnEdit);
            Controls.Add(btnNew);
            Controls.Add(dataGridViewReports);
            MinimumSize = new Size(500, 620);
            Name = "ReportListUI";
            Text = "Ana Ekran";
            Load += HomeScreen_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewReports).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridViewReports;
        private Button btnNew;
        private Button btnEdit;
        private Button btnDelete;
        private DataGridViewTextBoxColumn colSubject;
        private DataGridViewTextBoxColumn colPeriod;
        private DataGridViewTextBoxColumn colEmail;
        private DataGridViewTextBoxColumn colDirectory;
        private DataGridViewCheckBoxColumn colActive;
    }
}