using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

namespace trinket
{
    public partial class Get : Form
    {
        public Get()
        {
            InitializeComponent();

            this.ActiveControl = textBox1;

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Get_Shown(object sender, EventArgs e)
        {
            this.Activate();

            DataTable trinkets = new DataTable("trinkets_table");
            trinkets.Columns.Add("Text", typeof(String));
            trinkets.Columns.Add("Modified", typeof(DateTime));
            trinkets.Columns.Add("Name", typeof(String));

            string[] trinketfiles = Directory.GetFiles(".", "*.txt");
            foreach (string trinketfile in trinketfiles) {

                FileInfo fi = new FileInfo(trinketfile);
                
                string text = File.ReadAllText(trinketfile);
                DateTime modified = fi.LastWriteTime;
                string name = fi.Name;

                DataRow row = trinkets.NewRow();
                row["Text"] = text;
                row["Modified"] = modified;
                row["Name"] = name;
                trinkets.Rows.Add(row);

            }

            trinketDataGrid.DataSource = trinkets;

            trinketDataGrid.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            trinketDataGrid.Columns[0].Width = 512;
            trinketDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            trinketDataGrid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
            trinketDataGrid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

            trinketDataGrid.Sort(trinketDataGrid.Columns[1], ListSortDirection.Descending);
            
            trinkets.Dispose();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            (trinketDataGrid.DataSource as DataTable).DefaultView.RowFilter = string.Format("Text like '%{0}%'", textBox1.Text);
        }
    }
}
