using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace HASH
{
    /// <summary>
    /// Contains information gathered and determined about a ROM (size, hashes, platform, etc)
    /// </summary>
    /// <remarks>Typically, this class will be instantiated by the static GetRomData method.</remarks>
    class RomData
    {



        /// <summary>
        /// Gets the platform associated with the specified ROM
        /// </summary>
        /// <param name="rom">ROM iamge</param>
        /// <param name="extension">ROM file's extension</param>
        /// <param name="platformPrompt">If not null, the user will be prompted to manually select a platform with this message.</param>
        /// <param name="platformPromptTitle">Window caption to be used if platformPrompt is non-null</param>
        /// <param name="platform">Returns the ROM's platform, null if the user cancelled the dialog, or Platform.UnknownPlatform if the ROM's platform is not supported</param>
        /// <param name="miscPlatform">The name of the ROM's platform if the platform does not have built-in support, otherwise null.</param>
        /// <returns>A value indicating how the platform was identified.</returns>
        public static PlatformIdentMethod GetRomPlatform(byte[] rom, string extension, string platformPrompt, string platformPromptTitle, out Platform platform, out string miscPlatform) {
            return IdentifyPlatform(rom, extension, platformPrompt, platformPromptTitle, out platform, out miscPlatform);
        }

        /// <summary>
        /// Gets the platform associated with the specified ROM
        /// </summary>
        /// <param name="rom">ROM iamge</param>
        /// <param name="extension">ROM file's extension</param>
        /// <param name="platform">Returns the ROM's platform, null if the user cancelled the dialog,  or Platform.UnknownPlatform if the ROM's platform is not supported</param>
        /// <param name="miscPlatform">The name of the ROM's platform if the platform does not have built-in support, otherwise null.</param>
        /// <returns>A value indicating how the platform was identified.</returns>
        public static PlatformIdentMethod GetRomPlatform(byte[] rom, string extension, out Platform platform, out string miscPlatform) {
            return IdentifyPlatform(rom, extension, null, null, out platform, out miscPlatform);
        }

        /// <summary>
        /// Calculates ROM data and produces extended ROM info
        /// </summary>
        /// <param name="rom">ROM image</param>
        /// <param name="platform">ROM platform, or Platform.UnknownPlatform</param>
        /// <param name="miscPlatform">Null, or, optionally, the name of the unsupported platform is Platform.Unknown is specified</param>
        /// <param name="findDbMatches">Specify true to search relevant databases for matches</param>
        /// <param name="extendedData">Specify true to produce extended data</param>
        /// <param name="worker">Work manager</param>
        /// <returns>An intialized RomData object, or null if the worker aborted the operation</returns>
        public static RomData GetRomData(byte[] rom, Platform platform, string miscPlatform, bool findDbMatches, bool extendedData, IHashWorkManager worker) {
            bool abort = false;

            RomData result = new RomData();
            result.Platform = platform;
            result.MiscPlatform = miscPlatform;
            result.FileSize = rom.Length;


            //IdentifyPlatform(rom, extension, result, platformPrompt, platformPromptTitle);

            worker.CheckIn(.1f, out abort);
            if (abort) return null;


            if (result.Platform != null) {
                result.Platform.InitRomData(result, rom);

                result.RomSize = result.Platform.GetRomSize(rom);
                result.FormatName = result.Platform.GetRomFormatName(rom);
                result.ExternalHeader = result.Platform.HasHeader(rom);

                worker.CheckIn(.2f, out abort);
                if (abort) return null;

                result.Platform.CalculateHashes(rom, worker, 0.2f, 0.6f);
                var hashes = worker.Hashes;

                if (hashes != null) result._Hashes.AddRange(hashes); // May return null if operation aborted
                result._Hashes.Sort((RomHash a, RomHash b) => (int)a.Type - (int)b.Type); // Sort (ordering isn't meaningful, but will be consistent)
                worker.CheckIn(0.6f, out abort); if (abort) return null;

                if (findDbMatches) {
                    FindDbMatches(result);
                }
                worker.CheckIn(0.8f, out abort); if (abort) return null;

                if (extendedData) {
                    result.ExtendedData = result.Platform.GetExtendedInfo(rom, result);
                } else {
                    result.DatabaseMatches = new List<DBMatch>().AsReadOnly();
                    result.ExtendedData = new List<RomDataCategory>();
                }
            } else {
                result.RomSize = result.FileSize;
                result.FormatName = "unknown";
                result.ExternalHeader = null;
                result.DatabaseMatches = new List<DBMatch>().AsReadOnly();
                result.ExtendedData = new List<RomDataCategory>();
            }
            worker.CheckIn(1f, out abort);

            return result;
        }


        /// <summary>
        /// Implements platform identification
        /// </summary>
        /// <param name="rom">Rom image</param>
        /// <param name="extension">file extension</param>
        /// <param name="platformPrompt">Null, or a non-empty string to require manual platform selection</param>
        /// <param name="platformPromptTitle">Platform selection prompt</param>
        /// <param name="romPlatform">Platform object, or null if the user selected cancel from the platform dialog</param>
        /// <param name="miscPlatform">Unsupported platform name if applicatble</param>
        /// <returns>Platform identifacation method used</returns>
        private static PlatformIdentMethod IdentifyPlatform(byte[] rom, string extension, string platformPrompt, string platformPromptTitle, out Platform romPlatform, out string miscPlatform) {
            bool ext = false, contents = false;
            miscPlatform = null;

            if (string.IsNullOrEmpty(platformPrompt)) {
                romPlatform = Platform.GetAssociatedPlatform(rom, extension, out ext, out contents);
            } else {
                if (string.IsNullOrEmpty(platformPromptTitle)) platformPromptTitle = "Select a platform";
                romPlatform = Platform.UnknownPlatform;
            }

            if (romPlatform.ID == Platforms.Unknown) {
                string misc;
                bool cancelled = false;
                Platforms platform;

                if (string.IsNullOrEmpty(platformPrompt)) {
                    cancelled = !frmPlatformPrompt.GetPlatform(out platform, out misc);
                } else {
                    cancelled = !frmPlatformPrompt.GetPlatform(out platform, out misc, platformPrompt, platformPromptTitle);
                }

                if (platform == Platforms.Unknown) {
                    miscPlatform = misc;
                } else {
                    romPlatform = Platform.GetAssociatedPlatform(platform);
                }

                if (cancelled)
                    romPlatform = null;
            }


            if (ext & contents) {
                return PlatformIdentMethod.ExtensionAndContents;
            } else if (ext) {
                return PlatformIdentMethod.Extension;
            } else {
                return (contents) ? PlatformIdentMethod.Contents : PlatformIdentMethod.None;
            }
        }

        /// <summary>
        /// ROM format name
        /// </summary>
        public string FormatName { get; private set; }
        /// <summary>
        /// Gets the size of the ROM image. This will be different from the FileSize if the file is a container instead of a simple ROM image.
        /// </summary>
        public int RomSize { get; private set; }
        /// <summary>
        /// Gets the size of the ROM file, including headers and other data.
        /// </summary>
        public int FileSize { get; private set; }
        /// <summary>
        /// Returns true if the ROM image has an external header, false if does not, and null if headers are not applicable to the format.
        /// </summary>
        public virtual bool? ExternalHeader { get; private set; }

        /// <summary>
        /// Used to store miscellaneous data. Any object can be used as a key (a unique instance of System.Object is recommended).
        /// </summary>
        public Dictionary<object, object> MiscData { get; private set; }

        /// <summary>
        /// Finds ROM database matches for a given RomData object, and populates the RomData object with those matches.
        /// </summary>
        /// <param name="data">A RomData object that has its hash list populated but not its db match list</param>
        private static void FindDbMatches(RomData data) {

            List<DBMatch> matches = new List<DBMatch>();



            if (data.Platform != null) {
                List<RomDB> processedDbs = new List<RomDB>();

                // Recognized systems
                foreach (var db in RomDB.GetDBs(data.Platform.ID)) {
                    AddDbMatches(data, matches, db);

                    processedDbs.Add(db);
                }

                if (!string.IsNullOrEmpty(data.MiscPlatform)) {
                    // Unrecognized system
                    foreach (var db in RomDB.GetAllDBs()) {
                        // Don't process same DB twice
                        bool platformMatch = false;

                        foreach (var platform in db.MiscPlatforms) {
                            if (platform.Equals(data.MiscPlatform, StringComparison.OrdinalIgnoreCase)) {
                                platformMatch = true;
                            }
                        }

                        if (platformMatch) {
                            AddDbMatches(data, matches, db);

                            processedDbs.Add(db);
                        }
                    }
                }
            }

            data.DatabaseMatches = matches.AsReadOnly();
        }

        private static void AddDbMatches(RomData data, List<DBMatch> matches, RomDB db) {
            var dbmatches = db.FindMatches(data.Hashes);

            for (int i = 0; i < dbmatches.Count; i++) {
                matches.Add(new DBMatch(db, dbmatches[i]));

            }
        }


        /// <summary>
        /// Represents a ROM database match.
        /// </summary>
        public struct DBMatch
        {
            /// <summary>
            /// Creates a DBMatch object
            /// </summary>
            /// <param name="database">The ROM database this match corresponds to.</param>
            /// <param name="entry">The matched entry.</param>
            /// <exception cref="NullReferenceException">Thrown if any arguments are null.</exception>
            public DBMatch(RomDB database, RomDB.Entry entry)
                : this() {
                this.Database = database;
                this.Entry = entry;

                if (this.Database == null || this.Entry == null) throw new ArgumentNullException();
            }
            public RomDB Database { get; private set; }
            public RomDB.Entry Entry { get; set; }

            public bool IsEmpty { get { return Database == null; } }
        }

        /// <summary>
        /// Creates a new, empty instance of RomData
        /// </summary>
        public RomData() {
            Hashes = _Hashes;
            MiscData = new Dictionary<object, object>();
        }

        /// <summary>Returns the platform associated with the ROM, or null if the platform was not determined.</summary>
        public Platform Platform { get; set; }
        /// <summary>Gets/sets a string identifying the platform if it is not recognized by this program. A non-null value
        /// is only valid if Platform is not set to Unknown.</summary>
        public string MiscPlatform { get; private set; }
        /// <summary>Returns the method used to identify the ROM's platform.</summary>
        public PlatformIdentMethod PlatformIdentMethod { get; set; }
        /// <summary>Returns a set of ROM/file hashes</summary>
        public IList<RomHash> Hashes { get; private set; }
        private List<RomHash> _Hashes = new List<RomHash>();

        public IList<DBMatch> DatabaseMatches { get; private set; }
        /// <summary>
        /// Returns a ROM hash specified, or null if it is not found. If multiple applicable hashes are found, the first one is returned.
        /// </summary>
        /// <param name="hashType">The hash to retreive.</param>
        /// <returns>A RomHash object.</returns>
        public RomHash GetHash(HashFlags hashType) {
            for (int i = 0; i < Hashes.Count; i++) {
                if (Hashes[i].Type == hashType) return Hashes[i];
            }

            return null;
        }
        /// <summary>
        /// Returns a set of hashes that match the specified filter.
        /// </summary>
        /// <param name="filter">A filter that specifies which flags are required for a hash.</param>
        /// <returns></returns>
        public IList<RomHash> GetHashes(HashFlags filter) {
            List<RomHash> result = new List<RomHash>();
            if ((int)filter != 0) {
                for (int i = 0; i < Hashes.Count; i++) {
                    if ((Hashes[i].Type & filter) != 0) {
                        result.Add(Hashes[i]);
                    }
                }
            }
            return result;
        }


        List<RomDataCategory> _ExtendedData = new List<RomDataCategory>();
        public IList<RomDataCategory> ExtendedData { get; private set; }

        /// <summary>
        /// Adds the specified RomDataCategory to the extended data.
        /// </summary>
        /// <param name="category">Is that spelled right?</param>
        /// <exception cref="ArgumentException">Thrown if a category is added with an invalid or duplicate name.</exception>
        public void AddExDataCategory(RomDataCategory category) {
            if (string.IsNullOrEmpty(category.Name)) throw new ArgumentException("Category name is invalid.");
            if (GetExDataCategory(category.Name) != null) throw new ArgumentException("Category name already exists");

            _ExtendedData.Add(category);
        }
        /// <summary>
        /// Gets the extended data category with the specified name, or null if the category does not exist.
        /// </summary>
        /// <param name="name">herp</param>
        /// <returns>derp</returns>
        public RomDataCategory GetExDataCategory(string name) {
            for (int i = 0; i < _ExtendedData.Count; i++) {
                if (_ExtendedData[i].Name == name) return _ExtendedData[i];
            }

            return null;
        }
    }

    class RomDataCategory : List<RomDataEntry>
    {
        public RomDataCategory(string name) {
            this.Name = name;
        }
        public string Name { get; private set; }

        /// <summary>
        /// Returns the first entry with the specified name, or null if not found.
        /// </summary>
        /// <param name="entryName">blah</param>
        /// <returns>buhlah</returns>
        public RomDataEntry GetEntry(string entryName) {
            for (int i = 0; i < Count; i++) {
                if (this[i].Name == entryName) return this[i];
            }

            return null;
        }
    }

    /// <summary>Standard tags used to identify objects in RomData.MiscData</summary>
    public static class DataTags
    {
        /// <summary>A decoded header. This is platform dependant and can be internal (e.g. snes) or external (e.g. nes)</summary>
        public static readonly object DecodedHeader = new object();
    }

    /// <summary>
    /// Rom detail entry
    /// </summary>
    class RomDataEntry
    {
        /// <summary>
        /// Name of the value
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Identifies the method used to determine what platform a ROM is intended for.
    /// </summary>
    public enum PlatformIdentMethod
    {
        /// <summary>No source of identification is specified</summary>
        [Description("None")]
        None,
        /// <summary>A ROM's platform was identified by file extension</summary>
        [Description("File extension")]
        Extension,
        /// <summary>A ROM's platform was identified by file contents</summary>
        [Description("File contents")]
        Contents,
        /// <summary>A ROM's platform was identified by file extension and contents</summary>
        [Description("File contents and extension")]
        ExtensionAndContents
    }
}