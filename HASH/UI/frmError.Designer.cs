namespace HASH
{
    partial class frmError
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmError));
            this.lblExType = new System.Windows.Forms.Label();
            this.txtDetails = new System.Windows.Forms.TextBox();
            this.lstStackTrace = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOK = new System.Windows.Forms.ToolStripButton();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnDetails = new System.Windows.Forms.ToolStripButton();
            this.pgDetails = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblExType
            // 
            this.lblExType.AutoSize = true;
            this.lblExType.Location = new System.Drawing.Point(12, 9);
            this.lblExType.Name = "lblExType";
            this.lblExType.Size = new System.Drawing.Size(171, 13);
            this.lblExType.TabIndex = 0;
            this.lblExType.Text = "System.ToBeDeterminedException";
            // 
            // txtDetails
            // 
            this.txtDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDetails.Location = new System.Drawing.Point(12, 25);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.Size = new System.Drawing.Size(408, 60);
            this.txtDetails.TabIndex = 1;
            this.txtDetails.Text = "Exception message will go here for you to see.";
            // 
            // lstStackTrace
            // 
            this.lstStackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstStackTrace.FormattingEnabled = true;
            this.lstStackTrace.IntegralHeight = false;
            this.lstStackTrace.Location = new System.Drawing.Point(12, 91);
            this.lstStackTrace.Name = "lstStackTrace";
            this.lstStackTrace.Size = new System.Drawing.Size(408, 142);
            this.lstStackTrace.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOK,
            this.btnCopy,
            this.btnDetails});
            this.toolStrip1.Location = new System.Drawing.Point(0, 253);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(429, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnOK
            // 
            this.btnOK.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnOK.AutoSize = false;
            this.btnOK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOK.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 22);
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnCopy.AutoSize = false;
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(80, 22);
            this.btnCopy.Text = "Copy";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnDetails
            // 
            this.btnDetails.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnDetails.AutoSize = false;
            this.btnDetails.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDetails.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(80, 22);
            this.btnDetails.Text = "Details";
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // pgDetails
            // 
            this.pgDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pgDetails.HelpVisible = false;
            this.pgDetails.Location = new System.Drawing.Point(176, 25);
            this.pgDetails.Name = "pgDetails";
            this.pgDetails.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.pgDetails.Size = new System.Drawing.Size(244, 208);
            this.pgDetails.TabIndex = 4;
            this.pgDetails.ToolbarVisible = false;
            this.pgDetails.Visible = false;
            // 
            // frmError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 278);
            this.Controls.Add(this.pgDetails);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lstStackTrace);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.lblExType);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(445, 322);
            this.Name = "frmError";
            this.Text = "Error Details";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExType;
        private System.Windows.Forms.TextBox txtDetails;
        private System.Windows.Forms.ListBox lstStackTrace;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnDetails;
        private System.Windows.Forms.ToolStripButton btnOK;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.PropertyGrid pgDetails;
    }
}