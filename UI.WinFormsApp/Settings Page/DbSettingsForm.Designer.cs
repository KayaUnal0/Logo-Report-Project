namespace Logo_Project
{
    partial class DbSettingsForm
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
            lblServer = new Label();
            txtServer = new TextBox();
            lblDatabase = new Label();
            lblUser = new Label();
            lblPassword = new Label();
            txtDatabase = new TextBox();
            txtUser = new TextBox();
            txtPassword = new TextBox();
            lblEncrypt = new Label();
            lblTrust = new Label();
            cbEncrypt = new CheckBox();
            cbTrust = new CheckBox();
            btnTest = new Button();
            btnSave = new Button();
            SuspendLayout();
            // 
            // lblServer
            // 
            lblServer.AutoSize = true;
            lblServer.Location = new Point(20, 20);
            lblServer.Name = "lblServer";
            lblServer.Size = new Size(59, 20);
            lblServer.TabIndex = 0;
            lblServer.Text = "Sunucu:";
            // 
            // txtServer
            // 
            txtServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtServer.Location = new Point(150, 18);
            txtServer.Name = "txtServer";
            txtServer.Size = new Size(320, 27);
            txtServer.TabIndex = 1;
            // 
            // lblDatabase
            // 
            lblDatabase.AutoSize = true;
            lblDatabase.Location = new Point(20, 60);
            lblDatabase.Name = "lblDatabase";
            lblDatabase.Size = new Size(79, 20);
            lblDatabase.TabIndex = 2;
            lblDatabase.Text = "Veritabanı:";
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Location = new Point(20, 100);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(68, 20);
            lblUser.TabIndex = 3;
            lblUser.Text = "Kullanıcı:";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(20, 140);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(42, 20);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Şifre:";
            // 
            // txtDatabase
            // 
            txtDatabase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDatabase.Location = new Point(150, 58);
            txtDatabase.Name = "txtDatabase";
            txtDatabase.Size = new Size(320, 27);
            txtDatabase.TabIndex = 5;
            // 
            // txtUser
            // 
            txtUser.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUser.Location = new Point(150, 98);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(320, 27);
            txtUser.TabIndex = 6;
            // 
            // txtPassword
            // 
            txtPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPassword.Location = new Point(150, 138);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(320, 27);
            txtPassword.TabIndex = 7;
            // 
            // lblEncrypt
            // 
            lblEncrypt.AutoSize = true;
            lblEncrypt.Location = new Point(20, 183);
            lblEncrypt.Name = "lblEncrypt";
            lblEncrypt.Size = new Size(61, 20);
            lblEncrypt.TabIndex = 8;
            lblEncrypt.Text = "Encrypt:";
            // 
            // lblTrust
            // 
            lblTrust.AutoSize = true;
            lblTrust.Location = new Point(20, 220);
            lblTrust.Name = "lblTrust";
            lblTrust.Size = new Size(152, 20);
            lblTrust.TabIndex = 9;
            lblTrust.Text = "TrustServerCertificate:";
            // 
            // cbEncrypt
            // 
            cbEncrypt.AutoSize = true;
            cbEncrypt.Location = new Point(191, 186);
            cbEncrypt.Name = "cbEncrypt";
            cbEncrypt.Size = new Size(18, 17);
            cbEncrypt.TabIndex = 10;
            cbEncrypt.UseVisualStyleBackColor = true;
            // 
            // cbTrust
            // 
            cbTrust.AutoSize = true;
            cbTrust.Location = new Point(191, 223);
            cbTrust.Name = "cbTrust";
            cbTrust.Size = new Size(18, 17);
            cbTrust.TabIndex = 11;
            cbTrust.UseVisualStyleBackColor = true;
            cbTrust.CheckedChanged += cbTrust_CheckedChanged;
            // 
            // btnTest
            // 
            btnTest.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnTest.Location = new Point(230, 269);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(140, 30);
            btnTest.TabIndex = 12;
            btnTest.Text = "Bağlantıyı Test Et";
            btnTest.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(376, 269);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(94, 29);
            btnSave.TabIndex = 13;
            btnSave.Text = "Kaydet";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // DbSettingsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(502, 319);
            Controls.Add(btnSave);
            Controls.Add(btnTest);
            Controls.Add(cbTrust);
            Controls.Add(cbEncrypt);
            Controls.Add(lblTrust);
            Controls.Add(lblEncrypt);
            Controls.Add(txtPassword);
            Controls.Add(txtUser);
            Controls.Add(txtDatabase);
            Controls.Add(lblPassword);
            Controls.Add(lblUser);
            Controls.Add(lblDatabase);
            Controls.Add(txtServer);
            Controls.Add(lblServer);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(520, 366);
            Name = "DbSettingsForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Veritabanı Ayarları";
            Load += DbSettingsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblServer;
        private TextBox txtServer;
        private Label lblDatabase;
        private Label lblUser;
        private Label lblPassword;
        private TextBox txtDatabase;
        private TextBox txtUser;
        private TextBox txtPassword;
        private Label lblEncrypt;
        private Label lblTrust;
        private CheckBox cbEncrypt;
        private CheckBox cbTrust;
        private Button btnTest;
        private Button btnSave;
    }
}