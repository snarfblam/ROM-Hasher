using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace HASH.Config
{
    /// <summary>
    /// Serializable class that stores contains database configuration.
    /// </summary>
    [Serializable]
    public class DBConfig
    {
        /// <summary>
        /// Creates an instance of DBConfig
        /// </summary>
        public DBConfig() {
            Databases = new List<Database>();
        }

        /// <summary>
        /// Gets/sets the path of the database directory. This is either absolute or relative to the application directory.
        /// </summary>
        /// <remarks>DBConfig v. 0</remarks>
        public string DBDirectory { get; set; }

        /// <summary>
        /// Gets/sets the version of the DBConfig.
        /// </summary>
        /// <remarks>DBConfig v. 0</remarks>
        public int DBConfigVersion { get; set; }
        internal const int CurrentDBConfigVersion = 0;

        /// <summary>
        /// Gets/sets the list of databases.
        /// </summary>
        public List<Database> Databases { get; set; }

        /// <summary>
        /// Contains information about a ROM database.
        /// </summary>
        [Serializable]
        public class Database
        {
            /// <summary>
            /// Creates a new instance of this class.
            /// </summary>
            public Database() {
                Platforms = new List<Platforms>();
                MiscPlatforms = new List<string>();
            }

            /// <summary>
            /// Gets/sets the filename relative to the database folder.
            /// </summary>
            /// <remarks>DBConfig v. 0</remarks>
            public string Filename { get; set; }
            /// <summary>
            /// Gets/sets the name displayed for this database.
            /// </summary>
            /// <remarks>DBConfig v. 0</remarks>
            public string Name { get; set; }
            /// <summary>
            /// Gets the list of platforms this database applies to.
            /// </summary>
            /// <remarks>DBConfig v. 0</remarks>
            public List<Platforms> Platforms { get; set; }
            /// <summary>
            /// Gets the list of additional platforms this database applies to (those that this program has no built-in support for)
            /// </summary>
            /// <remarks>DBConfig v. 0</remarks>
            public List<string> MiscPlatforms { get; set; }
            /// <summary>
            /// Gets/sets the database format
            /// </summary>
            /// <remarks>DBConfig v. 0</remarks>
            public DBFormat Format { get; set; }

            /// <summary>
            /// Gets/sets hints used to parse the database.
            /// </summary>
            /// <remarks>DBConfig v. 0</remarks>
            public DBHints Hints { get; set; }


        }

    }

    /// <summary>
    /// Enumerates database formats.
    /// </summary>
    [Serializable]
    public enum DBFormat
    {
        /// <summary>Format is known or not specified</summary>
        Unspecified,
        /// <summary>ClrMamePro DAT</summary>
        ClrMamePro
    }

    /// <summary>
    /// Enumerates 'hints' used to parse databases.
    /// </summary>
    [Serializable]
    [Flags]
    public enum DBHints
    {
        /// <summary>Indicates that when hash type is not specified as File or ROM, File is assumed</summary>
        [Description("Default to file hash")]
        DefaultHash_File = 1,
        /// <summary>Indicates that when hash type is not specified as File or ROM, ROM is assumed</summary>
        [Description("Default to ROM hash")]
        DefaultHash_ROM = 2,
        /// <summary>Indicates that the database specifies single-byte-swapped hashes (e.g. N64 for No-Intro)</summary>
        [Description("Byte-swapped")]
        ByteSwapped = 4,
    }
}
