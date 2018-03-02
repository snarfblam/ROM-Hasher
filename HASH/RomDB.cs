using HASH.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace HASH
{
    /// <summary>
    /// Container for parsed ROM database data
    /// </summary>
    class RomDB
    {
        static RomDB() {
            AllMiscPlatforms = _AllMiscPlatforms.AsReadOnly();
        }

        /// <summary>
        /// Loads all databases. UnloadDBs must be called between calls to LoadDBs.
        /// </summary>
        public static void LoadDBs(){
            if(!Directory.Exists(FileSystem.DbPath)){
                Directory.CreateDirectory(FileSystem.DbPath);
            }

            var alldbs = Program.DatabaseConfig.Databases;
            for (int i = 0; i < alldbs.Count; i++) {
                var db = alldbs[i];
                try{
                    string path = db.Filename;
                    if(!Path.IsPathRooted(path)){
                        path = System.IO.Path.Combine(FileSystem.DbPath, path);
                    }

                    if (!File.Exists(path)) {
                        MessageBox.Show("The database " + Path.GetFileName(path) + " was not found and can not be loaded.");
                    } else {

                        var loadedDB = ClrMameProParser.ParseDAT(db, File.ReadAllText(path), db.Platforms);
                        loadedDB.AssociatedConfigEntry = db;
                        DBs.Add(loadedDB);

                        for (int j = 0; j < db.MiscPlatforms.Count; j++) {
                            var platformName = db.MiscPlatforms[j];
                            loadedDB.MiscPlatforms.Add(db.MiscPlatforms[j]);

                            // Only add unique strings. Case-insensitive
                            if (_AllMiscPlatforms.Find(s => platformName.Equals(s, StringComparison.OrdinalIgnoreCase)) == null) {
                                _AllMiscPlatforms.Add(platformName);
                            }
                        }
                    }
                }catch (ClrMameProParser.CmpDocumentException ex){
                   var rslt = MessageBox.Show("A database could not be loaded (" + db.Filename + "). Additional errors will be ignored. Show details?", "Error", MessageBoxButtons.YesNo);
                   if (rslt == DialogResult.Yes) {
                       using (var frm = new frmError()) {
                           frm.SetError(ex);
                           frm.ShowDialog();
                       }
                   }
                }

            }

        }

        /// <summary>
        /// List of non-built-in platforms supported by this databases 
        /// </summary>
        public List<string> MiscPlatforms { get; set; }

        static List<string> _AllMiscPlatforms = new List<string>();
        /// <summary>
        /// List of all non-built-in platforms supported by a loaded database
        /// </summary>
        public static IList<string> AllMiscPlatforms { get; private set; }

        private static List<RomDB> DBs = new List<RomDB>();
        /// <summary>
        /// Gets all databases for built-in systems applicable to the specified platform
        /// </summary>
        /// <param name="platform">Platform to aquire DBs for</param>
        /// <returns>A collection of all applicable databases</returns>
        public static IList<RomDB> GetDBs(Platforms platform) {
            List<RomDB> result = new List<RomDB>();

            for (int i = 0; i < DBs.Count; i++) {
                if (DBs[i].Platforms.Contains(platform)) 
                    result.Add(DBs[i]);
            }

            return result;
        }

        /// <summary>
        /// Gets a list of all databases loaded
        /// </summary>
        /// <returns>A collection of loaded databases</returns>
        public static IList<RomDB> GetAllDBs() {
            return new List<RomDB>(DBs);
        }

        /// <summary>
        /// Gets the database configuration that was used to load this database
        /// </summary>
        public DBConfig.Database AssociatedConfigEntry { get; set; }
        
        /// <summary>
        /// Creates an empty RomDB object
        /// </summary>
        /// <param name="platform">Platform this DB applies to</param>
        /// <param name="name">Name of the database</param>
        public RomDB(Platforms platform, string name) 
            :this(name) {
            this.Platforms.Add(platform);

        }

        /// <summary>
        /// Creates an empty RomDB object
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <param name="platforms">A collection of platforms this DB applies to</param>
        public RomDB(IList<Platforms> platforms, string name)
            : this(name) {
            if (platforms != null)
                this.Platforms.AddRange(platforms);
        }

        /// <summary>
        /// Initializes an empty RomDB
        /// </summary>
        /// <param name="name">DB name</param>
        private RomDB(string name) {
            MiscPlatforms = new List<string>();
            Platforms = new List<Platforms>();
            this.Name = name;
            Entries = _Entries.AsReadOnly();
        }

        /// <summary>Gets the name of this database. See remarks.</summary>
        /// <remarks>The names of "standard" ROM dbs are listed as constants of the RomDB class, e.g. RomDB.NoIntroName.</remarks>
        public string Name { get; private set; }
        List<Entry> _Entries = new List<Entry>();
        /// <summary>Gets the entries contained in this ROM database.</summary>
        public IList<Entry> Entries { get; private set; }

        ///// <summary>Gets the platform that this database applies to.</summary>
        //public Platforms Platform { get; private set; }
        public List<Platforms> Platforms { get; private set; }
        
        /// <summary>Gets the version of this database. The format of this value depends on the database or may be null if version is not specified.</summary>
        public string Version { get; private set; }
        /// <summary>Gets the date this database was created/updated, or null if not specified.</summary>
        public DateTime? Date { get; private set; }
        /// <summary>Gets/sets a comment for this ROM database.</summary>
        public string Comment { get; set; }

        /// <summary>
        /// Sets the version and date properties
        /// </summary>
        /// <param name="version"></param>
        /// <param name="date"></param>
        internal void SetVersion(string version, DateTime? date) {
            this.Version = version;
            this.Date = date;
        }

        /// <summary>
        /// Adds an entry to the database
        /// </summary>
        /// <param name="e">Database entry</param>
        public void AddEntry(Entry e) {
            _Entries.Add(e);
        }

        /// <summary>
        /// ROM databse entry
        /// </summary>
        public class Entry
        {
            /// <summary>
            /// Size of the ROM, or zero if unknown
            /// </summary>
            public ulong size;
            /// <summary>
            /// Entry name
            /// </summary>
            public string name;
            /// <summary>
            /// Any hashes associated with the ROM entry
            /// </summary>
            public List<RomHash> Hashes = new List<RomHash>();
            
        }


        /// <summary>
        /// Finds database matches for the given hashes.
        /// </summary>
        /// <param name="Hashes">List of hashes to compare against ROM database. See remarks.</param>
        /// <param name="romSize">Specify zero. Behavior is not defined for non-zero value.</param>
        /// <remarks>Each entry in the ROM database is compared with all applicable hashes. An
        /// entry is considered a match if all applicable hashes match and there is at least one
        /// applicable hash.</remarks>
        public IList<Entry> FindMatches(IList<RomHash> Hashes, ulong romSize) {
            return FindMatches(Hashes);
        }
        /// <summary>
        /// Finds database matches for the given hashes.
        /// </summary>
        /// <param name="Hashes">List of hashes to compare against ROM database. See remarks.</param>
        /// <remarks>Each entry in the ROM database is compared with all applicable hashes. An
        /// entry is considered a match if all applicable hashes match and there is at least one
        /// applicable hash.</remarks>
        public IList<Entry> FindMatches(IList<RomHash> Hashes) {
            List<Entry> result = new List<Entry>();
            
            for (int i = 0; i < Entries.Count; i++) {
                var entry = Entries[i];
                //if (entry.size == romSize) {
                    if (hashesMatch(entry.Hashes, Hashes)) {
                        result.Add(entry);
                    }
                //}
            }

            return result;
        }

        // Returns false if any db hashes disagree with the given rom hashes
        private bool hashesMatch(IList<RomHash> dbHashes, IList<RomHash> romHashes) {
            int matchCount = 0;
            for (int i = 0; i < romHashes.Count; i++) {
                if (hashMatches(dbHashes, romHashes[i])) {
                    matchCount++;
                } else {
                    return false;
                }
            }
            return matchCount > 0;
        }

        // Returns false if any db hashes disagree with the given rom hash
        private bool hashMatches(IList<RomHash> dbHashes, RomHash romHash) {
            for (int i = 0; i < dbHashes.Count; i++) {
                if (dbHashes[i].Type == romHash.Type) {
                    if (dbHashes[i].Value.Length != romHash.Value.Length) {
                        System.Diagnostics.Debug.Fail("Why would this ever happen?");
                        return false;
                    }

                    byte[] dbh = dbHashes[i].Value;
                    byte[] rh = romHash.Value;
                    for (int iByte = 0; iByte < dbh.Length; iByte++) {
                        if (dbh[iByte] != rh[iByte]) {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Unloads all databases from memory. After calling LoadDBs, call this
        /// method before any subsequent call to LoadDBs.
        /// </summary>
        internal static void UnloadDbs() {
            RomDB._AllMiscPlatforms.Clear();
            RomDB.DBs.Clear();
        }
    }

    
}
