using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using Romulus;

namespace HASH
{
    /// <summary>
    /// Primary ROM Hasher UI
    /// </summary>
    public partial class HashForm : Form
    {
        /// <summary>
        /// Instantiates main form
        /// </summary>
        public HashForm() {
            InitializeComponent();

            // Init UI from program config
            btnSkipHashes.Checked = Program.Config.SkipExtraHashes;

            // Initialize "Load ROM" link
            lblFile.LinkArea = new LinkArea(0, 10);

            // Initialize details UI manager
            lists.Host = tbDetails;
            lists.ViewAdded += new EventHandler<ViewEventArgs>(lists_ViewAdded);
            lists.ViewRemoved += new EventHandler<ViewEventArgs>(lists_ViewRemoved);
            const string consolas = "Consolas";

            // Prefer Consolas, otherwise use default monospace
            // Yes, I know no one cares about this but me
            txtRHDN.Font = new Font(consolas, txtRHDN.Font.Size, FontStyle.Regular);
            if (!txtRHDN.Font.Name.Equals(consolas, StringComparison.InvariantCultureIgnoreCase)) {
                txtRHDN.Font = new Font(FontFamily.GenericMonospace, txtRHDN.Font.Size, FontStyle.Regular);
            }

        }

        /// <summary>Path of ROM being displayed</summary>
        string PrimaryRomPath;
        /// <summary>Filename of ROM being dispalyed</summary>
        string PrimaryRomFilename;
        /// <summary>Path of ROM being loaded</summary>
        string pending_PrimaryRomPath;
        /// <summary>Filename of ROM being loaded</summary>
        string pending_PimaryRomFilename;
        /// <summary>RomData for rom being displayed</summary>
        RomData PrimaryRom;

        /// <summary>Path of secondary ROM being displayed</summary>
        string SecondaryRomPath;
        /// <summary>Filename of secondary ROM being displayed</summary>
        string SecondaryRomFilename;
        /// <summary>Path of secondary ROM being loaded</summary>
        string pending_SecondaryRomPath;
        /// <summary>Filename of secondary ROM being loaded</summary>
        string pending_SecondaryRomFilename;
        /// <summary>Secondary ROM being dispalyed</summary>
        RomData SecondaryRom;

        /// <summary>Set to true while a file-opening operation is in progress. This value should be examined before
        /// attempting to initiate a file-open operation</summary>
        bool busyHashing = false;

        #region Display lists
        // Manages UI for "details" display on bottom

        ListManager lists = new ListManager();

        void lists_ViewRemoved(object sender, ViewEventArgs e) {
            e.View.ContextMenuStrip = null;
        }

        void lists_ViewAdded(object sender, ViewEventArgs e) {
            e.View.ContextMenuStrip = mnuDetails;
            var view = e.View as ListView;
            if (view != null) {
                view.MultiSelect = true;
            }
        }

        #endregion

        /// <summary>Begins a file open operation</summary>
        /// <param name="path">Path to the ROM</param>
        /// <param name="promptForPlatform">True to propmpt the user to select a plaptfoprm. P.</param>
        private void LoadPrimaryROM(string path, bool promptForPlatform) {
            pending_PrimaryRomPath = path;
            pending_PimaryRomFilename = Path.GetFileName(path);

            HashJob hj = new HashJob(this, path, promptForPlatform, false);
            hj.WorkComplete += new EventHandler(primaryHasher_WorkComplete);
            hj.WorkAborted += new EventHandler(primaryHasher_Aborted);
            EnterBusyMode();
            hj.DoWork();

        }

        void primaryHasher_Aborted(object sender, EventArgs e) {
            var hj = ((HashJob)sender);
            hj.WorkComplete -= primaryHasher_WorkComplete;
            hj.WorkAborted -= primaryHasher_Aborted;

            ExitBusyMode();
        }

