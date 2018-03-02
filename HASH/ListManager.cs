using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EntryCollection = System.Collections.Generic.IList<System.Collections.Generic.IList<string>>;

namespace HASH
{
    /// <summary>
    /// Manages the creation, destruction, and population of detail display list UI
    /// </summary>
    class ListManager
    {
        /// <summary>
        /// Contains all loaded view
        /// </summary>
        List<DisplayList> entries = new List<DisplayList>();
        /// <summary>
        /// Currently visible view
        /// </summary>
        DisplayList _ActiveList;

        /// <summary>
        /// Raised when a view is added
        /// </summary>
        internal event EventHandler<ViewEventArgs> ViewAdded;
        /// <summary>
        /// Raised before a view is removed
        /// </summary>
        internal event EventHandler<ViewEventArgs> ViewRemoving;
        /// <summary>
        /// Raised after a view is removed
        /// </summary>
        internal event EventHandler<ViewEventArgs> ViewRemoved;

        TabControl _Host;
        /// <summary>
        /// Gets/sets the TabControl that will contain the views
        /// </summary>
        public TabControl Host {
            get { return _Host; }
            set {
                if (entries.Count > 0) throw new InvalidOperationException("Can not re-host ListManager while it contains active lists.");
                _Host = value;
            }
        }

        /// <summary>
        /// Gets the view that is currently visible
        /// </summary>
        public ListView ActiveDisplay {
            get {
                if(Host == null) return null;
                if(Host.SelectedTab == null) return null;

                // Find the listview control within the selected tab
                for (int iChild = 0; iChild < Host.SelectedTab.Controls.Count; iChild++) {
                    Control child = Host.SelectedTab.Controls[iChild];
                    if (child is ListView) {

                        // Confirm it is out ListView
                        for (int iView = 0; iView < entries.Count; iView++) {
                            if (child == entries[iView].ListView) {
                                return child as ListView;
                            }
                        }
                    }
                }

                // Nothing found
                return null;
            }
        }

        /// <summary>
        /// Adds the specified data to the UI, creating views as necessary
        /// </summary>
        /// <param name="lists"></param>
        public void AddLists(IList<RomDataCategory> lists) {
            if (lists == null) throw new ArgumentNullException();

            string[] columns = { "Name", "Value" };
            for (int i = 0; i < lists.Count; i++) {
                AddList(columns, lists[i]);
            }
        }

        /// <summary>
        /// Adds a view with the specified data
        /// </summary>
        /// <param name="columns">Header names</param>
        /// <param name="values">Data values</param>
        /// <returns>Index of the added list</returns>
        public int AddList(IList<string> columns, RomDataCategory values) {
            if (Host == null) throw new InvalidOperationException("ListManager must be hosted before adding lists");

            TabPage newpage = new TabPage();
            newpage.Text = values.Name;

            ListView newView = new ListView();
            newView.View = View.Details;
            newView.FullRowSelect = true;
            newView.Dock = DockStyle.Fill;

            newView.Resize += delegate(object o, EventArgs e) {
                if (newView.Columns.Count > 0) {
                    newView.Columns[newView.Columns.Count - 1].Width = -2;
                }
            };

            for (int i = 0; i < columns.Count; i++) {
                var header = new ColumnHeader() { Text = columns[i] , Width =  150};
                newView.Columns.Add(header);
            }

            for (int i = 0; i < values.Count; i++) {
                var item = values[i];
                ListViewItem lvItem = new ListViewItem();

                lvItem.Text = item.Name;
                //for (int iSubItem = 1; iSubItem < item.Count; iSubItem++) {
                //    lvItem.SubItems.Add(item[i]);
               // }
                lvItem.SubItems.Add(item.Value);

                if (IsImportantItem(values.Name, item.Name)) {
                    lvItem.Font = new System.Drawing.Font(lvItem.Font, System.Drawing.FontStyle.Bold);
                }

                newView.Items.Add(lvItem);



            }
            var newList = new DisplayList(this, newView, newpage);
            entries.Add(newList);
            if (_ActiveList != null) {
                //_ActiveList.ListView.Visible = false;
            }

            _ActiveList = newList;
            newpage.Controls.Add(newView);
            _Host.Controls.Add(newpage);
            newView.BringToFront();

            ViewAdded.Raise(this, new ViewEventArgs(newView));
            return entries.Count - 1;

        }

        /// <summary>
        /// Identifies whether category/entry name combination is considered 'important' based on program configuration. Case-sensitive.
        /// </summary>
        /// <param name="cat">Category name</param>
        /// <param name="name">Entry name.</param>
        /// <returns>True if the item is considered 'important'</returns>
        private bool IsImportantItem(string cat, string name) {
            string fullFieldName = (cat + "/" + name);
            var importantFields = Program.Config.ImportantFields;

            if (importantFields != null) {
                for (int i = 0; i < importantFields.Count; i++) {
                    if (importantFields[i].Equals(fullFieldName, StringComparison.OrdinalIgnoreCase)) {
                        return true;
                    }
                }
            }
            return false;
        }
    
        /// <summary>
        /// Gets the index of the specified view, or -1 if the view is not found in the UI.
        /// </summary>
        /// <param name="ListView">View to get index of</param>
        /// <returns>Index of the specified view, or -1 if the view is not found.</returns>
        public int GetIndex(ListView ListView)
        {
            for (int i = 0; i < entries.Count; i++) {
                if (entries[i].ListView == ListView) return i;
            }

            return -1;
        }

        /// <summary>
        /// Makes the specified view visible
        /// </summary>
        /// <param name="index">Index of the view</param>
        /// <param name="focus">Specify true to bring input focus to the view</param>
        public void ShowList(int index, bool focus) {
            if (index != _ActiveList.Index) {
                //_ActiveList.ListView.Hide();
                //_ActiveList = entries[index];
                //_ActiveList.ListView.Visible = true;
                Host.SelectedTab = entries[index].TabPage;
                _ActiveList = entries[index];

                if (focus)
                    _ActiveList.ListView.Focus();
            }
        }

        /// <summary>
        /// Removes all views from UI
        /// </summary>
        internal void ClearLists() {
            _ActiveList = null;
            for (int i = 0; i < entries.Count; i++) {
                ViewEventArgs eventArgs = new ViewEventArgs(entries[i].ListView);
                ViewRemoving.Raise(this, eventArgs);

                Host.TabPages.Remove(entries[i].TabPage);
                entries[i].TabPage.Dispose();

                ViewRemoved.Raise(this, eventArgs);

            }

            entries.Clear();
        }
    }

    /// <summary>
    /// Stores event parameters relating to a view that contains a listing of details (used by ListManager class)
    /// </summary>
    class ViewEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a ViewEventArgs
        /// </summary>
        /// <param name="view"></param>
        public ViewEventArgs(Control view) {
            this.View = view;
        }
        public Control View { get; private set; }
    }
    /// <summary>
    /// Contains information pertaining to a single view
    /// </summary>
    class DisplayList{
        public DisplayList(ListManager manager, ListView control, TabPage page) {
            this.Manager = manager;
            this.ListView = control;
            this.TabPage = page;
        }

        /// <summary>
        /// Owning ListManager
        /// </summary>
        public ListManager Manager { get; private set; }
        /// <summary>
        /// View's UI
        /// </summary>
        public ListView ListView { get; private set; }
        /// <summary>
        /// View UI's parent control
        /// </summary>
        public TabPage TabPage { get; private set; }
        /// <summary>
        /// Index of this view
        /// </summary>
        public int Index { get { return Manager.GetIndex(ListView); } }
    }
}
