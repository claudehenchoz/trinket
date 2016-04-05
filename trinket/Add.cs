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

using System.Diagnostics;

using YamlDotNet.Serialization;

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
            var NewEntry = new Entry(textBox1.Text, DateTime.Now, "firstentry");
            var yaml = new Serializer();
            StreamWriter streamWriter = new StreamWriter(Guid.NewGuid().ToString()+".txt");
            yaml.Serialize(streamWriter, NewEntry);
            streamWriter.Close();
            this.Close();
        }

    }
}