        void primaryHasher_WorkComplete(object sender, EventArgs e) {
            var hj = ((HashJob)sender);
            hj.WorkComplete -= primaryHasher_WorkComplete;
            hj.WorkAborted -= primaryHasher_Aborted;

            if (this.IsDisposed) return;

            if (hj.Complete) {
                PrimaryRom = hj.Result;
                PrimaryRomPath = pending_PrimaryRomPath;
                PrimaryRomFilename = pending_PimaryRomFilename;

                UpdateRomDisplay(true);
            }

            ExitBusyMode();
        }

        /// <summary>
        /// Begins a file open operation for a "patched" rom
        /// </summary>
        /// <param name="path"></param>
        /// <param name="promptForPlatform">If true, the user will be prompted to manually select a platform.</param>
        private void LoadSecondaryRom(string path, bool promptForPlatform) {
            pending_SecondaryRomPath = path;
            pending_SecondaryRomFilename = Path.GetFileName(path);

            HashJob hj = new HashJob(this, path, promptForPlatform, true);
            hj.WorkComplete += new EventHandler(secondaryHasher_WorkComplete);
            hj.WorkAborted += new EventHandler(secondaryHasher_Aborted);
            EnterBusyMode();
            hj.DoWork();

        }

        void secondaryHasher_Aborted(object sender, EventArgs e) {
            var hj = ((HashJob)sender);
            hj.WorkComplete -= secondaryHasher_WorkComplete;
            hj.WorkAborted -= secondaryHasher_Aborted;

            ExitBusyMode();
        }

        void secondaryHasher_WorkComplete(object sender, EventArgs e) {
            var hj = ((HashJob)sender);
            hj.WorkComplete -= secondaryHasher_WorkComplete;
            hj.WorkAborted -= secondaryHasher_Aborted;

            if (this.IsDisposed) return;

            if (hj.Complete) {
                SecondaryRom = hj.Result;
                SecondaryRomPath = pending_SecondaryRomPath;
                SecondaryRomFilename = pending_SecondaryRomFilename;

                UpdateRomDisplay(false);
            }

            ExitBusyMode();
        }

        /// <summary>
        /// Places the UI in "busy mode". ROMs can not be loaded and the busy cursor is removed.
        /// </summary>
        private void EnterBusyMode() {
            busyHashing = true;
            toolStrip1.Enabled = false;
            UseWaitCursor = true;
        }

        /// <summary>
        /// Removes UI from "busy mode"
        /// </summary>
        private void ExitBusyMode() {
            busyHashing = false;
            toolStrip1.Enabled = true;
            UseWaitCursor = false;
        }

        /// <summary>Stores cached font for bolded items in details</summary>
        Font romDisplay_BoldFont;
        /// <summary>Stores cached font for items in details</summary>
        Font romDisplay_NormalFont;
        /// <summary>
        /// Updates the appearance of the file boxes, summary, and optionally the detail lists
        /// </summary>
        private void UpdateRomDisplay(bool updateDetails) {
            UpdateRomSummary();

            SuspendLayout();


            if (PrimaryRom != null) {
                DottedFileBorder = false;
                lblFile.Text = PrimaryRomFilename; // +Environment.NewLine + rom.Length.ToString() + " ($" + rom.Length.ToString("x") + ") bytes";
                if ((lblFile.Font.Style & FontStyle.Bold) != FontStyle.Bold) {
                    lblFile.Font = new Font(lblFile.Font, FontStyle.Bold);
                }
                lblFile.LinkArea = new LinkArea(0, 0);
                lblFile.TextAlign = ContentAlignment.TopCenter;
                if (PrimaryRom.Platform != null) {
                    lblFile.Image = PrimaryRom.Platform.SmallPlatformImage;
                } else {
                    lblFile.Image = null;
                }
                lblFile.ImageAlign = ContentAlignment.BottomCenter;
            }

            if (SecondaryRom == null) {
                DottedSecondaryFileBorder = true;
                lblSecondaryFile.Text = lblSecondaryFile.Tag.ToString();
                lblSecondaryFile.LinkArea = new LinkArea(0, lblSecondaryFile.Text.Length);
                if (romDisplay_NormalFont == null) 
                    romDisplay_NormalFont = new Font(lblSecondaryFile.Font, FontStyle.Regular);
                lblSecondaryFile.Font = romDisplay_NormalFont;
            } else {
                DottedSecondaryFileBorder = false;
                lblSecondaryFile.Text = SecondaryRomFilename;
                lblSecondaryFile.LinkArea = new LinkArea(0, 0);
                if (romDisplay_BoldFont == null)
                    romDisplay_BoldFont = new Font(lblSecondaryFile.Font, FontStyle.Bold);
                lblSecondaryFile.Font = romDisplay_BoldFont;
            }

            if (updateDetails) {
                lists.ClearLists();
                lists.AddLists(PrimaryRom.ExtendedData);

            }
            ResumeLayout();

            pnlFile.Invalidate();
        }

