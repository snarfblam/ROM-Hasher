using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel;
using Romulus;

namespace HASH
{
    /// <summary>
    /// Defines platform-specific functionality.
    /// </summary>
    abstract partial class Platform
    {
        /// <summary>
        /// Initialzies all supported platforms
        /// </summary>
        static Platform() {
            Platforms = _platforms.AsReadOnly();

            _platforms.Add(NES);
            _platforms.Add(FDS);
            _platforms.Add(Genesis);
            _platforms.Add(Gameboy);
            _platforms.Add(GameboyAdvance);
            _platforms.Add(MasterSystem);
            _platforms.Add(GameGear);
            _platforms.Add(Nintendo64);
            _platforms.Add(NintendoDS);
            _platforms.Add(NeoGeoPocket);


            _platforms.Add(SuperNES);
        }
        static List<Platform> _platforms = new List<Platform>();

        /// <summary>
        /// Contains all supported platforms. This list does not include Platform.UnknownPlatform
        /// </summary>
        public static IList<Platform> Platforms { get; private set; }
        /// <summary>Provides platform-specific functionality for NES</summary>
        public static Platform NES = new Platform_NES();
        /// <summary>Provides platform-specific functionality for FDS</summary>
        public static Platform FDS = new Platform_FDS();
        /// <summary>Provides platform-specific functionality for SNES</summary>
        public static Platform SuperNES = new Platform_SNES();
        /// <summary>Provides platform-specific functionality for Genesis</summary>
        public static Platform Genesis = new Platform_Genesis();
        /// <summary>Provides platform-specific functionality for Gameboy</summary>
        public static Platform Gameboy = new Platform_GB();
        /// <summary>Provides platform-specific functionality for Gameboy Advance</summary>
        public static Platform GameboyAdvance = new Platform_GBA();
        /// <summary>Provides platform-specific functionality for Master System</summary>
        public static Platform MasterSystem = new Platform_MasterSystem();
        /// <summary>Provides platform-specific functionality for Game Gear</summary>
        public static Platform GameGear = new Platform_GameGear();
        /// <summary>Provides platform-specific functionality for N64</summary>
        public static Platform Nintendo64 = new Platform_N64();
        /// <summary>Provides platform-specific functionality for Nintendo DS</summary>
        public static Platform NintendoDS = new Platform_NDS();
        /// <summary>Provides platform-specific functionality for Neo Geo Pocket</summary>
        public static Platform NeoGeoPocket = new Platform_NGP();
        
        /// <summary>
        /// Placeholder used for ROMs when the platform can't be determined.
        /// </summary>
        public static Platform UnknownPlatform = new Platform_Unknown();

        public static Platform GetAssociatedPlatform(HASH.Platforms id) {
            for (int i = 0; i < Platforms.Count; i++) {
                if (Platforms[i].ID == id) return Platforms[i];
            }
            
            return Platform.UnknownPlatform;
        }
        /// <summary>
        /// Returns the platform associated with the ROM image, or HASH.Platform.UnknownPlatform if the platform can not be determined.
        /// </summary>
        /// <param name="rom">ROM image</param>
        /// <param name="extension">The file extension of the rom, NOT INCLUDING A DOT, or null or empty string.</param>
        /// <param name="byFileExtension">Returns true if the file extension was used to disambiguate.</param>
        /// <param name="byContents">Returns true if the file contents were used to disambiguate.</param>
        /// <returns></returns>
        public static Platform GetAssociatedPlatform(byte[] rom, string extension, out bool byContents, out bool byFileExtension) {
            List<Platform> results = new List<Platform>();
            byContents = true;
            byFileExtension = true;

            // Find all possible matches
            for (int i = 0; i < Platforms.Count; i++) {
                if (Platforms[i].IsPlatformMatch(rom)) {
                    results.Add(Platforms[i]);
                }
            }

            // If we didn't find a match, we'll pick from ALL platforms based on file extension only
            if (results.Count == 0) {
                results.AddRange(Platforms);
                byContents = false; // We couldn't determine anything by file contents
            } else if (results.Count == 1) {
                byFileExtension = false; // We determined by file contents alone
            }

            // If there is more than one candidate, pick from candidates by file extension
            if (results.Count > 1 && !string.IsNullOrEmpty(extension)) {
                for (int i = results.Count - 1; i >= 0; i--) {
                    if (!check_ext(results[i].KnownExtensions, extension)) {
                        results.RemoveAt(i);
                    }
                }
            }

            // Return what we found
            if (results.Count == 1) return results[0];
            return UnknownPlatform;
        }

