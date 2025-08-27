using Common.Shared;
using Common.Shared.Dtos;
using Common.Shared.Enums;
using Core.Interfaces;
using Hangfire;
using Infrastructure.Logic.Database;
using Infrastructure.Logic.Filesystem;
using Infrastructure.Logic.Jobs;
using Infrastructure.Logic.Templates;
using Logo_Project.Logging;
using Serilog;
using System;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Collections.Generic;


namespace UI.WinFormsApp
{
    public partial class ReportPlannerUI : Form
    {
        private readonly ISqlQueryRunner SqlQueryRunner;
        private readonly IHangfireManager HangfireManager;
        private readonly EmailJob EmailJob;
        private readonly IFileSaver FileSaver;
        private readonly TemplateRenderer TemplateRenderer;
        private readonly IReportRepository ReportRepository;
        private bool isEditMode = false;
        private string OriginalTitle;
        private ReportDto currentReport = null;

        private List<CheckBox> dayCheckboxes = new();


        public ReportPlannerUI(IEmailSender emailSender, ISqlQueryRunner sqlQueryRunner, IHangfireManager hangfireManager,
                     IFileSaver fileSaver, EmailJob emailJob, TemplateRenderer templateRenderer, IReportRepository reportRepository)
        {
            SqlQueryRunner = sqlQueryRunner;
            HangfireManager = hangfireManager;
            EmailJob = emailJob;
            FileSaver = fileSaver;
            TemplateRenderer = templateRenderer;
            ReportRepository = reportRepository;

            InitializeComponent();
            // Build the weekly checkbox list used by logic
            dayCheckboxes = new()
            {
                cbPazartesi, cbSalı, cbÇarşamba, cbPerşembe, cbCuma, cbCumartesi, cbPazar
            };

            // Populate Period combo and hook event
            cmbPeriod.Items.AddRange(Enum.GetNames(typeof(ReportPeriod)));
            cmbPeriod.SelectedIndexChanged += CmbPeriod_SelectedIndexChanged;
            cmbPeriod.SelectedIndex = 0;    // Günlük
            cmbFileType.SelectedIndex = 0;  // Excel
            CmbPeriod_SelectedIndexChanged(cmbPeriod, EventArgs.Empty);

            // Buttons
            btnOnayla.Click += BtnOnayla_Click;
            btnBrowse.Click += BtnBrowse_Click;
            btnOnayla.Name = "btnOnayla";
        }


        private async void BtnOnayla_Click(object sender, EventArgs e)
        {
            try
            {
                UIWinFormsLoggerConfig.Instance.Logger.Information("Onayla butonuna basıldı");

                var report = isEditMode ? currentReport : new ReportDto();

                // Get file type selection
                report.FileType = cmbFileType.SelectedItem?.ToString() ?? "Excel";
                // Collect report data
                report.Email = txtEmail.Text;
                report.Subject = txtReportTitle.Text;
                report.Query = rtbSqlQuery.Text;

                if (Enum.TryParse<ReportPeriod>(cmbPeriod.SelectedItem?.ToString(), out var parsedPeriod))
                {
                    report.Period = parsedPeriod;
                }
                else
                {
                    report.Period = ReportPeriod.Günlük; // default fallback
                }

                report.SelectedDays = dayCheckboxes
                    .Where(cb => cb.Checked)
                    .Select(cb => Enum.Parse<WeekDay>(cb.Text, ignoreCase: true))
                    .ToList();

                // Execute SQL query
                var result = SqlQueryRunner.ExecuteQuery(report.Query);

                // File paths and template rendering
                string csvPath = null;
                string htmlPath = null;
                string htmlContent = null;
                string templatePath = "Templates/EmailTemplate.sbn";

                if (string.IsNullOrWhiteSpace(txtDirectory.Text))
                {
                    MessageBox.Show("Lütfen bir dizin belirtin. Bu alan zorunludur.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Use provided directory or keep the one already in report
                if (!isEditMode)
                {
                    report.Active = true;
                    report.Directory = string.IsNullOrWhiteSpace(txtDirectory.Text) ? null : txtDirectory.Text;
                }

                var dtpTime = Controls.Find("dtpTime", true).FirstOrDefault() as DateTimePicker;
                if (dtpTime != null)
                {
                    report.Time = dtpTime.Value.TimeOfDay;
                }

                var dtpDate = Controls.Find("dtpDate", true).FirstOrDefault() as DateTimePicker;
                if (dtpDate != null)
                {
                    report.Date = dtpDate.Value;
                }

                // Save or update report
                if (isEditMode)
                    ReportRepository.UpdateReport(OriginalTitle, report);
                else

                    ReportRepository.SaveReport(report);

                // Schedule recurring jobs
                HangfireManager.ScheduleRecurringEmailJobs(report);

                MessageBox.Show("Rapor planlandı ve e-posta gönderimi sıraya alındı.",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UIWinFormsLoggerConfig.Instance.Logger.Information("{Email} için e-posta gönderimi sıraya alındı.", report.Email);
                Close();
            }
            catch (Exception ex)
            {
                UIWinFormsLoggerConfig.Instance.Logger.Error(ex, "E-posta planlaması sırasında hata oluştu.");
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
            cmbPeriod.SelectedItem = report.Period.ToString();


            txtReportTitle.ReadOnly = true;
            txtDirectory.ReadOnly = true;
            OriginalTitle = report.Subject;

            var dtpTime = Controls.Find("dtpTime", true).FirstOrDefault() as DateTimePicker;
            if (dtpTime != null)
            {
                dtpTime.Value = DateTime.Today.Add(report.Time);
            }
            var dtpDate = Controls.Find("dtpDate", true).FirstOrDefault() as DateTimePicker;
            if (dtpDate != null)
            {
                dtpDate.Value = report.Date;
            }

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

            var dtpDate = Controls.Find("dtpDate", true).FirstOrDefault();
            var dtpTime = Controls.Find("dtpTime", true).FirstOrDefault();
            var lblDay = Controls.Find("lblDay", true).FirstOrDefault();
            var lblTime = Controls.Find("lblTime", true).FirstOrDefault();

            if (!Enum.TryParse<ReportPeriod>(cmbPeriod.SelectedItem?.ToString(), out var selectedPeriod))
                selectedPeriod = ReportPeriod.Günlük;

            bool isDaily = selectedPeriod == ReportPeriod.Günlük;
            bool isWeekly = selectedPeriod == ReportPeriod.Haftalık;
            bool isMonthly = selectedPeriod == ReportPeriod.Aylık;


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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void cmbPeriod_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        bool expanded = false;

        private void btnExpandSql_Click(object? sender, EventArgs e)
        {
            var sql = rtbSqlQuery.Text;
            if (Logo_Project.Report_Planner_Page.SqlEditorForm.Edit(this, ref sql))
                rtbSqlQuery.Text = sql;
        }


    }
}
