namespace Logo_Project.Report_Planner_Page
{
    partial class SqlEditorForm
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
            txtSqlEditor = new RichTextBox();
            btnSave = new Button();
            SuspendLayout();
            // 
            // txtSqlEditor
            // 
            txtSqlEditor.AcceptsTab = true;
            txtSqlEditor.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtSqlEditor.Location = new Point(12, 12);
            txtSqlEditor.Name = "txtSqlEditor";
            txtSqlEditor.Size = new Size(558, 642);
            txtSqlEditor.TabIndex = 0;
            txtSqlEditor.Text = "";
            txtSqlEditor.WordWrap = false;
            txtSqlEditor.TextChanged += txtSqlEditor_TextChanged;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.DialogResult = DialogResult.OK;
            btnSave.Location = new Point(457, 660);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(113, 31);
            btnSave.TabIndex = 1;
            btnSave.Text = "Kaydet";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // SqlEditorForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 703);
            Controls.Add(btnSave);
            Controls.Add(txtSqlEditor);
            KeyPreview = true;
            MinimumSize = new Size(600, 750);
            Name = "SqlEditorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "SQL Düzenleyici";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox txtSqlEditor;
        private Button btnSave;
    }
}