        // Return true if romExt is contained in knownExts
        private static bool check_ext(string[] knownExts, string romExt) {
            for (int i = 0; i < knownExts.Length; i++) {
                if (romExt.Equals(knownExts[i], StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the platform logo, or null if the is no logo. See remarks.
        /// </summary>
        /// <remarks>The logo image should be no larger than 150x48 pixels. The returned object is a shared object and should not be disposed.</remarks>
        public virtual System.Drawing.Image SmallPlatformImage { get { return null; } }



        /// <summary>
        /// Returns the "base" platform for this platform. E.g. Platform.GameboyColor.BasePlatform returns Platform.Gameboy.
        /// Returns null if this is a base platform.
        /// </summary>
        public Platform BasePlatform { get; private set; }


        /// <summary>
        /// Returns true if the ROM appears to be for this platform or false if the ROM can not
        /// be CONFIRMED to be intended for this platform.
        /// </summary>
        /// <param name="rom"></param>
        /// <returns></returns>
        public abstract bool IsPlatformMatch(byte[] rom);

        /// <summary>
        /// A constant identifying this platform
        /// </summary>
        public abstract Platforms ID { get; }

        /// <summary>
        /// Gets the known file extensions for the platform. These do not include
        /// the preceeding dot and should be considered case-insensitive.
        /// </summary>
        public string[] KnownExtensions { get; protected set; }

        /// <summary>
        /// Gets a set of hashes relevant to this platform based on the ROM provided.
        /// </summary>
        /// <param name="rom"></param>
        /// <param name="worker"></param>
        /// <param name="startProgress">The progress percentage before hashing starts.</param>
        /// <param name="endProgress">The progress percentage to report when hashing completes.</param>
        /// <returns>A list of hashes for the ROM, or null if the work manager aborts the operation.</returns>
        public abstract void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress);

        /// <summary>
        /// Used by asyncronous workers to check if an abort has been reqested
        /// </summary>
        /// <returns>True if a request to cancel the operation is pending</returns>
        public delegate bool AsyncCancelCheck();



        /// <summary>
        /// Gets extended information for a ROM and associated RomData object.
        /// </summary>
        /// <param name="rom">A ROM file</param>
        /// <param name="data">A RomData object with Platform and Hashes properties initialized.</param>
        /// <returns></returns>
        public IList<RomDataCategory> GetExtendedInfo(byte[] rom, RomData data) {
            var result = new RomExDataBuilder();

            AddStandardData(result,rom,data);
            AddPlatformExtendedData(result,rom,data);

            return result;
        }

        /// <summary>
        /// Override this method to populate the extended ROM data.
        /// </summary>
        /// <param name="builder">An object to assist in populating extended ROM data.</param>
        /// <param name="rom">ROM file</param>
        /// <param name="data">RomData object that has Platform and Hashes properties initialized.</param>
        protected abstract void AddPlatformExtendedData(RomExDataBuilder builder, byte[] rom, RomData data);

        /// <summary>
        /// Adds standard ROM details
        /// </summary>
        /// <param name="builder">Data builder</param>
        /// <param name="rom">ROM image</param>
        /// <param name="data">ROM information</param>
        private void AddStandardData(RomExDataBuilder builder, byte[] rom, RomData data) {
            const string general = RomExDataBuilder.GeneralCat;

            builder.AddData(general, "Platform", data.Platform.ID.GetDescription());
            builder.AddData(general, "ROM format", data.FormatName);
            if (data.ExternalHeader != null) builder.AddData(general, "External Header", data.ExternalHeader.Value ? "Yes" : "No");
            builder.AddData(general, "File Size", data.FileSize.ToString() + " (" + data.FileSize.ToString("x") + ")");
            builder.AddData(general, "ROM Size", data.RomSize.ToString() + " (" + data.RomSize.ToString("x") + ")");

            // ROM hashes
            var crc32 = data.GetHash(HashFlags.RomHash | HashFlags.CRC32);
            var sha1 = data.GetHash(HashFlags.RomHash | HashFlags.SHA1);
            if (crc32 != null) { builder.AddData(general, RomHash.GetHashName(HashFlags.RomHash | HashFlags.CRC32), Hex.FormatHex(crc32.Value)); }
            if (sha1 != null) { builder.AddData(general, RomHash.GetHashName(HashFlags.RomHash | HashFlags.SHA1), Hex.FormatHex(sha1.Value)); }

            // File hashes as a last resort when ROM hashes aren't present
            if (crc32 == null) {
                crc32 = data.GetHash(HashFlags.FileHash | HashFlags.CRC32);
                if (crc32 != null) { builder.AddData(general, RomHash.GetHashName(HashFlags.FileHash | HashFlags.CRC32), Hex.FormatHex(crc32.Value)); }
            }
            if (sha1 == null) {
                sha1 = data.GetHash(HashFlags.FileHash | HashFlags.SHA1);
                if (sha1 != null) { builder.AddData(general, RomHash.GetHashName(HashFlags.FileHash | HashFlags.SHA1), Hex.FormatHex(sha1.Value)); }
            }

            for (int i = 0; i < data.DatabaseMatches.Count; i++) {
                var match = data.DatabaseMatches[i];
                builder.AddData(general, match.Database.Name + " entry", match.Entry.name);
            }

            for (int i = 0; i < data.Hashes.Count; i++) {
                builder.AddData(
                    RomExDataBuilder.HashesCat, 
                    RomHash.GetHashName(data.Hashes[i].Type), 
                    Hex.FormatHex(data.Hashes[i].Value));
            }
        }

    
        /// <summary>
        /// Used to accumulate extended ROM data
        /// </summary>
        protected class RomExDataBuilder : List<RomDataCategory>
        {
            /// <summary>This cat is in charge of all the other cats.</summary>
            public const string GeneralCat = "General";
            /// <summary>This cat just loves #hashes# (^ω^)</summary>
            public const string HashesCat = "All Hashes";
            /// <summary>This cat leads the herd. Gaggle? Gaggle. This cat leads the gaggle.</summary>
            public const string HeaderCat = "Header";

            /// <summary>
            /// Adds a new entry. If the category does not exist, it is created. Duplicate names within the category can be created.
            /// </summary>
            public void AddData(string category, string name, string value) {
                var newEntry = new RomDataEntry() { Name = name, Value = value };
                for (int i = 0; i < Count; i++) {
                    if (this[i].Name == category) {
                        this[i].Add(newEntry);
                        return;
                    }
                }

                RomDataCategory newCat = new RomDataCategory(category);
                newCat.Add(newEntry);
                this.Add(newCat);
            }

            /// <summary>
            /// Sets an entry value. If the category does not exist, it is created. If there are any existing entries in the specified category
            /// with the same name THEY WILL BE DELETED.
            /// </summary>
            public void SetData(string category, string name, string value) {
                for (int i = 0; i < Count; i++) {
                    var cat = this[i];
                    if (cat.Name == category) {

                        // Remove existing entries by same name
                        for (int j = cat.Count - 1; j >= 0; j--) {
                            if (cat[j].Name == name) cat.RemoveAt(j);
                        }

                        // Add new entry
                        cat.Add(new RomDataEntry() { Name = name, Value = value });
                    }
                }
            }
        }

        /// <summary>
        /// Gets the size of the specified ROM image, not including headers or extra data.
        /// </summary>
        public abstract int GetRomSize(byte[] rom);

        /// <summary>
        /// Gets the name of the ROM format.
        /// </summary>
        public abstract string GetRomFormatName(byte[] rom);

        /// <summary>
        /// Returns true if the ROM image has an external header, false if does not, and null if headers are not applicable to the format.
        /// </summary>
        public virtual bool? HasHeader(byte[] rom) {
            return null;
        }

        /// <summary>
        /// This is the first method called when initializing a RomData object.
        /// Platform-specific information can be calculated and stored in the
        /// RomData's MiscData collection.
        /// </summary>
        internal virtual void InitRomData(RomData data, byte[] rom) {
            
        }
    }
    
    /// <summary>
    /// Enumerates platforms with built-in support
    /// </summary>
    [Serializable]
    public enum Platforms
    {
        /// <summary>Unknown platform</summary>
        [Description("Unknown platform")]
        Unknown,
        /// <summary>Nintendo Entertainment System</summary>
        [Description("Nintendo Entertainment System")]
        NES,
        /// <summary>Famicom Disk System</summary>
        [Description("Famicom Disk System")]
        FDS,
        /// <summary>Super NES</summary>
        [Description("Super NES")]
        SuperNES,
        /// <summary>Genesis</summary>
        [Description("Genesis")]
        Genesis,
        /// <summary>Game Boy</summary>
        [Description("Game Boy")]
        Gameboy,
        /// <summary>Game Boy Advance</summary>
        [Description("Game Boy Advance")]
        GameboyAdvance,
        /// <summary>Nintendo DS</summary>
        [Description("Nintendo DS")]
        NintendoDS,
        /// <summary>Nintendo 64</summary>
        [Description("Nintendo 64")]
        Nintendo64,
        /// <summary>Game Gear</summary>
        [Description("Game Gear")]
        GameGear,
        /// <summary>Master System</summary>
        [Description("Master System")]
        MasterSystem,
        /// <summary>Neo Geo Pocket</summary>
        [Description("Neo Geo Pocket")]
        NeoGeoPocket,
    }
}
