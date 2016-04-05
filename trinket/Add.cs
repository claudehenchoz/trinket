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

using System.Web.Script.Serialization;


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
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var NewEntry = new Entry();
            NewEntry.date = DateTime.Now;
            NewEntry.text = textBox1.Text;
            NewEntry.name = "firstentry";

            var json = new JavaScriptSerializer().Serialize(NewEntry);

            File.WriteAllText("test.txt", json);

            this.Close();
        }

    }
}
