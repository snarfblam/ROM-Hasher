using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HASH
{
    
    public partial class frmBusy : Form
    {
        /// <summary>
        /// UI element that displays an animation while the program works.
        /// </summary>
        public frmBusy() {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Escape) {
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
