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
    public partial class Add : Form
    {
        public Add()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            
            if (keyData == (Keys.Control | Keys.Enter))
            {
                SaveAndClose();
                return true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveAndClose();
        }
        
        private void SaveAndClose()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string trinketFolder = Path.Combine(documentsPath, "Trinket");
            
            // Create the Trinket folder if it doesn't exist
            if (!Directory.Exists(trinketFolder))
            {
                Directory.CreateDirectory(trinketFolder);
            }
            
            string filePath = Path.Combine(trinketFolder, Guid.NewGuid().ToString() + ".txt");
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(textBox1.Text.Trim());
            streamWriter.Close();
            this.Close();
        }

        private void Add_Shown(object sender, EventArgs e)
        {
            this.Activate();
        }

    }
}
