using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace HASH.Config
{
    /// <summary>
    /// Represents an XML-serializable class that can persist program settings.
    /// </summary>
    [Serializable]
    public class ProgramConfig
    {
        /// <summary>
        /// Creates a new program configuration with reasonable default values.
        /// </summary>
        /// <returns>A ProgramConfig object</returns>
        public static ProgramConfig CreateDefaultConfig() {
            ProgramConfig result = new ProgramConfig();
            // Prefer SHA-1 for both ROM and file
            result.RhdnPreferredHashes.Add(HashFlags.SHA1 | HashFlags.RomHash);
            result.RhdnPreferredHashes.Add(HashFlags.FileHash | HashFlags.SHA1);

            result.ImportantFields.Add("General/ROM Format");
            result.ImportantFields.Add("General/External Header");
            result.ImportantFields.Add("General/Interleaved");
            result.ImportantFields.Add("General/No-Intro entry");

            result.SkipExtraHashes = false;
            result.BusyWindowDelay = 350;
            result.FileSizeForImmediateBusyWindow = 4200000; // A little over 4 MB

            result.PreferredDatabaseName = "No-Intro";
            return result;
        }

        /// <summary>
        /// Creates a new ProgramConfig object with uninitialized values.
        /// </summary>
        public ProgramConfig() {
            RhdnPreferredHashes = new List<HashFlags>();
            ImportantFields = new List<string>();
        }
        /// <summary>
        /// Gets/sets the hashses that will appear in the textbox that can be copied to romhacking.net submissions.
        /// </summary>
        public List<HashFlags> RhdnPreferredHashes { get; set; }

        /// <summary>
        /// Gets/sets a list of fields that will be hilighted in the details view. The format is Category/FieldName, e.g. "General/Platform"
        /// </summary>
        public List<string> ImportantFields { get; set; }

        /// <summary>
        /// Gets/sets the database that will be treated as the primary
        /// </summary>
        public string PreferredDatabaseName { get; set; }
        /// <summary>
        /// Gets/sets the version number of the program config format. Newer versions of the program may update or add
        /// to the config file information.
        /// </summary>
        [Browsable(false)]
        public int ProgramConfigVersion { get; set; }

        /// <summary>
        /// Gets/sets whether unnecessary hashes will be skipped.
        /// </summary>
        public bool SkipExtraHashes { get; set; }

        /// <summary>
        /// Gets/sets the number of milliseconds the program must be busy for before an 'working' indicator is shown
        /// </summary>
        public int BusyWindowDelay { get; set; }

        /// <summary>
        /// Gets/sets the minimum file size for which the busy window will be displayed without delay
        /// </summary>
        public long FileSizeForImmediateBusyWindow { get; set; }

        /// <summary>
        /// The program configuration format version for this version of the software. This can be
        /// incremented if new versions of the software make modifications to identify and potentially
        /// account for the discrepancy.
        /// </summary>
        internal const int CurrentProgramConfigVersion = 0;
    }
}
