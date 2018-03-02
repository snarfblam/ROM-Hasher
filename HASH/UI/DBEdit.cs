using HASH.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HASH
{
    /// <summary>
    /// Defines UI for editing a databse
    /// </summary>
    public partial class DBEdit : Form
    {
        static List<Platforms> platformIDs = new List<Platforms>();
        static List<string> platformNames = new List<string>();

        static List<DBHints> dbHints = new List<DBHints>();
        static List<string> dbHintNames = new List<string>();

        static List<DBFormat> dbFormats = new List<DBFormat>();
        static List<string> dbFormatNames = new List<string>();

        private const uint ECM_FIRST = 0x1500;
        private const uint EM_SETCUEBANNER = ECM_FIRST + 1;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        static void SetWatermark(TextBox box, string text) {
            if (!Program.RunningOnMono) {
                SendMessage(box.Handle, EM_SETCUEBANNER, 0, text);
            }
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            //DialogResult = DialogResult.Cancel;
        }

        static DBEdit() {
            EnumEnum(platformIDs, platformNames);
            EnumEnum(dbHints, dbHintNames);
            EnumEnum(dbFormats, dbFormatNames);


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

        public DBEdit() {
            InitializeComponent();

            for (int i = 0; i < platformNames.Count; i++) {
                lstPlatforms.Items.Add(platformNames[i]);
            }

            for (int i = 0; i < dbHintNames.Count; i++) {
                lstHints.Items.Add(dbHintNames[i]);
            }

            for (int i = 0; i < dbFormatNames.Count; i++) {
                cboFormat.Items.Add(dbFormatNames[i]);
            }


        }

        internal static DialogResult EditEntry(DBConfig.Database dbEntry) {
            using (var editform = new DBEdit()) {
                editform.Database = dbEntry;
                return editform.ShowDialog();
            }
        }

        DBConfig.Database _Database;

        /// <summary>
        /// Gets/sets the Database object to edit
        /// </summary>
        public DBConfig.Database Database {
            get {
                return _Database;
            }
            set {
                _Database = value;
                if (_Database != null) {
                    txtPath.Text = _Database.Filename;
                    cboFormat.SelectedIndex = EnumToInt(_Database.Format);

                    for (int i = 0; i < _Database.Platforms.Count; i++) {
                        int platform = (int)_Database.Platforms[i];
                        if (platform < lstPlatforms.Items.Count) {
                            lstPlatforms.SetItemChecked(platform, true);
                        }
                    }

                    for (int i = 0; i < dbHints.Count; i++) {
                        if ((_Database.Hints & dbHints[i]) != 0) {
                            lstHints.SetItemChecked(i, true);
                        }
                    }

                    for (int i = 0; i < _Database.MiscPlatforms.Count; i++) {
                        lstMiscPlatforms.Items.Add(_Database.MiscPlatforms[i]);
                    }

                    txtName.Text = _Database.Name;
                    if (string.IsNullOrEmpty(_Database.Name)) {
                        SetWatermark(txtName, "Enter a name!");
                    }
                }
            }
        }

        
        /// <summary>
        /// Converts the enum value to its integer equivalent if it is a defined value, or zero
        /// if the value is undefined. Should not be used for bit flags.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int EnumToInt(Enum value) {
            if (Enum.IsDefined(value.GetType(), value))
                return (int)(object)value;
            return 0;
        }


        private void btnDefaultOK_Click(object sender, EventArgs e) {
            ApplyChanges();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e) {
            ApplyChanges();
            DialogResult = DialogResult.OK;
            Close();
        }

        
        private void btnCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void ApplyChanges() {
            if (Database != null) {
                Database.Format = (DBFormat)cboFormat.SelectedIndex;

                Database.Platforms.Clear();
                for (int i = 0; i < lstPlatforms.Items.Count; i++) {
                    if (lstPlatforms.GetItemChecked(i)) {
                        Database.Platforms.Add((Platforms)i);
                    }
                }

                Database.MiscPlatforms.Clear();
                for (int i = 0; i < lstMiscPlatforms.Items.Count; i++) {
                    Database.MiscPlatforms.Add(lstMiscPlatforms.Items[i].ToString());
                }

                DBHints hints = 0;
                for (int i = 0; i < lstHints.Items.Count; i++) {
                    if (lstHints.GetItemChecked(i))
                        hints |= dbHints[i];
                }
                Database.Hints = hints;
                Database.Name = txtName.Text;
            }
        }

        private void btnAddPlatform_Click(object sender, EventArgs e) {
            string name = (txtNewPlatform.Text ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(name)) {
                bool unique = true;
                for (int i = 0; i < lstMiscPlatforms.Items.Count; i++) {
                    string item = lstMiscPlatforms.Items[i].ToString();
                    if (name.Equals(item, StringComparison.OrdinalIgnoreCase))
                        unique = false;
                }

                if (unique) {
                    lstMiscPlatforms.Items.Add(name);
                    txtNewPlatform.Text = string.Empty;
                }
            }
        }

        private void btnRemovePlatform_Click(object sender, EventArgs e) {
            if (lstMiscPlatforms.SelectedIndex > -1) {
                lstMiscPlatforms.Items.RemoveAt(lstMiscPlatforms.SelectedIndex);
            }
        }

        private void label5_Click(object sender, EventArgs e) {

        }

        private void cboFormat_SelectedIndexChanged(object sender, EventArgs e) {

        }

        


    }
}
