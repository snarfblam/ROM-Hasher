using HASH.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HASH
{
    /// <summary>
    /// Lists databases and allows the user to add, remove, or re-configure databases.
    /// </summary>
    public partial class frmDBConfig : Form
    {
        /// <summary>In certain circumstances (after a database is added or edited) the list view item that corresponds to this
        /// entry will be selected by default.</summary>
        DBConfig.Database defaultSelection;

        
        public frmDBConfig() {
            InitializeComponent();

            PopulateList();
        }

        private void PopulateList() {
            lstDbs.BeginUpdate();
            lstDbs.Items.Clear();

            foreach (var db in Program.DatabaseConfig.Databases) {
                ListViewItem newItem = new ListViewItem(db.Name);
                newItem.SubItems.Add(Path.GetFileName(db.Filename));
                newItem.SubItems.Add(GetPlatformString(db));
                newItem.SubItems.Add(db.Format.GetDescription());
                newItem.Tag = db;

                lstDbs.Items.Add(newItem);
            }
            lstDbs.EndUpdate();
        }

        private string GetPlatformString(DBConfig.Database db) {
            if (db.Platforms.Count == 0){
                return (db.MiscPlatforms.Count > 0) ? "{misc}" : "{none}";
            } else if (db.Platforms.Count == 1) {
                return db.Platforms[0].ToString() + ((db.MiscPlatforms.Count > 0) ? " {more}" : string.Empty);
            } else if (db.Platforms.Count > 1 || db.MiscPlatforms.Count > 0) {
                return db.Platforms[0].ToString() + " {more}";
            } else {
                return "{none}";
            }
        }

        private void listView1_SizeChanged(object sender, EventArgs e) {
            lstDbs.Columns[lstDbs.Columns.Count - 1].Width = -2;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            btnEdit.Enabled = btnRemove.Enabled = (lstDbs.SelectedItems.Count > 0);
        }

        private void frmDBConfig_Activated(object sender, EventArgs e) {
            toolStrip1.Enabled = true;
        }

        private void frmDBConfig_Deactivate(object sender, EventArgs e) {
            toolStrip1.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e) {
            if (dbOpenDlg.ShowDialog(this) == DialogResult.OK) {
                string filepath = dbOpenDlg.FileName;

                defaultSelection = null;


                AddDatabase(filepath);
            }
            toolStrip1.DeselectButtons();

        }

        private void AddDatabase(string filepath) {
            string filename = Path.GetFileName(filepath);
            string newFilePath = Path.Combine(FileSystem.DbPath, filename);

            // If the DB is already loaded, let user reconfigure
            var fileAlreadyLoaded = CheckFileLoaded(filepath);
            if (fileAlreadyLoaded != null) {
                if (MessageBox.Show("This database is already loaded. Would you like to configure it?", "Database", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    EditDatabase(fileAlreadyLoaded);
                }
                return;
            }

            // If the DB is not loaded, but already in DB folder, we don't need to copy
            bool inDbFolder = CheckSameFile(filepath, newFilePath);
            if (!inDbFolder) {
                // Don't overwrite existing file
                if (File.Exists(newFilePath)) {
                    if (MessageBox.Show("A database with the same filename is already exists. The new database file will be renamed.", "Database Notice", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                        newFilePath = FileSystem.GetUniqueFilename(newFilePath);
                    } else {
                        return;
                    }
                }

                File.Copy(filepath, newFilePath);
            }

            // Set up some reasonable defaults
            var newDbEntry = new DBConfig.Database();
            newDbEntry.Format = DBFormat.ClrMamePro;
            newDbEntry.Filename = FileSystem.MakePathRelative(FileSystem.DbPath, newFilePath);

            DialogResult result = DBEdit.EditEntry(newDbEntry);

            // If the user cancels, and the file was copied to DB folder, delete the folder.
            if (result == DialogResult.Cancel && !inDbFolder) {
                FileSystem.IgnoreFileErrors(() => File.Delete(newFilePath));
            } else {
                Program.DatabaseConfig.Databases.Add(newDbEntry);
                defaultSelection = newDbEntry;
            }

            // Re-populate list
            PopulateList();
            MakeDefaultSelection();
            return;
        }

        private void MakeDefaultSelection() {
            lstDbs.SelectedItems.Clear();
            for (int i = 0; i < lstDbs.Items.Count; i++) {
                if (lstDbs.Items[i].Tag == defaultSelection) {
                    lstDbs.SelectedIndices.Add(i);
                }
            }
        }




        /// <summary>
        /// Returns a database object if the specified database file is already loaded
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private DBConfig.Database CheckFileLoaded(string filepath) {
            for (int i = 0; i < Program.DatabaseConfig.Databases.Count; i++) {
                var db = Program.DatabaseConfig.Databases[i];

                
                if (CheckSameFile(filepath, db.Filename)) return db;
                if (!Path.IsPathRooted(db.Filename)) {
                    string dbAbs = Path.Combine(FileSystem.DbPath, db.Filename);
                    if (CheckSameFile(filepath, dbAbs)) return db;
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if the specified path points to a location within the specified folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        /// <remarks>There are some scenarios where this check will fail to identify contained paths</remarks>
        private bool CheckInFolder(string folder, string filepath) {
            string fileFolder = Path.GetDirectoryName(filepath);
            folder = Path.GetFullPath(folder);
            //Todo: need to address possibility of case-sensitive file systems
            return folder.Equals(fileFolder, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines if the two paths point to the same file
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <remarks>There are some scenarios where this check will fail to identify same files</remarks>
        private bool CheckSameFile(string a, string b) {
            //Todo: need to address possibility of case-sensitive file systems
            a = Path.GetFullPath(a);
            b = Path.GetFullPath(b);

            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        private void btnEdit_Click(object sender, EventArgs e) {
            if (lstDbs.SelectedItems.Count == 1) {
                var selection = lstDbs.SelectedItems[0];

                EditDatabase(selection.Tag as DBConfig.Database);
            }
            toolStrip1.DeselectButtons();
        }

        private void EditDatabase(DBConfig.Database db) {
            defaultSelection = db;
            if (db != null) {
                DialogResult result = DBEdit.EditEntry(db);

                if (result == DialogResult.OK) {
                    PopulateList();
                    MakeDefaultSelection();
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e) {
            if (lstDbs.SelectedItems.Count == 1) {
                var selection = lstDbs.SelectedItems[0];

                if (MessageBox.Show("Remove entry and delete database from cache?", "Delete Database", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    RemoveDatabase(selection.Tag as DBConfig.Database);
                }
            }

            defaultSelection = null;
            PopulateList();
            toolStrip1.DeselectButtons();
        }

        private void RemoveDatabase(DBConfig.Database database) {
            Program.DatabaseConfig.Databases.Remove(database);

            string filepath = FileSystem.GetDbFilePath(database);
            FsError error;

            FileSystem.PerformFileAction(() => File.Delete(filepath), out error);
            if (error != null) {
                MessageBox.Show("An error occurred deleting the cached database file.");
            }
        }

    }
}
