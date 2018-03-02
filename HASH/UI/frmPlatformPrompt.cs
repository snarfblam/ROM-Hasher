using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace HASH
{
    /// <summary>
    /// Prompts user to select a platform from the list of supported and custom platforms
    /// </summary>
    public partial class frmPlatformPrompt : Form
    {
        List<Image> PlatformImages = new List<Image>();
        List<HASH.Platforms> PlatformIDs = new List<HASH.Platforms>();
        List<string> PlatformNames = new List<string>();

        List<string> MiscPlatforms = new List<string>();
        public frmPlatformPrompt() {
            InitializeComponent();

            EnumEnum(PlatformIDs, PlatformNames);

            for (int i = 0; i < PlatformNames.Count; i++) {
                var platform = Platform.GetAssociatedPlatform(PlatformIDs[i]);

                if (PlatformIDs[i] == Platforms.Unknown) {
                    PlatformNames[i] = "Other";
                } else {
                    PlatformNames[i] = platform.ID.GetDescription();
                }
                PlatformImages.Add(platform.SmallPlatformImage);

                lstPlatforms.Items.Add(PlatformNames[i]);
            }
            for (int i = 0; i < RomDB.AllMiscPlatforms.Count; i++) {
                MiscPlatforms.Add(RomDB.AllMiscPlatforms[i]);
                lstPlatforms.Items.Add(MiscPlatforms[i]);
            }


        }


        static void EnumEnum<T>(List<T> enumValues, List<string> enumNames) {
            enumValues.Clear();
            enumNames.Clear();

            System.Collections.IList values = Enum.GetValues(typeof(T));
            for (int i = 0; i < values.Count; i++) {
                T value = (T)values[i];
                //if ((int)(object)value != 0) {
                enumValues.Add(value);
                enumNames.Add(((Enum)(object)value).GetDescription());
                //}
            }
        }
        private void lstPlatforms_MeasureItem(object sender, MeasureItemEventArgs e) {
            e.ItemWidth = lstPlatforms.Width;
            e.ItemHeight = 50;
        }

        public Platforms SelectedPlatform { get; private set; }
        public string SelectedMiscPlatform { get; private set; }

        private void lstPlatforms_DrawItem(object sender, DrawItemEventArgs e) {
            Image image; string name;

            bool selected = (e.State & DrawItemState.Selected) != 0;
            bool hot = (e.Index == hotTrackedIndex);
            bool isMiscPlatform = e.Index >= PlatformNames.Count;

            if (isMiscPlatform) {
                name = RomDB.AllMiscPlatforms[e.Index - PlatformNames.Count];
                image = null;
            } else {
                name = PlatformNames[e.Index];
                image = PlatformImages[e.Index];
            }

            using (Brush b = new SolidBrush(lstPlatforms.BackColor)) {
                e.Graphics.FillRectangle(b, e.Bounds);
            }
            if (selected) {
                int opacity = hot ? 160 : 128;
                using (Brush b = new SolidBrush(Color.FromArgb(opacity, SystemColors.Highlight))) {
                    e.Graphics.FillRectangle(b, e.Bounds);
                }
                using (LinearGradientBrush b = new LinearGradientBrush(new Point(0, e.Bounds.Top + 20), new Point(0, e.Bounds.Top + 70), Color.FromArgb(128, 255, 255, 255), Color.Transparent)) {
                    var gradientRect = e.Bounds;
                    gradientRect.Offset(0, 0);
                    gradientRect.Height -= 0;
                    e.Graphics.FillRectangle(b, gradientRect);

                }

                e.Graphics.DrawString(name, lstPlatforms.Font, SystemBrushes.ControlText, e.Bounds.Left + 165, e.Bounds.Y + 10);

                Rectangle boundsMinusOne = e.Bounds;
                boundsMinusOne.Width -= 1; boundsMinusOne.Height -= 1;
                e.Graphics.DrawRectangle(SystemPens.Highlight, boundsMinusOne);
            } else if (hot) {
                using (Brush b = new SolidBrush(Color.FromArgb(32, SystemColors.Highlight))) {
                    e.Graphics.FillRectangle(b, e.Bounds);
                }
                e.Graphics.DrawString(name, lstPlatforms.Font, SystemBrushes.ControlText, e.Bounds.Left + 165, e.Bounds.Y + 10);

                Rectangle boundsMinusOne = e.Bounds;
                boundsMinusOne.Width -= 1; boundsMinusOne.Height -= 1;
                //e.Graphics.DrawRectangle(SystemPens.Highlight, boundsMinusOne);
            } else {

                e.Graphics.DrawString(name, lstPlatforms.Font, SystemBrushes.ControlText, e.Bounds.Left + 165, e.Bounds.Y + 10);
            }
            if (image != null) {
                e.Graphics.DrawImage(image, new Rectangle(e.Bounds.Left, e.Bounds.Top, 150, 50), new Rectangle(0, 0, 150, 50), GraphicsUnit.Pixel);
            }
        }

        private void lstPlatforms_SelectedIndexChanged(object sender, EventArgs e) {
            if (lstPlatforms.SelectedIndex > -1) {
                if (lstPlatforms.SelectedIndex < PlatformIDs.Count) {
                    // Recognized platform
                    SelectedPlatform = PlatformIDs[lstPlatforms.SelectedIndex];
                    SelectedMiscPlatform = null;
                } else {
                    SelectedPlatform = Platforms.Unknown;
                    SelectedMiscPlatform = MiscPlatforms[lstPlatforms.SelectedIndex - PlatformIDs.Count];
                }
            }
        }

        int hotTrackedIndex = -1;
        private void lstPlatforms_MouseMove(object sender, MouseEventArgs e) {
            int index = lstPlatforms.IndexFromPoint(e.Location);
            if (index != hotTrackedIndex) {
                lstPlatforms.SuspendLayout();
                if (hotTrackedIndex != -1) {
                    var oldRect = lstPlatforms.GetItemRectangle(hotTrackedIndex);
                    lstPlatforms.Invalidate(oldRect);
                }

                hotTrackedIndex = index;
                if (hotTrackedIndex != -1) {
                    var newRect = lstPlatforms.GetItemRectangle(hotTrackedIndex);
                    lstPlatforms.Invalidate(newRect);
                }
                lstPlatforms.ResumeLayout();
            }
        }

        private void lstPlatforms_MouseLeave(object sender, EventArgs e) {
            if (hotTrackedIndex > -1) {
                lstPlatforms.SuspendLayout();
                if (hotTrackedIndex != -1) {
                    var oldRect = lstPlatforms.GetItemRectangle(hotTrackedIndex);
                    lstPlatforms.Invalidate(oldRect);
                }

                hotTrackedIndex = -1;
                if (hotTrackedIndex != -1) {
                    var newRect = lstPlatforms.GetItemRectangle(hotTrackedIndex);
                    lstPlatforms.Invalidate(newRect);
                }
                lstPlatforms.ResumeLayout();

            }
        }

        /// <summary>
        /// Gets/sets the prompt text
        /// </summary>
        public string Prompt { get { return label1.Text; } set { label1.Text = value; } }

        /// <summary>
        /// Asks the user to select a platform
        /// </summary>
        /// <param name="platform">Returns the selected platform, or Platform.UnknownPlatform if the user selects an unsupported platform.</param>
        /// <param name="misc">Returns the name of the selected platform if it does not have built-in support.</param>
        /// <returns>True if a platform was selected, otherwise false.</returns>
        internal static bool GetPlatform(out Platforms platform, out string misc) {
            using (var frm = new frmPlatformPrompt()) {
                var result = frm.ShowDialog();
                if (result == DialogResult.OK) {
                    platform = frm.SelectedPlatform;
                    misc = frm.SelectedMiscPlatform;
                    return true;
                } else {
                    platform = Platforms.Unknown;
                    misc = null;
                    return false;
                }
            }

        }
        /// <summary>
        /// Asks the user to select a platform
        /// </summary>
        /// <param name="platform">Returns the selected platform, or Platform.UnknownPlatform if the user selects an unsupported platform.</param>
        /// <param name="misc">Returns the name of the selected platform if it does not have built-in support.</param>
        /// <param name="prompt">Prompt text shown at top of form</param>
        /// <param name="caption">Window caption</param>
        /// <returns>True if a platform was selected, otherwise false.</returns>
        internal static bool GetPlatform(out Platforms platform, out string misc, string prompt, string caption) {
            using (var frm = new frmPlatformPrompt()) {
                frm.Prompt = prompt;
                frm.Text = caption;

                var result = frm.ShowDialog();
                if (result == DialogResult.OK){
                    platform = frm.SelectedPlatform;
                    misc = frm.SelectedMiscPlatform;
                    return true;
                } else {
                    platform = Platforms.Unknown;
                    misc = null;
                    return false;
                }
            }

        }
        private void btnOK_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
        }
    }
}