using Common.Shared;
using Common.Shared.Dtos;
using Core.Interfaces;
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

        public HomeScreen(IEmailSender emailSender, ISqlQueryRunner sqlQueryRunner, IHangfireManager hangfireManager, IFileSaver fileSaver, EmailJob emailJob, TemplateRenderer templateRenderer)
        {
            _emailSender = emailSender;
            _sqlQueryRunner = sqlQueryRunner;
            _hangfireManager = hangfireManager;
            _fileSaver = fileSaver;
            _emailJob = emailJob;
            _templateRenderer = templateRenderer;

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

            lstReports = new ListBox
            {
                Location = new Point(20, 80),
                Size = new Size(540, 280)
            };
            Controls.Add(lstReports);

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

            LoadReports();
        }

        private void LoadReports()
        {
            ReportStorage.LoadFromDisk();
            _reports = ReportStorage.GetAllReports();
            lstReports.Items.Clear();

            foreach (var report in _reports)
            {
                lstReports.Items.Add($"{report.Subject} - {report.Email}");
            }
        }



        private void BtnNew_Click(object sender, EventArgs e)
        {
            var form = new Form1(_emailSender, _sqlQueryRunner, _hangfireManager, _fileSaver, _emailJob, _templateRenderer);
            form.ShowDialog();
            LoadReports(); 
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (lstReports.SelectedIndex < 0)
            {
                MessageBox.Show("Lütfen bir rapor seçin.");
                return;
            }

            var selected = _reports[lstReports.SelectedIndex];
            var form = new Form1(_emailSender, _sqlQueryRunner, _hangfireManager, _fileSaver, _emailJob, _templateRenderer);
            form.LoadReport(selected);
            form.ShowDialog();
            LoadReports();
        }


        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstReports.SelectedIndex < 0)
            {
                MessageBox.Show("Lütfen bir rapor seçin.");
                return;
            }

            int index = lstReports.SelectedIndex;
            ReportStorage.DeleteReport(index);
            LoadReports();
        }


        private void HomeScreen_Load(object sender, EventArgs e)
        {

        }
    }
}
