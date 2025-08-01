using Common.Shared;
using Common.Shared.Dtos;
using Common.Shared.Enums;
using Core.Interfaces;
using Hangfire;
using Infrastructure.Logic.Database;
using Infrastructure.Logic.Filesystem;
using Infrastructure.Logic.Jobs;
using Infrastructure.Logic.Templates;
using Serilog;
using System;
using System.Drawing;
using System.Text.Json;
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
        private readonly IReportRepository _reportRepository;
        private bool isEditMode = false;
        private string _originalTitle;
        private ReportDto currentReport = null;

        private TextBox txtReportTitle, txtEmail, txtDirectory;
        private RichTextBox rtbSqlQuery;
        private ComboBox cmbPeriod;
        private List<CheckBox> dayCheckboxes = new();


        public Form1(IEmailSender emailSender, ISqlQueryRunner sqlQueryRunner, IHangfireManager hangfireManager, IFileSaver fileSaver, EmailJob emailJob, TemplateRenderer templateRenderer, IReportRepository reportRepository)
        {
            _sqlQueryRunner = sqlQueryRunner;
            _hangfireManager = hangfireManager;
            _emailJob = emailJob;
            _fileSaver = fileSaver;
            _templateRenderer = templateRenderer;
            _reportRepository = reportRepository;

            InitializeComponent();
            SetupFormLayout();
        }


        private async void BtnOnayla_Click(object sender, EventArgs e)
        {
            try
            {
                Log.Information("Onayla button clicked");

                var report = isEditMode ? currentReport : new ReportDto();

                // Get file type selection
                var cmbFileType = Controls.Find("cmbFileType", true).FirstOrDefault() as ComboBox;
                report.FileType = cmbFileType?.SelectedItem?.ToString() ?? "Excel";

                // Collect report data
                report.Email = txtEmail.Text;
                report.Subject = txtReportTitle.Text;
                report.Query = rtbSqlQuery.Text;
                report.Period = cmbPeriod.SelectedItem?.ToString() ?? "";
                report.SelectedDays = dayCheckboxes
                    .Where(cb => cb.Checked)
                    .Select(cb => Enum.Parse<WeekDay>(cb.Text, ignoreCase: true))
                    .ToList();

                // Execute SQL query
                var result = _sqlQueryRunner.ExecuteQuery(report.Query);

                // File paths and template rendering
                string csvPath = null;
                string htmlPath = null;
                string htmlContent = null;
                string templatePath = "Templates/EmailTemplate.sbn";

                if (!isEditMode)
                {
                    report.Directory = string.IsNullOrWhiteSpace(txtDirectory.Text) ? null : txtDirectory.Text;

                    if (!string.IsNullOrEmpty(report.Directory))
                    {
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        var fileBaseName = $"Report_{timestamp}";

                        // Save CSV if needed
                        if (report.FileType.Contains("Excel"))
                        {
                            csvPath = _fileSaver.SaveCsvToFile(
                                report.Directory,
                                fileBaseName + ".csv",
                                result.Results
                            );
                        }

                        // Save HTML if needed
                        if (report.FileType.Contains("HTML"))
                        {
                            htmlContent = await _templateRenderer.RenderTemplateAsync(templatePath, new
                            {
                                subject = report.Subject,
                                status = result.Status.ToString(),
                                results = string.Join("\n", result.Results),
                                filePath = report.Directory
                            });

                            htmlPath = _fileSaver.SaveHtmlToFile(
                                report.Directory,
                                fileBaseName + ".html",
                                htmlContent
                            );
                        }
                    }
                }

                // Prepare email body
                string emailBody;
                if (report.FileType == "HTML" || report.FileType == "Excel, HTML")
                {
                    if (htmlContent == null)
                    {
                        htmlContent = await _templateRenderer.RenderTemplateAsync(templatePath, new
                        {
                            subject = report.Subject,
                            status = result.Status.ToString(),
                            results = string.Join("\n", result.Results),
                            filePath = report.Directory
                        });
                    }

                    emailBody = htmlContent;
                }
                else
                {
                    // CSV-only fallback
                    emailBody = string.Join("<br>", result.Results);
                }

                // Prepare attachments
                var attachments = new List<string>();
                if (!string.IsNullOrEmpty(csvPath))
                    attachments.Add(csvPath);
                if (!string.IsNullOrEmpty(htmlPath))
                    attachments.Add(htmlPath);

                // Send email in background
                BackgroundJob.Enqueue(() =>
                    EmailJobWrapper.SendEmail(report.Email, report.Subject, emailBody, attachments.ToArray()));

                // Save or update report
                if (isEditMode)
                    _reportRepository.UpdateReport(_originalTitle, report);
                else
                    _reportRepository.SaveReport(report);

                // Remove previous jobs
                foreach (WeekDay day in Enum.GetValues(typeof(WeekDay)))
                {
                    var jobId = $"report:{report.Subject}:{day}";
                    RecurringJob.RemoveIfExists(jobId);
                }

                // Schedule recurring jobs
                _hangfireManager.ScheduleRecurringEmailJobs(report);

                MessageBox.Show("Rapor planlandı ve e-posta gönderimi sıraya alındı.",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Log.Information("Email job enqueued for {Email}", report.Email);
                Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "E-posta planlaması sırasında hata oluştu.");
                MessageBox.Show("İşlem sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        public void LoadReport(ReportDto report)
        {
            isEditMode = true;
            currentReport = report;
            txtReportTitle.Text = report.Subject;
            txtEmail.Text = report.Email;
            rtbSqlQuery.Text = report.Query;
            txtDirectory.Text = report.Directory;
            cmbPeriod.SelectedItem = report.Period;

            txtReportTitle.ReadOnly = true;
            txtDirectory.ReadOnly = true;
            _originalTitle = report.Subject;

            foreach (var cb in dayCheckboxes)
            {
                if (Enum.TryParse<WeekDay>(cb.Text, ignoreCase: true, out var day))
                {
                    cb.Checked = report.SelectedDays?.Contains(day) == true;
                }
            }

        }


        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtDirectory.Text = dialog.SelectedPath;
            }
        }

        private void CmbPeriod_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string selectedPeriod = cmbPeriod.SelectedItem?.ToString() ?? "";

            var dtpDate = Controls.Find("dtpDate", true).FirstOrDefault();
            var dtpTime = Controls.Find("dtpTime", true).FirstOrDefault();
            var lblDay = Controls.Find("lblDay", true).FirstOrDefault();
            var lblTime = Controls.Find("lblTime", true).FirstOrDefault();

            bool isDaily = selectedPeriod == "Günlük";
            bool isWeekly = selectedPeriod == "Haftalık";
            bool isMonthly = selectedPeriod == "Aylık";

            int baseY = cmbPeriod.Location.Y + 40;

            // Reposition or hide Day Label & Picker
            if (lblDay != null && dtpDate != null)
            {
                lblDay.Visible = isMonthly;
                dtpDate.Visible = isMonthly;

                if (isMonthly)
                {
                    lblDay.Location = new Point(lblDay.Location.X, baseY);
                    dtpDate.Location = new Point(dtpDate.Location.X, baseY);
                    baseY += 35;
                }
            }

            // Reposition and show/hide Time
            if (lblTime != null && dtpTime != null)
            {
                lblTime.Visible = isDaily || isWeekly || isMonthly;
                dtpTime.Visible = isDaily || isWeekly || isMonthly;

                lblTime.Location = new Point(lblTime.Location.X, baseY);
                dtpTime.Location = new Point(dtpTime.Location.X, baseY);
                baseY += 35;
            }

            // Reposition day checkboxes for Haftalık
            int cbY = baseY;
            foreach (var cb in dayCheckboxes)
            {
                cb.Visible = isWeekly;
                cb.Location = new Point(cb.Location.X, cbY);
                cbY += 25;
            }

            // Reposition Onayla button after all
            var btnOnayla = Controls.Find("btnOnayla", true).FirstOrDefault();
            if (btnOnayla != null)
            {
                if (!isWeekly)
                {
                    btnOnayla.Location = new Point(btnOnayla.Location.X, baseY + 20);
                }
                else
                {
                    btnOnayla.Location = new Point(btnOnayla.Location.X, cbY + 20);
                }
                
            }
        }


        private void SetupFormLayout()
        {
            this.Text = "Rapor Planlayıcı";
            this.Size = new Size(600, 750);
            this.MinimumSize = this.Size;


            int labelX = 30;
            int controlX = 180;
            int y = 20;
            int spacing = 35;

            //Rapor Başlığı
            Controls.Add(new Label { Text = "Rapor Başlığı", Location = new Point(labelX, y), AutoSize = true });
            txtReportTitle = new TextBox 
            { 
                Location = new Point(controlX, y), 
                Width = 300,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(txtReportTitle);
            y += spacing;

            //Rapor Sorgusu
            Controls.Add(new Label { Text = "Rapor Sorgusu", Location = new Point(labelX, y), AutoSize = true });
            rtbSqlQuery = new RichTextBox 
            { 
                Location = new Point(controlX, y), 
                Size = new Size(300, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(rtbSqlQuery);
            y += 110;

            //E-posta
            Controls.Add(new Label { Text = "E-Posta", Location = new Point(labelX, y), AutoSize = true });
            txtEmail = new TextBox 
            { 
                Location = new Point(controlX, y), 
                Width = 300, 
                Text = "xxx@logocom.tr",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(txtEmail);
            y += spacing;

            //Dizin
            Controls.Add(new Label { Text = "Dizin", Location = new Point(labelX, y), AutoSize = true });
            txtDirectory = new TextBox
            {
                Location = new Point(controlX, y),
                Width = 220,
                PlaceholderText = "İsteğe bağlı",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(txtDirectory);

            //Gözat butonu
            var btnBrowse = new Button
            {
                Text = "Gözat...",
                Location = new Point(controlX + 230, y),
                Width = 70,
                Height = 25,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnBrowse.Click += BtnBrowse_Click;
            Controls.Add(btnBrowse);
            y += spacing;

            //Period
            Controls.Add(new Label { Text = "Period", Location = new Point(labelX, y), AutoSize = true });
            cmbPeriod = new ComboBox
            {
                Location = new Point(controlX, y),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            cmbPeriod.Items.AddRange(new string[] { "Günlük", "Haftalık", "Aylık" });
            Controls.Add(cmbPeriod);
            y += spacing;

            // Day checkboxes
            string[] days = { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi", "Pazar" };
            for (int i = 0; i < days.Length; i++)
            {
                var cb = new CheckBox
                {
                    Text = days[i],
                    Location = new Point(controlX, y + i * 25),
                    AutoSize = true
                };
                dayCheckboxes.Add(cb);
                Controls.Add(cb);
            }
            y += 8 * 25;

            // Gün (label) — for Aylık
            var lblDay = new Label
            {
                Name = "lblDay",
                Text = "Gün",
                Location = new Point(labelX, y),
                AutoSize = true
            };
            Controls.Add(lblDay);

            Controls.Add(new DateTimePicker
            {
                Name = "dtpDate",
                Format = DateTimePickerFormat.Short,
                Location = new Point(controlX, y),
                Width = 200,
                 Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            });
            y += spacing;

            // Saat (label) — for all
            var lblTime = new Label
            {
                Name = "lblTime",
                Text = "Saat",
                Location = new Point(labelX, y),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(lblTime);

            Controls.Add(new DateTimePicker
            {
                Name = "dtpTime",
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Location = new Point(controlX, y),
                Width = 200,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            });
            y += spacing;

            Controls.Add(new Label { Text = "Dosya türü", Location = new Point(labelX, y), AutoSize = true });
            var cmbFileType = new ComboBox
            {
                Name = "cmbFileType",
                Location = new Point(controlX, y),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFileType.Items.AddRange(new string[] { "Excel", "HTML", "Excel, HTML" });
            cmbFileType.SelectedIndex = 0;
            Controls.Add(cmbFileType);
            y += spacing;

            var btnOnayla = new Button
            {
                Name = "btnOnayla",
                Text = "Onayla",
                Location = new Point(325, y + 20),
                Width = 150,
                Height = 40,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnOnayla.Click += BtnOnayla_Click;
            Controls.Add(btnOnayla);

            cmbPeriod.SelectedIndexChanged += CmbPeriod_SelectedIndexChanged;
            cmbPeriod.SelectedIndex = 0; // Select "Günlük" by default
            CmbPeriod_SelectedIndexChanged(cmbPeriod, EventArgs.Empty);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
