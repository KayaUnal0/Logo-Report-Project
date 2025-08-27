using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Logo_Project.Report_Planner_Page
{
    public partial class SqlEditorForm : Form
    {
        public SqlEditorForm()
        {
            InitializeComponent();
        }

        private void txtSqlEditor_TextChanged(object sender, EventArgs e)
        {

        }
        public string SqlText
        {
            get => txtSqlEditor.Text;              // use plain text, not RTF
            set => txtSqlEditor.Text = value ?? string.Empty;
        }
        public static bool Edit(IWin32Window owner, ref string sql)
        {
            using var dlg = new SqlEditorForm { SqlText = sql };
            var ok = dlg.ShowDialog(owner) == DialogResult.OK;
            if (ok) sql = dlg.SqlText;
            return ok;
        }
    }
}
