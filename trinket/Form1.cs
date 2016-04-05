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

        private void MainForm_Load(object sender, EventArgs e)
        {
            Hotkey hk = new Hotkey();
            hk.KeyCode = Keys.PageUp;
            hk.Windows = true;
            hk.Pressed += delegate { var Add = new Add(); Add.Show(); };
            hk.Register(this);

            Hotkey hkget = new Hotkey();
            hkget.KeyCode = Keys.PageDown;
            hkget.Windows = true;
            hkget.Pressed += delegate { var Get = new Get(); Get.Show(); };
            hkget.Register(this);

        }

        private void Get_Click(object sender, EventArgs e)
        {
            var Get = new Get();
            Get.Show();
        }

    }

}
