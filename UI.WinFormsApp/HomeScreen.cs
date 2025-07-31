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

        private void SetupLayout()
        {
            this.Text = "Ana Ekran";
            this.Size = new Size(800, 620);
            this.MinimumSize = new Size(500, 620);

            // Yeni Rapor Butonu
            var btnNew = new Button
            {
                Text = "Yeni Rapor Oluştur",
                Location = new Point(20, 20),
                Size = new Size(180, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnNew.Click += BtnNew_Click;
            Controls.Add(btnNew);

            // Düzenle Butonu
            var btnEdit = new Button
            {
                Text = "Düzenle",
                Location = new Point(20, 480),
                Size = new Size(140, 35),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnEdit.Click += BtnEdit_Click;
            Controls.Add(btnEdit);

            // Sil Butonu
            var btnDelete = new Button
            {
                Text = "Sil",
                Location = new Point(180, 480),
                Size = new Size(140, 35),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnDelete.Click += BtnDelete_Click;
            Controls.Add(btnDelete);

            //Grid
            dataGridViewReports = new DataGridView
            {
                Location = new Point(20, 90),
                Size = new Size(740, 340),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            Controls.Add(dataGridViewReports);

            LoadReportsGrid();
        }


        private void HomeScreen_Load(object sender, EventArgs e)
        {

        }
    }
}
