namespace trinket
{
    partial class Get
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Get));
            this.trinketDataGrid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.trinketDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // trinketDataGrid
            // 
            this.trinketDataGrid.AllowUserToAddRows = false;
            this.trinketDataGrid.AllowUserToDeleteRows = false;
            this.trinketDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.trinketDataGrid.Location = new System.Drawing.Point(5, 5);
            this.trinketDataGrid.Name = "trinketDataGrid";
            this.trinketDataGrid.ReadOnly = true;
            this.trinketDataGrid.RowHeadersVisible = false;
            this.trinketDataGrid.RowHeadersWidth = 20;
            this.trinketDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.trinketDataGrid.Size = new System.Drawing.Size(614, 591);
            this.trinketDataGrid.TabIndex = 0;
            // 
            // Get
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 601);
            this.Controls.Add(this.trinketDataGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Get";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Get Trinket";
            this.Shown += new System.EventHandler(this.Get_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.trinketDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView trinketDataGrid;
    }
}