        /// <summary>
        /// Updates the ROM summary if a primary ROM is loaded
        /// </summary>
        private void UpdateRomSummary() {
            // Summary (only shown if a primary ROM is loaded)
            if (PrimaryRom != null) {
                StringBuilder summary = new StringBuilder();

                // List DB Matches
                var EntryMatches = PrimaryRom.DatabaseMatches;

                string preferredDB = Program.Config.PreferredDatabaseName;
                bool preferredDbNotFound = true;
                for (int iMatch = 0; iMatch < EntryMatches.Count; iMatch++) {
                    string dbName = EntryMatches[iMatch].Database.Name ?? "upenfeawnfwf;nfnfepfepoiefjifewjnfewnjfewpeafwiop"; 
                    bool isPreferredDb = dbName.Equals(preferredDB, StringComparison.InvariantCultureIgnoreCase);

                    // First No-Intro match is considered canonical
                    if (isPreferredDb & preferredDbNotFound) {
                        //summary.AppendLine(preferredDB + " Name: " + EntryMatches[iMatch].Entry.name);
                        summary.AppendLine("Database match: " + EntryMatches[iMatch].Entry.name);
                        var platforms = String.Join(", ", EntryMatches[iMatch].Database.Platforms.ConvertAll<string>(plat => plat.ToString()).ToArray());
                        summary.AppendLine("Database: " + EntryMatches[iMatch].Database.Name + ": " + platforms + " (v. " +  EntryMatches[iMatch].Database.Version + ")");

                        preferredDbNotFound = false;
                    } else {
                        if (isPreferredDb) {
                            summary.AppendLine("Additional " + preferredDB + " Name: " + EntryMatches[iMatch].Entry.name);
                        } else {
                            summary.AppendLine(EntryMatches[iMatch].Database.Name + " Name: " + EntryMatches[iMatch].Entry.name);
                        }
                    }

                    // DB Version
                    summary.AppendLine(GetDbVersionString(EntryMatches[iMatch].Database));

                }
                if (preferredDbNotFound) {
                    summary.AppendLine("ROM not found in Database: " + preferredDB);
                }


                // Primary ROM hashes
                List<RomHash> hashes = new List<RomHash>();
                var desiredHashes = Program.Config.RhdnPreferredHashes;
                for (int i = 0; i < desiredHashes.Count; i++) {
                    var hash = PrimaryRom.GetHash(desiredHashes[i]);
                    if (hash != null) hashes.Add(hash);
                }
                AddHashesToSB(summary, hashes, null);

                // Secondary ROM hashes
                if (SecondaryRom != null) {
                    hashes.Clear();
                    for (int i = 0; i < desiredHashes.Count; i++) {
                        var hash = SecondaryRom.GetHash(desiredHashes[i]);
                        if (hash != null) hashes.Add(hash);
                    }

                    AddHashesToSB(summary, hashes, "Patched");
                }


                txtRHDN.Text = summary.ToString();
            }
        }

        /// <summary>
        /// Gets friendly display string for DB version
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static string GetDbVersionString(RomDB db) {
            if (string.IsNullOrEmpty(db.Version)) {
                return "(" + db.Name + " version unknown)";
            }
            return "(" + db.Name + " version  " + db.Version + ")";
        }

