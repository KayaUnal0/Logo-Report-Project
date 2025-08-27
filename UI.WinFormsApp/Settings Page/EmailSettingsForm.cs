using System;
using System.Windows.Forms;
using Common.Shared.Dtos;
using Infrastructure.Logic.Config;
using System.Net;
using System.Net.Mail;

namespace Logo_Project
{
    public partial class EmailSettingsForm : Form
    {
        public EmailSettingsForm()
        {
            InitializeComponent();

            Load += EmailSettingsForm_Load;
            btnEmailSave.Click += (_, __) => SaveEmail();

            txtSenderPassword.UseSystemPasswordChar = true;
        }

        private void EmailSettingsForm_Load(object? sender, EventArgs e)
        {
            var (_, s) = SettingsManager.LoadEmail();
            txtSenderEmail.Text = s.SenderEmail ?? "";
            txtSenderPassword.Text = s.SenderPassword ?? "";
            txtSmtpServer.Text = s.SmtpServer ?? "";
            numSmtpPort.Value = s.SmtpPort > 0 ? s.SmtpPort : 587;
        }

        private void SaveEmail()
        {
            var s = new EmailSettings
            {
                SenderEmail = txtSenderEmail.Text?.Trim(),
                SenderPassword = txtSenderPassword.Text,
                SmtpServer = txtSmtpServer.Text?.Trim(),
                SmtpPort = (int)numSmtpPort.Value
            };

            try
            {
                SettingsManager.SaveEmail(s);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ayarlar kaydedilemedi:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
