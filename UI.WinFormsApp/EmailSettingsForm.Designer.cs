namespace Logo_Project
{
    partial class EmailSettingsForm
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
            lblSenderEmail = new Label();
            txtSenderEmail = new TextBox();
            lblSenderPassword = new Label();
            txtSenderPassword = new TextBox();
            lblSmtpServer = new Label();
            lblSmtpPort = new Label();
            btnEmailSave = new Button();
            numSmtpPort = new NumericUpDown();
            txtSmtpServer = new TextBox();
            ((System.ComponentModel.ISupportInitialize)numSmtpPort).BeginInit();
            SuspendLayout();
            // 
            // lblSenderEmail
            // 
            lblSenderEmail.AutoSize = true;
            lblSenderEmail.Location = new Point(20, 20);
            lblSenderEmail.Name = "lblSenderEmail";
            lblSenderEmail.Size = new Size(63, 20);
            lblSenderEmail.TabIndex = 0;
            lblSenderEmail.Text = "E-posta:";
            // 
            // txtSenderEmail
            // 
            txtSenderEmail.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSenderEmail.Location = new Point(119, 18);
            txtSenderEmail.Name = "txtSenderEmail";
            txtSenderEmail.Size = new Size(361, 27);
            txtSenderEmail.TabIndex = 0;
            // 
            // lblSenderPassword
            // 
            lblSenderPassword.AutoSize = true;
            lblSenderPassword.Location = new Point(20, 60);
            lblSenderPassword.Name = "lblSenderPassword";
            lblSenderPassword.Size = new Size(42, 20);
            lblSenderPassword.TabIndex = 1;
            lblSenderPassword.Text = "Şifre:";
            // 
            // txtSenderPassword
            // 
            txtSenderPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSenderPassword.Location = new Point(119, 60);
            txtSenderPassword.Name = "txtSenderPassword";
            txtSenderPassword.Size = new Size(361, 27);
            txtSenderPassword.TabIndex = 2;
            // 
            // lblSmtpServer
            // 
            lblSmtpServer.AutoSize = true;
            lblSmtpServer.Location = new Point(20, 100);
            lblSmtpServer.Name = "lblSmtpServer";
            lblSmtpServer.Size = new Size(49, 20);
            lblSmtpServer.TabIndex = 3;
            lblSmtpServer.Text = "SMTP:";
            // 
            // lblSmtpPort
            // 
            lblSmtpPort.AutoSize = true;
            lblSmtpPort.Location = new Point(20, 140);
            lblSmtpPort.Name = "lblSmtpPort";
            lblSmtpPort.Size = new Size(79, 20);
            lblSmtpPort.TabIndex = 4;
            lblSmtpPort.Text = "SMTP Port:";
            // 
            // btnEmailSave
            // 
            btnEmailSave.Location = new Point(405, 210);
            btnEmailSave.Name = "btnEmailSave";
            btnEmailSave.Size = new Size(75, 30);
            btnEmailSave.TabIndex = 6;
            btnEmailSave.Text = "Kaydet";
            btnEmailSave.UseVisualStyleBackColor = true;
            // 
            // numSmtpPort
            // 
            numSmtpPort.Location = new Point(180, 138);
            numSmtpPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numSmtpPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSmtpPort.Name = "numSmtpPort";
            numSmtpPort.Size = new Size(100, 27);
            numSmtpPort.TabIndex = 3;
            numSmtpPort.Value = new decimal(new int[] { 587, 0, 0, 0 });
            numSmtpPort.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // txtSmtpServer
            // 
            txtSmtpServer.Location = new Point(180, 98);
            txtSmtpServer.Name = "txtSmtpServer";
            txtSmtpServer.Size = new Size(300, 27);
            txtSmtpServer.TabIndex = 7;
            // 
            // EmailSettingsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(502, 253);
            Controls.Add(txtSmtpServer);
            Controls.Add(numSmtpPort);
            Controls.Add(btnEmailSave);
            Controls.Add(lblSmtpPort);
            Controls.Add(lblSmtpServer);
            Controls.Add(txtSenderPassword);
            Controls.Add(lblSenderPassword);
            Controls.Add(txtSenderEmail);
            Controls.Add(lblSenderEmail);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(520, 300);
            Name = "EmailSettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "E-posta Ayarları";
            Load += EmailSettingsForm_Load;
            ((System.ComponentModel.ISupportInitialize)numSmtpPort).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblSenderEmail;
        private TextBox txtSenderEmail;
        private Label lblSenderPassword;
        private TextBox txtSenderPassword;
        private Label lblSmtpServer;
        private Label lblSmtpPort;
        private Button btnEmailTest;
        private Button btnEmailSave;
        private NumericUpDown numSmtpPort;
        private TextBox txtSmtpServer;
    }
}