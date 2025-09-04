using Common.Shared;
using Common.Shared.Dtos;
using Core.Interfaces;
using Infrastructure.Logic;
using Infrastructure.Logic.Config;
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
        private readonly AppServices Services;
        private List<ReportDto> Reports;

        public ReportListUI(AppServices services)
        {
            Services = services;

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
            Reports = Services.ReportRepository.GetReports();

            dataGridViewReports.DataSource = Reports;
        }


        private void BtnNew_Click(object sender, EventArgs e)
        {
            var form = new UI.WinFormsApp.ReportPlannerUI(Services);
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
            var selected = Reports[index];

            var form = new UI.WinFormsApp.ReportPlannerUI(Services);
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
            var selected = Reports[index];

            var confirm = MessageBox.Show($"'{selected.Subject}' başlıklı raporu silmek istiyor musunuz?", "Onayla", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                Services.ReportRepository.DeleteReport(selected.Subject);
                Services.HangfireManager.RemoveRecurringJob(selected.Subject);
                LoadReportsGrid();
            }
        }

        private void HomeScreen_Load(object sender, EventArgs e)
        {
            foreach (var report in Reports.Where(r => r.Active))
            {
                Services.HangfireManager.ScheduleRecurringEmailJobs(report);
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
                    Services.ReportRepository.UpdateReport(report.Subject, report);

                    if (report.Active)
                        Services.HangfireManager.ScheduleRecurringEmailJobs(report);
                    else
                        Services.HangfireManager.RemoveRecurringJob(report.Subject);
                }
            }
        }

        private void BtnSettings_Click(object? sender, EventArgs e)
        {
            using var chooser = new SettingsChooserForm();   //Veritabanı / E-posta modal
            if (chooser.ShowDialog(this) == DialogResult.OK)
            {
                // If Database settings were saved, rebuild the SQL runner immediately
                if (chooser.DatabaseSaved)
                {
                    var (_, queryDb) = SettingsManager.LoadQueryDb();
                    Services.SqlQueryRunner = new SqlQueryRunner(queryDb);
                }

                MessageBox.Show("Ayarlar güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridViewReports_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
