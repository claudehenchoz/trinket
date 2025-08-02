namespace trinket
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TrinketIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrinketMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Add = new System.Windows.Forms.ToolStripMenuItem();
            this.Get = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Quit = new System.Windows.Forms.ToolStripMenuItem();
            this.TrinketMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrinketIcon
            // 
            this.TrinketIcon.ContextMenuStrip = this.TrinketMenu;
            this.TrinketIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrinketIcon.Icon")));
            this.TrinketIcon.Text = "Trinket";
            this.TrinketIcon.Visible = true;
            // 
            // TrinketMenu
            // 
            this.TrinketMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Add,
            this.Get,
            this.toolStripSeparator1,
            this.Quit});
            this.TrinketMenu.Name = "TrinketMenu";
            this.TrinketMenu.ShowImageMargin = false;
            this.TrinketMenu.Size = new System.Drawing.Size(128, 98);
            // 
            // Add
            // 
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(127, 22);
            this.Add.Text = "Add  WIN+Ctrl+PgUp";
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // Get
            // 
            this.Get.Name = "Get";
            this.Get.Size = new System.Drawing.Size(127, 22);
            this.Get.Text = "Get  WIN+Ctrl+PgDown";
            this.Get.Click += new System.EventHandler(this.Get_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(124, 6);
            // 
            // Quit
            // 
            this.Quit.Name = "Quit";
            this.Quit.Size = new System.Drawing.Size(127, 22);
            this.Quit.Text = "Quit";
            this.Quit.Click += new System.EventHandler(this.Quit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.Text = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.TrinketMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon TrinketIcon;
        private System.Windows.Forms.ContextMenuStrip TrinketMenu;
        private System.Windows.Forms.ToolStripMenuItem Add;
        private System.Windows.Forms.ToolStripMenuItem Get;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem Quit;
    }
}

