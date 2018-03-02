namespace HASH
{
    partial class DBEdit
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lstPlatforms = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lstMiscPlatforms = new System.Windows.Forms.ListBox();
            this.txtNewPlatform = new System.Windows.Forms.TextBox();
            this.btnAddPlatform = new System.Windows.Forms.Button();
            this.btnRemovePlatform = new System.Windows.Forms.Button();
            this.lstHints = new System.Windows.Forms.CheckedListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOK = new System.Windows.Forms.ToolStripButton();
            this.btnCancel = new System.Windows.Forms.ToolStripButton();
            this.btnDefaultOK = new System.Windows.Forms.Button();
            this.cboFormat = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSecretCancel = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filename";
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(105, 65);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(379, 20);
            this.txtPath.TabIndex = 2;
            // 
            // lstPlatforms
            // 
            this.lstPlatforms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstPlatforms.CheckOnClick = true;
            this.lstPlatforms.FormattingEnabled = true;
            this.lstPlatforms.IntegralHeight = false;
            this.lstPlatforms.Location = new System.Drawing.Point(12, 106);
            this.lstPlatforms.Name = "lstPlatforms";
            this.lstPlatforms.Size = new System.Drawing.Size(184, 198);
            this.lstPlatforms.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Platforms";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(199, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Misc Platforms";
            // 
            // lstMiscPlatforms
            // 
            this.lstMiscPlatforms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstMiscPlatforms.FormattingEnabled = true;
            this.lstMiscPlatforms.IntegralHeight = false;
            this.lstMiscPlatforms.Location = new System.Drawing.Point(202, 161);
            this.lstMiscPlatforms.Name = "lstMiscPlatforms";
            this.lstMiscPlatforms.Size = new System.Drawing.Size(120, 143);
            this.lstMiscPlatforms.TabIndex = 7;
            // 
            // txtNewPlatform
            // 
            this.txtNewPlatform.Location = new System.Drawing.Point(202, 106);
            this.txtNewPlatform.Name = "txtNewPlatform";
            this.txtNewPlatform.Size = new System.Drawing.Size(120, 20);
            this.txtNewPlatform.TabIndex = 4;
            // 
            // btnAddPlatform
            // 
            this.btnAddPlatform.Location = new System.Drawing.Point(202, 132);
            this.btnAddPlatform.Name = "btnAddPlatform";
            this.btnAddPlatform.Size = new System.Drawing.Size(48, 23);
            this.btnAddPlatform.TabIndex = 5;
            this.btnAddPlatform.Text = "Add";
            this.btnAddPlatform.UseVisualStyleBackColor = true;
            this.btnAddPlatform.Click += new System.EventHandler(this.btnAddPlatform_Click);
            // 
            // btnRemovePlatform
            // 
            this.btnRemovePlatform.Location = new System.Drawing.Point(256, 132);
            this.btnRemovePlatform.Name = "btnRemovePlatform";
            this.btnRemovePlatform.Size = new System.Drawing.Size(66, 23);
            this.btnRemovePlatform.TabIndex = 6;
            this.btnRemovePlatform.Text = "Remove";
            this.btnRemovePlatform.UseVisualStyleBackColor = true;
            this.btnRemovePlatform.Click += new System.EventHandler(this.btnRemovePlatform_Click);
            // 
            // lstHints
            // 
            this.lstHints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstHints.CheckOnClick = true;
            this.lstHints.FormattingEnabled = true;
            this.lstHints.IntegralHeight = false;
            this.lstHints.Location = new System.Drawing.Point(328, 106);
            this.lstHints.Name = "lstHints";
            this.lstHints.Size = new System.Drawing.Size(153, 198);
            this.lstHints.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(325, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Hints";
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOK,
            this.btnCancel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 307);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.ShowItemToolTips = false;
            this.toolStrip1.Size = new System.Drawing.Size(493, 25);
            this.toolStrip1.TabIndex = 9;
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
            // btnCancel
            // 
            this.btnCancel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnCancel.AutoSize = false;
            this.btnCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 22);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDefaultOK
            // 
            this.btnDefaultOK.Location = new System.Drawing.Point(63, 129);
            this.btnDefaultOK.Name = "btnDefaultOK";
            this.btnDefaultOK.Size = new System.Drawing.Size(75, 23);
            this.btnDefaultOK.TabIndex = 12;
            this.btnDefaultOK.Text = "button3";
            this.btnDefaultOK.UseVisualStyleBackColor = true;
            this.btnDefaultOK.Click += new System.EventHandler(this.btnDefaultOK_Click);
            // 
            // cboFormat
            // 
            this.cboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFormat.FormattingEnabled = true;
            this.cboFormat.Location = new System.Drawing.Point(105, 38);
            this.cboFormat.Name = "cboFormat";
            this.cboFormat.Size = new System.Drawing.Size(379, 21);
            this.cboFormat.TabIndex = 1;
            this.cboFormat.SelectedIndexChanged += new System.EventHandler(this.cboFormat_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Format";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(105, 12);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(379, 20);
            this.txtName.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Name";
            // 
            // btnSecretCancel
            // 
            this.btnSecretCancel.Location = new System.Drawing.Point(80, 161);
            this.btnSecretCancel.Name = "btnSecretCancel";
            this.btnSecretCancel.Size = new System.Drawing.Size(75, 23);
            this.btnSecretCancel.TabIndex = 16;
            this.btnSecretCancel.Text = "secret cancel button";
            this.btnSecretCancel.UseVisualStyleBackColor = true;
            // 
            // DBEdit
            // 
            this.AcceptButton = this.btnDefaultOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnSecretCancel;
            this.ClientSize = new System.Drawing.Size(493, 332);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lstHints);
            this.Controls.Add(this.btnRemovePlatform);
            this.Controls.Add(this.btnAddPlatform);
            this.Controls.Add(this.txtNewPlatform);
            this.Controls.Add(this.lstMiscPlatforms);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.cboFormat);
            this.Controls.Add(this.lstPlatforms);
            this.Controls.Add(this.btnSecretCancel);
            this.Controls.Add(this.btnDefaultOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.MinimizeBox = false;
            this.Name = "DBEdit";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Database";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.CheckedListBox lstPlatforms;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lstMiscPlatforms;
        private System.Windows.Forms.TextBox txtNewPlatform;
        private System.Windows.Forms.Button btnAddPlatform;
        private System.Windows.Forms.Button btnRemovePlatform;
        private System.Windows.Forms.CheckedListBox lstHints;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnOK;
        private System.Windows.Forms.ToolStripButton btnCancel;
        private System.Windows.Forms.Button btnDefaultOK;
        private System.Windows.Forms.ComboBox cboFormat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSecretCancel;
    }
}