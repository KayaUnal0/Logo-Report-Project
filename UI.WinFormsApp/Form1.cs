using Common.Shared.Dtos;
using Common.Shared.Enums;
using Core.Interfaces;
using Hangfire;
using Infrastructure.Logic.Filesystem;
using Infrastructure.Logic.Jobs;
using Infrastructure.Logic.Templates;
using Serilog;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI.WinFormsApp
{
    public partial class Form1 : Form
    {
        private readonly ISqlQueryRunner _sqlQueryRunner;
        private readonly IHangfireManager _hangfireManager;
        private readonly EmailJob _emailJob;
        private readonly IFileSaver _fileSaver;
        private readonly TemplateRenderer _templateRenderer;

        public Form1(IEmailSender emailSender, ISqlQueryRunner sqlQueryRunner, IHangfireManager hangfireManager, IFileSaver fileSaver, EmailJob emailJob, TemplateRenderer templateRenderer)
        {
            _sqlQueryRunner = sqlQueryRunner;
            _hangfireManager = hangfireManager;
            _emailJob = emailJob;
            _fileSaver = fileSaver;
            _templateRenderer = templateRenderer;

            InitializeComponent();
            SetupFormLayout();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Log.Information("Form loaded");
        }
        private async void BtnOnayla_Click(object sender, EventArgs e)
        {
            try
            {
                Log.Information("Onayla button clicked");

                var report = new ReportDto
                {
                    Email = (Controls["txtEmail"] as TextBox)?.Text ?? "",
                    Subject = (Controls["txtReportTitle"] as TextBox)?.Text ?? "",
                    Query = (Controls["rtbSqlQuery"] as RichTextBox)?.Text ?? ""
                };

                var result = _sqlQueryRunner.ExecuteQuery(report.Query);

                var directory = (Controls["txtDirectory"] as TextBox)?.Text ?? "";
                var fileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var filePath = _fileSaver.SaveReportToFile(directory, fileName, result.Results);

                var templatePath = "Templates/EmailTemplate.sbn";
                var body = await _templateRenderer.RenderTemplateAsync(templatePath, new
                {
                    subject = report.Subject,
                    status = result.Status.ToString(),
                    results = string.Join("\n", result.Results),
                    filePath
                });

                BackgroundJob.Enqueue(() => EmailJobWrapper.SendEmail(report.Email, report.Subject, body));


                MessageBox.Show("Rapor planlandı ve e-posta gönderimi sıraya alındı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log.Information("Email job enqueued for {Email}", report.Email);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "E-posta planlaması sırasında hata oluştu.");
                MessageBox.Show("İşlem sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // LAYOUT
        private void SetupFormLayout()
        {
            this.Text = "Rapor Planlayıcı";
            this.Size = new Size(600, 750);

            int labelX = 30;
            int controlX = 180;
            int y = 20;
            int spacing = 35;

            void AddControl(Control ctrl) => Controls.Add(ctrl);

            AddControl(new Label { Text = "Rapor Başlığı", Location = new Point(labelX, y), AutoSize = true });
            AddControl(new TextBox { Name = "txtReportTitle", Location = new Point(controlX, y), Width = 300 });
            y += spacing;

            AddControl(new Label { Text = "Rapor Sorgusu", Location = new Point(labelX, y), AutoSize = true });
            AddControl(new RichTextBox { Name = "rtbSqlQuery", Location = new Point(controlX, y), Size = new Size(300, 100) });
            y += 110;

            AddControl(new Label { Text = "E-Posta", Location = new Point(labelX, y), AutoSize = true });
            AddControl(new TextBox { Name = "txtEmail", Location = new Point(controlX, y), Width = 300, Text = "xxx@logocom.tr" });
            y += spacing;

            AddControl(new Label { Text = "Dizin", Location = new Point(labelX, y), AutoSize = true });
            AddControl(new TextBox { Name = "txtDirectory", Location = new Point(controlX, y), Width = 300, Text = "C:\\..." });
            y += spacing;

            AddControl(new Label { Text = "Period", Location = new Point(labelX, y), AutoSize = true });
            var cmbPeriod = new ComboBox
            {
                Name = "cmbPeriod",
                Location = new Point(controlX, y),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPeriod.Items.AddRange(new string[] { "Günlük", "Haftalık", "Aylık" });
            cmbPeriod.SelectedIndex = 0;
            AddControl(cmbPeriod);
            y += spacing;

            string[] days = { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi", "Pazar" };
            for (int i = 0; i < days.Length; i++)
            {
                AddControl(new CheckBox
                {
                    Text = days[i],
                    Location = new Point(controlX, y + i * 25),
                    AutoSize = true
                });
            }
            y += 8 * 25;

            AddControl(new Label { Text = "Gün", Location = new Point(labelX, y), AutoSize = true });
            AddControl(new DateTimePicker
            {
                Name = "dtpDate",
                Format = DateTimePickerFormat.Short,
                Location = new Point(controlX, y),
                Width = 200
            });
            y += spacing;

            AddControl(new Label { Text = "Saat", Location = new Point(labelX, y), AutoSize = true });
            AddControl(new DateTimePicker
            {
                Name = "dtpTime",
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Location = new Point(controlX, y),
                Width = 200
            });

            var btnOnayla = new Button
            {
                Name = "btnOnayla",
                Text = "Onayla",
                Location = new Point(325, y + 60),
                Width = 150,
                Height = 40
            };
            btnOnayla.Click += BtnOnayla_Click;
            AddControl(btnOnayla);
        }
    }
}
