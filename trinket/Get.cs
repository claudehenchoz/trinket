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
using System.Windows.Input;

namespace trinket
{
    public partial class Get : Form
    {
        public Get()
        {
            InitializeComponent();

            this.ActiveControl = trinketSearchbox;

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            
            // Handle navigation keys while keeping focus on search box
            if (trinketSearchbox.Focused)
            {
                switch (keyData)
                {
                    case Keys.Down:
                        NavigateGrid(1);
                        return true;
                    case Keys.Up:
                        NavigateGrid(-1);
                        return true;
                    case Keys.PageDown:
                        NavigateGrid(5);
                        return true;
                    case Keys.PageUp:
                        NavigateGrid(-5);
                        return true;
                    case Keys.Enter:
                        CopySelectedItemAndClose();
                        return true;
                }
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Get_Shown(object sender, EventArgs e)
        {
            this.Activate();
            trinketSearchbox.Focus();

            DataTable trinkets = new DataTable("trinkets_table");
            trinkets.Columns.Add("Preview", typeof(String));
            trinkets.Columns.Add("Text", typeof(String));
            trinkets.Columns.Add("Modified", typeof(DateTime));
            trinkets.Columns.Add("Name", typeof(String));

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string trinketFolder = Path.Combine(documentsPath, "Trinket");
            
            // Create the Trinket folder if it doesn't exist
            if (!Directory.Exists(trinketFolder))
            {
                Directory.CreateDirectory(trinketFolder);
            }
            
            string[] trinketfiles = Directory.GetFiles(trinketFolder, "*.txt");
            foreach (string trinketfile in trinketfiles) {

                FileInfo fi = new FileInfo(trinketfile);
                
                string text = File.ReadAllText(trinketfile);
                string preview = CreatePreview(text);
                DateTime modified = fi.LastWriteTime;
                string name = fi.Name;

                DataRow row = trinkets.NewRow();
                row["Preview"] = preview;
                row["Text"] = text;
                row["Modified"] = modified;
                row["Name"] = name;
                trinkets.Rows.Add(row);

            }

            trinketDataGrid.DataSource = trinkets;

            // Hide the full text column, show only preview
            trinketDataGrid.Columns["Text"].Visible = false;
            
            // Set up tooltip to show full text on hover
            trinketDataGrid.CellMouseEnter += TrinketDataGrid_CellMouseEnter;
            
            trinketDataGrid.Columns["Preview"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            trinketDataGrid.Columns["Preview"].Width = 512;
            trinketDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            trinketDataGrid.Columns["Modified"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
            trinketDataGrid.Columns["Name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

            trinketDataGrid.Sort(trinketDataGrid.Columns["Modified"], ListSortDirection.Descending);
            
            // Select first row if available
            if (trinketDataGrid.Rows.Count > 0)
            {
                trinketDataGrid.CurrentCell = trinketDataGrid.Rows[0].Cells[0];
                trinketDataGrid.Rows[0].Selected = true;
            }
            
            // Ensure search box has focus
            trinketSearchbox.Focus();
            
            trinkets.Dispose();
        }

        private void trinketSearchbox_TextChanged(object sender, EventArgs e)
        {
            (trinketDataGrid.DataSource as DataTable).DefaultView.RowFilter = string.Format("Text like '%{0}%'", trinketSearchbox.Text);
            
            // Select first row after filtering
            if (trinketDataGrid.Rows.Count > 0)
            {
                trinketDataGrid.CurrentCell = trinketDataGrid.Rows[0].Cells[0];
                trinketDataGrid.Rows[0].Selected = true;
            }
            
            // Keep focus on search box
            trinketSearchbox.Focus();
        }
        
        private void NavigateGrid(int direction)
        {
            if (trinketDataGrid.Rows.Count == 0) return;
            
            int currentIndex = trinketDataGrid.CurrentCell?.RowIndex ?? 0;
            int newIndex = Math.Max(0, Math.Min(trinketDataGrid.Rows.Count - 1, currentIndex + direction));
            
            if (newIndex < trinketDataGrid.Rows.Count)
            {
                trinketDataGrid.CurrentCell = trinketDataGrid.Rows[newIndex].Cells[0];
                trinketDataGrid.Rows[newIndex].Selected = true;
                
                // Scroll to make sure the selected row is visible
                trinketDataGrid.FirstDisplayedScrollingRowIndex = Math.Max(0, newIndex - 5);
            }
            
            // Keep focus on search box
            trinketSearchbox.Focus();
        }
        
        private void CopySelectedItemAndClose()
        {
            if (trinketDataGrid.CurrentCell != null && trinketDataGrid.CurrentCell.RowIndex >= 0)
            {
                var selectedRow = trinketDataGrid.Rows[trinketDataGrid.CurrentCell.RowIndex];
                string textToCopy = selectedRow.Cells["Text"].Value?.ToString() ?? "";
                
                if (!string.IsNullOrEmpty(textToCopy))
                {
                    Clipboard.SetText(textToCopy);
                }
            }
            
            this.Close();
        }





        private string CreatePreview(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // Split into lines and filter out empty lines or lines with only spaces/markdown quotes
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.None)
                           .Where(line => !string.IsNullOrWhiteSpace(line) && 
                                         !string.IsNullOrWhiteSpace(line.TrimStart('>', ' ', '\t')))
                           .ToList();

            // Take first 7 lines
            var previewLines = lines.Take(7).ToList();
            
            // Add [...] if we truncated
            if (lines.Count > 7)
            {
                if (previewLines.Count > 0)
                {
                    previewLines[previewLines.Count - 1] += " [...]";
                }
                else
                {
                    previewLines.Add("[...]");
                }
            }

            return string.Join(Environment.NewLine, previewLines);
        }

        private void TrinketDataGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var cell = trinketDataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                
                // Only set tooltip for the Preview column
                if (trinketDataGrid.Columns[e.ColumnIndex].Name == "Preview")
                {
                    string fullText = trinketDataGrid.Rows[e.RowIndex].Cells["Text"].Value?.ToString() ?? "";
                    cell.ToolTipText = fullText;
                }
                else
                {
                    cell.ToolTipText = "";
                }
            }
        }

    }
}
