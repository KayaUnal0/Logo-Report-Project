using System;
using System.Windows.Forms;

namespace Logo_Project
{
    public partial class SettingsChooserForm : Form
    {
        // Lets ReportListUI know what was saved (if any)
        public bool DatabaseSaved { get; private set; }
        public bool EmailSaved { get; private set; }

        public SettingsChooserForm()
        {
            InitializeComponent();

            btnDatabase.Click += (_, __) =>
            {
                using var dlg = new DbSettingsForm();
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    DatabaseSaved = true;
                }
            };

            btnEmail.Click += (_, __) =>
            {
                using var dlg = new EmailSettingsForm();
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    EmailSaved = true;
                }
            };

        }

        private void SettingsChooserForm_Load(object? sender, EventArgs e)
        {

        }

    }
}
