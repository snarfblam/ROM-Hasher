namespace HASH
{
    partial class HashForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HashForm));
            this.RomOpenDlg = new System.Windows.Forms.OpenFileDialog();
            this.txtRHDN = new System.Windows.Forms.TextBox();
            this.mnuRHDNbox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuRHDNboxCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tbDetails = new System.Windows.Forms.TabControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.openAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.btnDoubleROM = new System.Windows.Forms.ToolStripSplitButton();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDbCongif = new System.Windows.Forms.ToolStripDropDownButton();
            this.databaseConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSkipHashes = new System.Windows.Forms.ToolStripMenuItem();
            this.lblSecondaryFile = new System.Windows.Forms.LinkLabel();
            this.SecondRomDlg = new System.Windows.Forms.OpenFileDialog();
            this.mnuDetails = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCopyDetail = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyDetailList = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlFile = new System.Windows.Forms.Panel();
            this.lblFile = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openAsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRHDNbox.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.mnuDetails.SuspendLayout();
            this.pnlFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // RomOpenDlg
            // 
            this.RomOpenDlg.Filter = "Common extensions|*.nes;*.smc;*.fds;*.sfc;*.bin;*.smd;*.gb;*.gbc;*.gba;*.nds;*.n6" +
                "4;*.gen|All files|*.*";
            // 
            // txtRHDN
            // 
            this.txtRHDN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRHDN.ContextMenuStrip = this.mnuRHDNbox;
            this.txtRHDN.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRHDN.Location = new System.Drawing.Point(186, 39);
            this.txtRHDN.Multiline = true;
            this.txtRHDN.Name = "txtRHDN";
            this.txtRHDN.ReadOnly = true;
            this.txtRHDN.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRHDN.Size = new System.Drawing.Size(316, 150);
            this.txtRHDN.TabIndex = 3;
            this.txtRHDN.WordWrap = false;
            // 
            // mnuRHDNbox
            // 
            this.mnuRHDNbox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRHDNboxCopy});
            this.mnuRHDNbox.Name = "mnuRHDNbox";
            this.mnuRHDNbox.Size = new System.Drawing.Size(103, 26);
            // 
            // mnuRHDNboxCopy
            // 
            this.mnuRHDNboxCopy.Image = ((System.Drawing.Image)(resources.GetObject("mnuRHDNboxCopy.Image")));
            this.mnuRHDNboxCopy.Name = "mnuRHDNboxCopy";
            this.mnuRHDNboxCopy.Size = new System.Drawing.Size(102, 22);
            this.mnuRHDNboxCopy.Text = "Copy";
            this.mnuRHDNboxCopy.Click += new System.EventHandler(this.mnuRHDNboxCopy_Click);
            // 
            // tbDetails
            // 
            this.tbDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDetails.Location = new System.Drawing.Point(12, 195);
            this.tbDetails.Name = "tbDetails";
            this.tbDetails.SelectedIndex = 0;
            this.tbDetails.Size = new System.Drawing.Size(490, 193);
            this.tbDetails.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripButton,
            this.copyToolStripButton,
            this.btnDoubleROM,
            this.btnDbCongif});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(514, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openAsToolStripMenuItem});
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(68, 22);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.ButtonClick += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // openAsToolStripMenuItem
            // 
            this.openAsToolStripMenuItem.Name = "openAsToolStripMenuItem";
            this.openAsToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.openAsToolStripMenuItem.Text = "Open As...";
            this.openAsToolStripMenuItem.Click += new System.EventHandler(this.openAsToolStripMenuItem_Click);
            // 
            // copyToolStripButton
            // 
            this.copyToolStripButton.Image = global::HASH.Res.copy_16;
            this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripButton.Name = "copyToolStripButton";
            this.copyToolStripButton.Size = new System.Drawing.Size(109, 22);
            this.copyToolStripButton.Text = "&Copy Summary";
            this.copyToolStripButton.Click += new System.EventHandler(this.copyToolStripButton_Click);
            // 
            // btnDoubleROM
            // 
            this.btnDoubleROM.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openAsToolStripMenuItem1});
            this.btnDoubleROM.Image = ((System.Drawing.Image)(resources.GetObject("btnDoubleROM.Image")));
            this.btnDoubleROM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDoubleROM.Name = "btnDoubleROM";
            this.btnDoubleROM.Size = new System.Drawing.Size(112, 22);
            this.btnDoubleROM.Text = "Patched ROM";
            this.btnDoubleROM.ButtonClick += new System.EventHandler(this.btnDouble_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // btnDbCongif
            // 
            this.btnDbCongif.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnDbCongif.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDbCongif.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.databaseConfigToolStripMenuItem,
            this.btnSkipHashes});
            this.btnDbCongif.Image = ((System.Drawing.Image)(resources.GetObject("btnDbCongif.Image")));
            this.btnDbCongif.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDbCongif.Name = "btnDbCongif";
            this.btnDbCongif.Size = new System.Drawing.Size(29, 22);
            this.btnDbCongif.Text = "DB Config";
            // 
            // databaseConfigToolStripMenuItem
            // 
            this.databaseConfigToolStripMenuItem.Name = "databaseConfigToolStripMenuItem";
            this.databaseConfigToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.databaseConfigToolStripMenuItem.Text = "Database Config";
            this.databaseConfigToolStripMenuItem.Click += new System.EventHandler(this.btnDbCongif_Click);
            // 
            // btnSkipHashes
            // 
            this.btnSkipHashes.CheckOnClick = true;
            this.btnSkipHashes.Name = "btnSkipHashes";
            this.btnSkipHashes.Size = new System.Drawing.Size(194, 22);
            this.btnSkipHashes.Text = "Skip Unneeded Hashes";
            this.btnSkipHashes.CheckedChanged += new System.EventHandler(this.btnSkipHashes_CheckedChanged);
            // 
            // lblSecondaryFile
            // 
            this.lblSecondaryFile.AllowDrop = true;
            this.lblSecondaryFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.lblSecondaryFile.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecondaryFile.ForeColor = System.Drawing.Color.Black;
            this.lblSecondaryFile.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(254)))));
            this.lblSecondaryFile.Location = new System.Drawing.Point(12, 153);
            this.lblSecondaryFile.Margin = new System.Windows.Forms.Padding(10);
            this.lblSecondaryFile.Name = "lblSecondaryFile";
            this.lblSecondaryFile.Size = new System.Drawing.Size(168, 35);
            this.lblSecondaryFile.TabIndex = 7;
            this.lblSecondaryFile.TabStop = true;
            this.lblSecondaryFile.Tag = "Load or drag a ROM.";
            this.lblSecondaryFile.Text = "Load or drag a ROM.";
            this.lblSecondaryFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSecondaryFile.Visible = false;
            this.lblSecondaryFile.Paint += new System.Windows.Forms.PaintEventHandler(this.lblSecondaryFile_Paint);
            this.lblSecondaryFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSecondaryFile_LinkClicked);
            this.lblSecondaryFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.lblSecondaryFile_DragDrop);
            this.lblSecondaryFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.lblSecondaryFile_DragEnter);
            // 
            // SecondRomDlg
            // 
            this.SecondRomDlg.Filter = "Common extensions|*.nes;*.smc;*.fds;*.sfc;*.bin;*.smd;*.gb;*.gbc;*.gba;*.nds;*.n6" +
                "4;*.gen|All files|*.*";
            // 
            // mnuDetails
            // 
            this.mnuDetails.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCopyDetail,
            this.mnuCopyDetailList});
            this.mnuDetails.Name = "mnuDetails";
            this.mnuDetails.Size = new System.Drawing.Size(120, 48);
            // 
            // mnuCopyDetail
            // 
            this.mnuCopyDetail.Image = global::HASH.Res.copy_16;
            this.mnuCopyDetail.Name = "mnuCopyDetail";
            this.mnuCopyDetail.Size = new System.Drawing.Size(119, 22);
            this.mnuCopyDetail.Text = "Copy";
            this.mnuCopyDetail.DropDownOpening += new System.EventHandler(this.mnuCopyDetail_DropDownOpening);
            this.mnuCopyDetail.Click += new System.EventHandler(this.mnuCopyDetail_Click);
            // 
            // mnuCopyDetailList
            // 
            this.mnuCopyDetailList.Image = global::HASH.Res.copy_16;
            this.mnuCopyDetailList.Name = "mnuCopyDetailList";
            this.mnuCopyDetailList.Size = new System.Drawing.Size(119, 22);
            this.mnuCopyDetailList.Text = "Copy All";
            this.mnuCopyDetailList.DropDownOpening += new System.EventHandler(this.mnuCopyDetail_DropDownOpening);
            this.mnuCopyDetailList.Click += new System.EventHandler(this.mnuCopyDetailList_Click);
            // 
            // pnlFile
            // 
            this.pnlFile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlFile.BackgroundImage")));
            this.pnlFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlFile.Controls.Add(this.lblFile);
            this.pnlFile.Location = new System.Drawing.Point(12, 39);
            this.pnlFile.Name = "pnlFile";
            this.pnlFile.Size = new System.Drawing.Size(168, 147);
            this.pnlFile.TabIndex = 6;
            this.pnlFile.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlFile_Paint);
            // 
            // lblFile
            // 
            this.lblFile.AllowDrop = true;
            this.lblFile.BackColor = System.Drawing.Color.Transparent;
            this.lblFile.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFile.ForeColor = System.Drawing.Color.Black;
            this.lblFile.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(254)))));
            this.lblFile.Location = new System.Drawing.Point(0, 10);
            this.lblFile.Margin = new System.Windows.Forms.Padding(10);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(168, 137);
            this.lblFile.TabIndex = 1;
            this.lblFile.TabStop = true;
            this.lblFile.Text = "Load a ROM \r\nor drag and \r\ndrop it here.";
            this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFile.Paint += new System.Windows.Forms.PaintEventHandler(this.lblFile_Paint);
            this.lblFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblFile_LinkClicked);
            this.lblFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.lblFile_DragDrop);
            this.lblFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.lblFile_DragEnter);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(186, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(316, 150);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // openAsToolStripMenuItem1
            // 
            this.openAsToolStripMenuItem1.Name = "openAsToolStripMenuItem1";
            this.openAsToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.openAsToolStripMenuItem1.Text = "Open As...";
            this.openAsToolStripMenuItem1.Click += new System.EventHandler(this.openAsToolStripMenuItem1_Click);
            // 
            // HashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 400);
            this.Controls.Add(this.lblSecondaryFile);
            this.Controls.Add(this.pnlFile);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tbDetails);
            this.Controls.Add(this.txtRHDN);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HashForm";
            this.Text = "ROM Hasher";
            this.Deactivate += new System.EventHandler(this.HashForm_Deactivate);
            this.Load += new System.EventHandler(this.HashForm_Load);
            this.Activated += new System.EventHandler(this.HashForm_Activated);
            this.Click += new System.EventHandler(this.HashForm_Click);
            this.mnuRHDNbox.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.mnuDetails.ResumeLayout(false);
            this.pnlFile.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog RomOpenDlg;
        private System.Windows.Forms.LinkLabel lblFile;
        private System.Windows.Forms.TextBox txtRHDN;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip mnuRHDNbox;
        private System.Windows.Forms.ToolStripMenuItem mnuRHDNboxCopy;
        private System.Windows.Forms.TabControl tbDetails;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.Panel pnlFile;
        private System.Windows.Forms.LinkLabel lblSecondaryFile;
        private System.Windows.Forms.OpenFileDialog SecondRomDlg;
        private System.Windows.Forms.ToolStripSplitButton btnDoubleROM;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip mnuDetails;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyDetail;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyDetailList;
        private System.Windows.Forms.ToolStripDropDownButton btnDbCongif;
        private System.Windows.Forms.ToolStripMenuItem databaseConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton openToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem openAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnSkipHashes;
        private System.Windows.Forms.ToolStripMenuItem openAsToolStripMenuItem1;
    }
}