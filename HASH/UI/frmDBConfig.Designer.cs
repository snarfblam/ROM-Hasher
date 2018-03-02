namespace HASH
{
    partial class frmDBConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lstDbs = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnRemove = new System.Windows.Forms.ToolStripButton();
            this.secretButton = new System.Windows.Forms.ToolStripButton();
            this.dbOpenDlg = new System.Windows.Forms.OpenFileDialog();
            this.chName = new System.Windows.Forms.ColumnHeader();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstDbs
            // 
            this.lstDbs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDbs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstDbs.FullRowSelect = true;
            this.lstDbs.Location = new System.Drawing.Point(12, 28);
            this.lstDbs.Name = "lstDbs";
            this.lstDbs.Size = new System.Drawing.Size(504, 215);
            this.lstDbs.TabIndex = 0;
            this.lstDbs.UseCompatibleStateImageBehavior = false;
            this.lstDbs.View = System.Windows.Forms.View.Details;
            this.lstDbs.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.lstDbs.SizeChanged += new System.EventHandler(this.listView1_SizeChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.DisplayIndex = 1;
            this.columnHeader1.Text = "Filename";
            this.columnHeader1.Width = 151;
            // 
            // columnHeader2
            // 
            this.columnHeader2.DisplayIndex = 2;
            this.columnHeader2.Text = "Platforms";
            this.columnHeader2.Width = 164;
            // 
            // columnHeader3
            // 
            this.columnHeader3.DisplayIndex = 3;
            this.columnHeader3.Text = "Format";
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnEdit,
            this.btnRemove,
            this.secretButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(528, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(84, 22);
            this.btnAdd.Text = "Add Database";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEdit.Enabled = false;
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(82, 22);
            this.btnEdit.Text = "Edit Database";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRemove.Enabled = false;
            this.btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(105, 22);
            this.btnRemove.Text = "Remove Database";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // secretButton
            // 
            this.secretButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.secretButton.Name = "secretButton";
            this.secretButton.Size = new System.Drawing.Size(82, 22);
            this.secretButton.Tag = "Used to de\'select\' other buttons that show a dialog";
            this.secretButton.Text = "Secret Button";
            this.secretButton.Visible = false;
            // 
            // dbOpenDlg
            // 
            this.dbOpenDlg.Filter = "Known Formats|*.dat|All Files|*.*";
            // 
            // chName
            // 
            this.chName.DisplayIndex = 0;
            this.chName.Text = "Name";
            this.chName.Width = 100;
            // 
            // frmDBConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 255);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lstDbs);
            this.Name = "frmDBConfig";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "DB Config";
            this.Deactivate += new System.EventHandler(this.frmDBConfig_Deactivate);
            this.Activated += new System.EventHandler(this.frmDBConfig_Activated);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstDbs;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripButton btnRemove;
        private System.Windows.Forms.OpenFileDialog dbOpenDlg;
        private System.Windows.Forms.ToolStripButton secretButton;
        private System.Windows.Forms.ColumnHeader chName;
    }
}