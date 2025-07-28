using Common.Shared;
using Common.Shared.Dtos;
using Core.Interfaces;
using Infrastructure.Logic.Database;
using Infrastructure.Logic.Filesystem;
using Infrastructure.Logic.Jobs;
using Infrastructure.Logic.Templates;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UI.WinFormsApp;

namespace Logo_Project
{
    public partial class HomeScreen : Form
    {
        private readonly IEmailSender _emailSender;
        private readonly ISqlQueryRunner _sqlQueryRunner;
        private readonly IHangfireManager _hangfireManager;
        private readonly IFileSaver _fileSaver;
        private readonly EmailJob _emailJob;
        private readonly TemplateRenderer _templateRenderer;

        private ListBox lstReports;
        private List<ReportDto> _reports;

        private readonly IReportRepository _reportRepository;

        private DataGridView dataGridViewReports;


        public HomeScreen(IEmailSender emailSender, ISqlQueryRunner sqlQueryRunner, IHangfireManager hangfireManager, IFileSaver fileSaver, EmailJob emailJob, TemplateRenderer templateRenderer, IReportRepository reportRepository)
        {
            _emailSender = emailSender;
            _sqlQueryRunner = sqlQueryRunner;
            _hangfireManager = hangfireManager;
            _fileSaver = fileSaver;
            _emailJob = emailJob;
            _templateRenderer = templateRenderer;
            _reportRepository = reportRepository;

            InitializeComponent();
            SetupLayout();
        }

        private void SetupLayout()
        {
            this.Text = "Ana Ekran";
            this.Size = new Size(600, 500);

            var btnNew = new Button
            {
                Text = "Yeni Rapor Oluştur",
                Location = new Point(20, 20),
                Size = new Size(180, 40)
            };
            btnNew.Click += BtnNew_Click;
            Controls.Add(btnNew);

            var btnEdit = new Button
            {
                Text = "Düzenle",
                Location = new Point(20, 380),
                Size = new Size(100, 30)
            };
            btnEdit.Click += BtnEdit_Click;
            Controls.Add(btnEdit);

            var btnDelete = new Button
            {
                Text = "Sil",
                Location = new Point(130, 380),
                Size = new Size(100, 30)
            };
            btnDelete.Click += BtnDelete_Click;
            Controls.Add(btnDelete);

            dataGridViewReports = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(600, 300),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            Controls.Add(dataGridViewReports);

            LoadReportsGrid();
        }
        private void LoadReportsGrid()
        {
            _reports = _reportRepository.GetReports();

            dataGridViewReports.DataSource = _reports.Select(r => new
            {
                Başlık = r.Subject,
                Period = r.Period,
                Eposta = r.Email,
                Dizin = r.Directory,
                Aktif = true
            }).ToList();
        }


        private void BtnNew_Click(object sender, EventArgs e)
        {
            var form = new Form1(_emailSender, _sqlQueryRunner, _hangfireManager, _fileSaver, _emailJob, _templateRenderer, _reportRepository);
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

            var form = new Form1(_emailSender, _sqlQueryRunner, _hangfireManager, _fileSaver, _emailJob, _templateRenderer, _reportRepository);
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
                LoadReportsGrid();
            }
        }


        private void HomeScreen_Load(object sender, EventArgs e)
        {

        }
    }
}
