using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MovablePython;

namespace trinket
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var Add = new Add();

            Hotkey hk = new Hotkey();

            hk.KeyCode = Keys.PageUp;
            hk.Windows = true;
            hk.Pressed += delegate { Add.Show(); };
            hk.Register(Add);


        }

        private void Quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            var Add = new Add();
            Add.Show();
        }
    }
}
