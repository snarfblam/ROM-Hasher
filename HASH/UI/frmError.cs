using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HASH
{
    /// <summary>
    /// Displays exception details to users that can be reported.
    /// </summary>
    public partial class frmError : Form
    {
        public frmError() {
            InitializeComponent();
        }
        Exception x;

        /// <summary>
        /// Initializes display with the specified exception.
        /// </summary>
        /// <param name="ex">Exception to display</param>
        public void SetError(Exception ex) {
            lblExType.Text = ex.GetType().ToString();
            txtDetails.Text = ex.Message;
            lstStackTrace.Items.AddRange(ex.StackTrace.Split(new string[]{Environment.NewLine, "\r","\n"}, StringSplitOptions.None));
            pgDetails.SelectedObject = ex;

            this.x = ex;
        }

        private void btnDetails_Click(object sender, EventArgs e) {
            btnDetails.Checked = !btnDetails.Checked;

            if (btnDetails.Checked) {
                pgDetails.Visible = true;
                txtDetails.Width = pgDetails.Left - txtDetails.Left - 8;
                lstStackTrace.Width = txtDetails.Width;
            } else {
                pgDetails.Visible = false;
                txtDetails.Width = ClientRectangle.Width - txtDetails.Left - 8;
                lstStackTrace.Width = txtDetails.Width;
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            Close();
        }

        private void btnCopy_Click(object sender, EventArgs e) {
            Clipboard.SetText(GetExceptionText(x));
        }

        private string GetExceptionText(Exception x) {
            string nl = Environment.NewLine;
            string result = x.GetType().ToString() + nl
                + x.Message + nl
                + "Source: " + x.Source + nl
                + nl
                + x.StackTrace + nl + nl;

            if (x.InnerException != null) {
                result +=
                    "---- Inner Exception ---- " + nl
                    + nl
                    + GetExceptionText(x.InnerException);
            }

            return result;
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
