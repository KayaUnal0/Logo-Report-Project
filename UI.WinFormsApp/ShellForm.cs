using System;
using System.Windows.Forms;
using Infrastructure.Logic;
using UI.WinFormsApp;

namespace Logo_Project
{
    public partial class ShellForm : Form
    {
        public static ShellForm Current { get; private set; }
        public AppServices Services { get; }

        public ShellForm(AppServices services)
        {
            InitializeComponent();
            Services = services;
            Current = this;

            ShowInTaskbar = false;
            Opacity = 0;     // invisible holder
            Load += ShellForm_Load;
        }

        private void ShellForm_Load(object? sender, EventArgs e)
        {
            using var main = new ReportListUI(Services);
            main.ShowDialog(); 
            Close();
        }
    }
}
