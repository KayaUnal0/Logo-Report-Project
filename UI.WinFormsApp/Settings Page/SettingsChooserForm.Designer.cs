namespace Logo_Project
{
    partial class SettingsChooserForm
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
            btnEmail = new Button();
            btnDatabase = new Button();
            SuspendLayout();
            // 
            // btnEmail
            // 
            btnEmail.Location = new Point(210, 40);
            btnEmail.Name = "btnEmail";
            btnEmail.Size = new Size(150, 60);
            btnEmail.TabIndex = 1;
            btnEmail.Text = "E-Posta Ayarları";
            btnEmail.UseVisualStyleBackColor = true;
            // 
            // btnDatabase
            // 
            btnDatabase.Location = new Point(40, 40);
            btnDatabase.Name = "btnDatabase";
            btnDatabase.Size = new Size(150, 60);
            btnDatabase.TabIndex = 0;
            btnDatabase.Text = "Veritabanı Ayarları";
            btnDatabase.UseVisualStyleBackColor = true;
            // 
            // SettingsChooserForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(402, 153);
            Controls.Add(btnEmail);
            Controls.Add(btnDatabase);
            MaximizeBox = false;
            MaximumSize = new Size(420, 200);
            MinimizeBox = false;
            MinimumSize = new Size(420, 200);
            Name = "SettingsChooserForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Ayarlar";
            Load += SettingsChooserForm_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button btnEmail;
        private Button btnDatabase;
    }
}