using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HASH
{
    /// <summary>
    /// A less flickery ListBox class
    /// </summary>
    public class BufferedListbox : ListBox
    {
        /// <summary>
        /// Constructs a thing to do some stuff (better than the original thing)
        /// </summary>
        public BufferedListbox() {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;

        }

        /// <summary>
        /// This probably doesn't do anything
        /// </summary>
        /// <param name="m">mmmmmmm</param>
        protected override void OnNotifyMessage(Message m) {
            //Filter out the WM_ERASEBKGND message
            if (Program.RunningOnMono || m.Msg != 0x14) {
                base.OnNotifyMessage(m);
            }
        }

        /// <summary>
        /// Re-implements painting from scratch with less flicker
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e) {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);
            if (this.Items.Count > 0) {
                for (int i = 0; i < this.Items.Count; ++i) {
                    System.Drawing.Rectangle irect = this.GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect)) {
                        if ((this.SelectionMode == SelectionMode.One && this.SelectedIndex == i)
                        || (this.SelectionMode == SelectionMode.MultiSimple && this.SelectedIndices.Contains(i))
                        || (this.SelectionMode == SelectionMode.MultiExtended && this.SelectedIndices.Contains(i))) {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Selected, this.ForeColor,
                                this.BackColor));
                        } else {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Default, this.ForeColor,
                                this.BackColor));
                        }
                        iRegion.Complement(irect);
                    }
                }
            }
            base.OnPaint(e);
        }
    }
}