        /// <summary>
        /// Adds hashes to specified string builder for summary box. SB.
        /// </summary>
        /// <param name="hashes">A list of hashes to add. This list will be destroyed.</param>
        /// <param name="prefix">A string to prefix to hash name, e.g. "Patched" -> "Patched File MD5:..."</param>
        /// <param name="sb"></param>
        private void AddHashesToSB(StringBuilder sb, List<RomHash> hashes, string prefix) {
            // Why do I make things more complicated than they need to be?

            if (string.IsNullOrEmpty(prefix)) {
                prefix = string.Empty;
            } else {
                prefix += " ";
            }

            while (hashes.Count > 0) {
                // Grab last hash
                var hash = hashes[0];
                hashes.RemoveAt(0);

                // For ROM/FILE hashes, if two are the same, we want to report them as one 
                // Example: a headerless SNES ROM will have identical file and ROM hashes because the file is a plain ROM image, 
                // so we can report it as a "File/ROM SHA-1" rather than as two seprate hashes with identical values

                // Get the desired type of hash we want to check, e.g. for ROM SHA-1, we want to see if File SHA-1 is the same.
                HashFlags altHashType = 0;
                if (hash.Type.GetContents() == HashFlags.FileHash) {
                    altHashType = HashFlags.RomHash | hash.Type.GetAlgorithm();
                } else if (hash.Type.GetContents() == HashFlags.RomHash) {
                    altHashType = HashFlags.FileHash | hash.Type.GetAlgorithm();
                }

                RomHash altHash = null;
                if (altHashType != 0) {
                    for (int i = 0; i < hashes.Count; i++) {
                        // Find the right hash, and check its value
                        if (hashes[i].Type == altHashType && CompareByteArrays(hash.Value, hashes[i].Value)) {
                            // Match? Cool beans
                            altHash = hashes[i];
                            hashes.RemoveAt(i);
                            break; // for
                        }
                    }
                }

                var hashType = hash.Type;
                if (altHash != null) hashType |= altHash.Type;
                string hashname = RomHash.GetHashName(hashType);
                sb.AppendLine(prefix + hashname + ": " + Hex.FormatHex(hash.Value));
                
            }
        }

        /// <summary>
        /// Attempts to find the specified hash and append it to the string builder for RHDN-specific output.
        /// </summary>
        /// <param name="hashtype">The hash type (i.e. CRC32/MD5/SHA1). File/ROM/PRG/CHR should NOT be specified.</param>
        /// <returns>True if ANYTHING was appended to the string builder.</returns>
        /// <param name="rom"></param>
        /// <param name="sb"></param>
        private static bool AddHashToSB(StringBuilder sb, RomData rom, HashFlags hashtype) {
            string hashname = RomHash.GetHashName(HashFlags.RomHash | hashtype);
            var hash = rom.GetHash(HashFlags.RomHash | hashtype);

            if (hash == null) {
                hashname = hashname = RomHash.GetHashName(HashFlags.FileHash | hashtype); ;
                hash = rom.GetHash(HashFlags.FileHash | hashtype);
            }

            if (hash == null) return false;

            sb.AppendLine(hashname + ": " + Hex.FormatHex(hash.Value));
            return true;
        }

        /// <summary>
        /// Returns true if the two arrays contain identical data or are both null
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        bool CompareByteArrays(byte[] a, byte[] b) {
            if (a == null) {
                return b == null;
            } else if (b == null) return false;

            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++) {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private void lblFile_Paint(object sender, PaintEventArgs e) {
          
            
        }

        private void lblFile_DragEnter(object sender, DragEventArgs e) {
            if (busyHashing) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void lblFile_DragDrop(object sender, DragEventArgs e) {
            if (busyHashing) return;
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1) {
                    Program.QueueTask(
                        () => LoadPrimaryROM(files[0],false)
                    );
                } else {
                    MessageBox.Show("Multiple files can not be dropped at the same time.");
                }
            }
        }


