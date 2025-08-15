using System;
using System.Windows.Forms;
using Common.Shared.Dtos;
using Infrastructure.Logic.Config;
// If your project uses Microsoft.Data.SqlClient, use this:
using Microsoft.Data.SqlClient;
// If you prefer System.Data.SqlClient instead, swap the using above.

namespace Logo_Project
{
    public partial class DbSettingsForm : Form
    {
        public DbSettingsForm()
        {
            InitializeComponent();

            // Wire events (Designer stays untouched)
            Load += DbSettingsForm_Load;
            btnTest.Click += (_, __) => TestConnection();
            btnSave.Click += (_, __) => SaveValues();

            // Optional quality-of-life
            txtPassword.UseSystemPasswordChar = true;
        }

        private void DbSettingsForm_Load(object? sender, EventArgs e)
        {
            LoadValues();
        }

        private void LoadValues()
        {
            var (_, s) = SettingsManager.LoadQueryDb();
            txtServer.Text = s.Server ?? "";
            txtDatabase.Text = s.Database ?? "";
            txtUser.Text = s.UserId ?? "";
            txtPassword.Text = s.Password ?? "";
            cbEncrypt.Checked = s.Encrypt;
            cbTrust.Checked = s.TrustServerCertificate;
        }

        private static string BuildConn(DatabaseSettings s)
        {
            return $"Server={s.Server};Database={s.Database};User Id={s.UserId};Password={s.Password};" +
                   $"Encrypt={(s.Encrypt ? "True" : "False")};" +
                   $"TrustServerCertificate={(s.TrustServerCertificate ? "True" : "False")};";
        }

        private void TestConnection()
        {
            var s = GatherFromForm();

            try
            {
                using var conn = new SqlConnection(BuildConn(s));
                conn.Open();
                MessageBox.Show("Bağlantı başarılı.", "Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı başarısız:\n" + ex.Message, "Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveValues()
        {
            var s = GatherFromForm();

            try
            {
                SettingsManager.SaveQueryDb(s);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ayarlar kaydedilemedi:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DatabaseSettings GatherFromForm()
        {
            return new DatabaseSettings
            {
                Server = txtServer.Text?.Trim(),
                Database = txtDatabase.Text?.Trim(),
                UserId = txtUser.Text?.Trim(),
                Password = txtPassword.Text, // keep as-is
                Encrypt = cbEncrypt.Checked,
                TrustServerCertificate = cbTrust.Checked
            };
        }

        private void cbTrust_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
