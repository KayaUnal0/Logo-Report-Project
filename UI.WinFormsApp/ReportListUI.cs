using Common.Shared;
using Common.Shared.Dtos;
using Core.Interfaces;
using Infrastructure.Logic.Database;
using Infrastructure.Logic.Filesystem;
using Infrastructure.Logic.Jobs;
using Infrastructure.Logic.Templates;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UI.WinFormsApp;

namespace Logo_Project
{
    public partial class ReportListUI : Form
    {
        private readonly IEmailSender _emailSender;
        private ISqlQueryRunner _sqlQueryRunner;
        private readonly IHangfireManager _hangfireManager;
        private readonly IFileSaver _fileSaver;
        private readonly EmailJob _emailJob;
        private readonly TemplateRenderer _templateRenderer;

        private List<ReportDto> _reports;

        private readonly IReportRepository _reportRepository;



        public ReportListUI(IEmailSender emailSender, ISqlQueryRunner sqlQueryRunner, IHangfireManager hangfireManager,
                          IFileSaver fileSaver, EmailJob emailJob, TemplateRenderer templateRenderer, IReportRepository reportRepository)
        {
            _emailSender = emailSender;
            _sqlQueryRunner = sqlQueryRunner;
            _hangfireManager = hangfireManager;
            _fileSaver = fileSaver;
            _emailJob = emailJob;
            _templateRenderer = templateRenderer;
            _reportRepository = reportRepository;

            InitializeComponent();

            btnNew.Click += BtnNew_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSettings.Click += BtnSettings_Click;

            dataGridViewReports.CurrentCellDirtyStateChanged += DataGridViewReports_CurrentCellDirtyStateChanged;
            dataGridViewReports.CellValueChanged += DataGridViewReports_CellValueChanged;
            dataGridViewReports.AutoGenerateColumns = false;

            LoadReportsGrid();
        }


        private void LoadReportsGrid()
        {
            _reports = _reportRepository.GetReports();

            dataGridViewReports.DataSource = _reports;
        }


        private void BtnNew_Click(object sender, EventArgs e)
        {
            var form = new ReportPlannerUI(_emailSender, _sqlQueryRunner, _hangfireManager, _fileSaver, _emailJob, _templateRenderer, _reportRepository);
            form.ShowDialog();
            LoadReportsGrid();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewReports.CurrentRow == null)
            {
                MessageBox.Show("Lütfen bir rapor seçin.");
                return;
            }

            var index = dataGridViewReports.CurrentRow.Index;
            var selected = _reports[index];

            var form = new ReportPlannerUI(_emailSender, _sqlQueryRunner, _hangfireManager, _fileSaver, _emailJob, _templateRenderer, _reportRepository);
            form.LoadReport(selected);
            form.ShowDialog();
            LoadReportsGrid();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewReports.CurrentRow == null)
            {
                MessageBox.Show("Lütfen bir rapor seçin.");
                return;
            }

            var index = dataGridViewReports.CurrentRow.Index;
            var selected = _reports[index];

            var confirm = MessageBox.Show($"'{selected.Subject}' başlıklı raporu silmek istiyor musunuz?", "Onayla", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                _reportRepository.DeleteReport(selected.Subject);
                _hangfireManager.RemoveRecurringJob(selected.Subject);
                LoadReportsGrid();
            }
        }

        private void HomeScreen_Load(object sender, EventArgs e)
        {
            foreach (var report in _reports.Where(r => r.Active))
            {
                _hangfireManager.ScheduleRecurringEmailJobs(report);
            }
        }

        // Fires when a checkbox cell is clicked so the value actually commits
        private void DataGridViewReports_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dataGridViewReports.IsCurrentCellDirty)
                dataGridViewReports.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        // Persist Active toggle and sync Hangfire
        private void DataGridViewReports_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = dataGridViewReports.Columns[e.ColumnIndex];
            if (col.DataPropertyName == "Active")
            {
                if (dataGridViewReports.Rows[e.RowIndex].DataBoundItem is ReportDto report)
                {
                    _reportRepository.UpdateReport(report.Subject, report);

                    if (report.Active)
                        _hangfireManager.ScheduleRecurringEmailJobs(report);
                    else
                        _hangfireManager.RemoveRecurringJob(report.Subject);
                }
            }
        }

        private void BtnSettings_Click(object? sender, EventArgs e)
        {
            using var dlg = new DbSettingsForm();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                // Rebuild config & runner so new settings take effect now
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                _sqlQueryRunner = new SqlQueryRunner(config);

                MessageBox.Show(
                    "Ayarlar kaydedildi. Yeni sorgular güncel veritabanını kullanacak.",
                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridViewReports_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewReports_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