        private void lblSecondaryFile_DragEnter(object sender, DragEventArgs e) {
            if (busyHashing) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void lblSecondaryFile_DragDrop(object sender, DragEventArgs e) {
            if (busyHashing) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1) {
                    LoadSecondaryRom(files[0], false);
                } else {
                    MessageBox.Show("Multiple files can not be dropped at the same time.");
                }
            }
        }

        private void HashForm_Load(object sender, EventArgs e) {
            // Read-only textboxes revert to SystemColors.Control when they are first shown
            txtRHDN.BackColor = SystemColors.Window;
        }

        private void lblFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (busyHashing) return;

            if (RomOpenDlg.ShowDialog(null) == DialogResult.OK) {
                LoadPrimaryROM(RomOpenDlg.FileName, false);
            }
        }

        private void mnuRHDNboxCopy_Click(object sender, EventArgs e) {
            CopyRhndBox();
        }

        /// <summary>
        /// Copes the entire contents of the summary box to the clipboard, and
        /// selects the text and focuses the control to provide a UI cue to
        /// user indicating whole text was copied.
        /// </summary>
        private void CopyRhndBox() {
            if (!string.IsNullOrEmpty(txtRHDN.Text)) {
                txtRHDN.Focus();
                Application.DoEvents();

                txtRHDN.SelectionStart = 0;
                txtRHDN.SelectionLength = txtRHDN.TextLength;
                Clipboard.SetText(txtRHDN.Text);
            }
        }


        private void copyToolStripButton_Click(object sender, EventArgs e) {
            CopyRhndBox();

        }


        #region File Controls
        bool _dottedFileBorder = true;
        bool DottedFileBorder { get { return _dottedFileBorder; } set { _dottedFileBorder = value; } }

        private void pnlFile_Paint(object sender, PaintEventArgs e)
        {
            using (Pen p = new Pen(SystemColors.ControlDark))
            {
                //p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                if (DottedFileBorder)
                {
                    p.DashPattern = new float[] { 4, 2 };
                }
                e.Graphics.DrawRectangle(p, new Rectangle(0, 0, pnlFile.Width - 1, pnlFile.Height - 1));
            }
        }

        bool _dottedSecondaryFileBorder = true;
        bool DottedSecondaryFileBorder { get { return _dottedSecondaryFileBorder; } set { _dottedSecondaryFileBorder = value; } }

        private void lblSecondaryFile_Paint(object sender, PaintEventArgs e)
        {
            using (Pen p = new Pen(SystemColors.ControlDark))
            {
                //p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                if (DottedSecondaryFileBorder)
                {
                    p.DashPattern = new float[] { 4, 2 };
                }
                e.Graphics.DrawRectangle(p, new Rectangle(0, 0, lblSecondaryFile.Width - 1, lblSecondaryFile.Height - 1));
            }
        }

        bool _TwoFileView = false;
        /// <summary>
        /// Enables the second, smaller file box shown below the main file box.
        /// </summary>
        bool TwoFileView
        {
            get { return _TwoFileView; }
            set {
                if (value != _TwoFileView) {
                    SuspendLayout();

                    if (value) {
                        pnlFile.Height = lblSecondaryFile.Top - pnlFile.Top  - 8;
                    } else {
                        pnlFile.Height = lblSecondaryFile.Bottom - pnlFile.Top;
                    }

                    lblSecondaryFile.Visible = value;
                    lblFile.Height = pnlFile.Height - lblFile.Top;

                    ResumeLayout();

                }

                _TwoFileView = value;
            }
        }
        #endregion

        private void btnDouble_Click(object sender, EventArgs e) {
            if (TwoFileView) {
                if (SecondaryRom != null) {
                    const string message = "The second ROM will be unloaded.";
                    const string title = "Remove second ROM";

                    if (MessageBox.Show(message, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK) {
                        SecondaryRom = null;
                        SecondaryRomFilename = SecondaryRomPath = null;
                        UpdateRomDisplay(false);
                        TwoFileView = false;
                    }

                } else {
                    TwoFileView = false;
                }
            } else {
                TwoFileView = true;
            }

            //btnDoubleROM.Checked = TwoFileView;
        }

        private void lblSecondaryFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (busyHashing) return;

            SecondRomDlg.FilterIndex = RomOpenDlg.FilterIndex;
            if (SecondRomDlg.ShowDialog(null) == DialogResult.OK) {
                LoadSecondaryRom(SecondRomDlg.FileName, false); 
            }
        }

        private void HashForm_Activated(object sender, EventArgs e) {
            toolStrip1.Enabled = !busyHashing;
        }

        private void HashForm_Deactivate(object sender, EventArgs e) {
            toolStrip1.Enabled = false;
        }


        private void openAsToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (TwoFileView == false) {
                TwoFileView = true;
            }
            SecondRomDlg.FilterIndex = RomOpenDlg.FilterIndex;
            if (SecondRomDlg.ShowDialog(null) == DialogResult.OK) {
                LoadSecondaryRom(SecondRomDlg.FileName, true);
            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            if (TwoFileView == false) {
                TwoFileView = true;
            }
            SecondRomDlg.FilterIndex = RomOpenDlg.FilterIndex;
            if (SecondRomDlg.ShowDialog(null) == DialogResult.OK) {
                LoadSecondaryRom(SecondRomDlg.FileName, false);
            }
        }

        private void btnDbCongif_Click(object sender, EventArgs e) {
            Program.ConfigureDatabases();
            toolStrip1.DeselectButtons();

        }


        private void mnuCopyDetail_Click(object sender, EventArgs e) {
            if (lists.ActiveDisplay != null) {

            }
        }

        private void mnuCopyDetailList_Click(object sender, EventArgs e) {
            if (lists.ActiveDisplay != null) {
                if (lists.ActiveDisplay != null) {
                    if (lists.ActiveDisplay.Items.Count > 0) {
                        CopyDetails(lists.ActiveDisplay.Items);
                    }
                }
            }
        }
        private void mnuCopyDetail_DropDownOpening(object sender, EventArgs e) {
            if (lists.ActiveDisplay != null) {
                if (lists.ActiveDisplay.SelectedItems.Count > 0) {
                    CopyDetails(lists.ActiveDisplay.SelectedItems);
                }
            }

        }

        private void CopyDetails(System.Collections.IList list) {
            // Copy entire contents of listview to clipboard

            int longestNameLen = 1;
            int longestDescLen = 1;

            for (int i = 0; i < list.Count; i++) {
                var item = list[i] as ListViewItem;
                if(item != null && item.SubItems.Count > 1){
                    longestNameLen = Math.Max(longestNameLen, item.Text.Length);
                    longestDescLen = Math.Max(longestDescLen, item.SubItems[1].Text.Length);
                }
            }

            int nameColumnSize = longestNameLen + 4;
            int descColumnSize = longestDescLen;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++) {
                var item = list[i] as ListViewItem;
                if (item != null && item.SubItems.Count > 1) {
                    int padAmt = nameColumnSize - item.Text.Length;
                    sb.Append(item.Text);
                    sb.Append(' ', padAmt);

                    padAmt = descColumnSize - item.SubItems[1].Text.Length;
                    sb.Append(item.SubItems[1].Text);
                    sb.Append(' ', padAmt);

                    sb.AppendLine();
                }
            }

            Clipboard.SetText(sb.ToString());
        }

        private void HashForm_Click(object sender, EventArgs e) {

        }


        private void openToolStripButton_Click(object sender, EventArgs e) {
            if (RomOpenDlg.ShowDialog(null) == DialogResult.OK) {
                LoadPrimaryROM(RomOpenDlg.FileName, false);
            }
        }
        private void openAsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (RomOpenDlg.ShowDialog(null) == DialogResult.OK) {
                LoadPrimaryROM(RomOpenDlg.FileName, true);
            }
        }

        private void btnSkipHashes_CheckedChanged(object sender, EventArgs e) {
            Program.Config.SkipExtraHashes = btnSkipHashes.Checked;
        }





    }
}
