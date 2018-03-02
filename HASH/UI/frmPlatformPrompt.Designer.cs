namespace HASH
{
    partial class frmPlatformPrompt
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lstPlatforms = new HASH.BufferedListbox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(324, 382);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(405, 382);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Window;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(492, 62);
            this.label1.TabIndex = 3;
            this.label1.Text = "The ROM\'s platform could not be detected automatically.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lstPlatforms
            // 
            this.lstPlatforms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPlatforms.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstPlatforms.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstPlatforms.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstPlatforms.FormattingEnabled = true;
            this.lstPlatforms.IntegralHeight = false;
            this.lstPlatforms.ItemHeight = 50;
            this.lstPlatforms.Location = new System.Drawing.Point(0, 62);
            this.lstPlatforms.Name = "lstPlatforms";
            this.lstPlatforms.Size = new System.Drawing.Size(490, 309);
            this.lstPlatforms.TabIndex = 0;
            this.lstPlatforms.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstPlatforms_DrawItem);
            this.lstPlatforms.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lstPlatforms_MeasureItem);
            this.lstPlatforms.SelectedIndexChanged += new System.EventHandler(this.lstPlatforms_SelectedIndexChanged);
            this.lstPlatforms.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lstPlatforms_MouseMove);
            this.lstPlatforms.MouseLeave += new System.EventHandler(this.lstPlatforms_MouseLeave);
            // 
            // frmPlatformPrompt
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(492, 417);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lstPlatforms);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPlatformPrompt";
            this.ShowIcon = false;
            this.Text = "Select Platform";
            this.ResumeLayout(false);

        }

        #endregion

        private HASH.BufferedListbox lstPlatforms;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
    }
}