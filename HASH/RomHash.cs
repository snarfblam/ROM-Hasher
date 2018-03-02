using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Romulus;

namespace HASH
{
    /// <summary>
    /// A ROM hash value and associated algorithm and type
    /// </summary>
    class RomHash
    {
        /// <summary>
        /// Creates a RomHash object with the specified data
        /// </summary>
        /// <param name="value">Hash value</param>
        /// <param name="type">Hash type</param>
        public RomHash(byte[] value, HashFlags type) {
            this.Value = value;
            this.Type = type;
        }

        /// <summary>
        /// Gets hash data
        /// </summary>
        public byte[] Value { get; private set; }
        /// <summary>
        /// Gets a value that indicates the hash algorithm and data that hash applies to
        /// </summary>
        public HashFlags Type { get; private set; }

        public string ToHex() {
            return Hex.FormatHex(Value ?? new byte[0]);
        }

        static Dictionary<HashFlags, string> NamedHashFlags = new Dictionary<HashFlags, string>();
        static RomHash() {
            NamedHashFlags.Add(HashFlags.RomHash | HashFlags.CRC32, "ROM CRC32");
            NamedHashFlags.Add(HashFlags.RomHash | HashFlags.MD5, "ROM MD5");
            NamedHashFlags.Add(HashFlags.RomHash | HashFlags.SHA1, "ROM SHA-1");
            NamedHashFlags.Add(HashFlags.RomHash | HashFlags.SHA256, "ROM SHA-256");

            NamedHashFlags.Add(HashFlags.RomHash_ByteSwap | HashFlags.CRC32, "Byte-swapped ROM CRC32");
            NamedHashFlags.Add(HashFlags.RomHash_ByteSwap | HashFlags.MD5, "Byte-swapped ROM MD5");
            NamedHashFlags.Add(HashFlags.RomHash_ByteSwap | HashFlags.SHA1, "Byte-swapped ROM SHA-1");
            NamedHashFlags.Add(HashFlags.RomHash_ByteSwap | HashFlags.SHA256, "Byte-swapped ROM SHA-256");

            NamedHashFlags.Add(HashFlags.FileHash | HashFlags.CRC32, "File CRC32");
            NamedHashFlags.Add(HashFlags.FileHash | HashFlags.MD5, "File MD5");
            NamedHashFlags.Add(HashFlags.FileHash | HashFlags.SHA1, "File SHA-1");
            NamedHashFlags.Add(HashFlags.FileHash | HashFlags.SHA256, "File SHA-256");

            NamedHashFlags.Add(HashFlags.RomHash | HashFlags.FileHash | HashFlags.CRC32, "ROM/File CRC32");
            NamedHashFlags.Add(HashFlags.RomHash | HashFlags.FileHash | HashFlags.MD5, "ROM/File MD5");
            NamedHashFlags.Add(HashFlags.RomHash | HashFlags.FileHash | HashFlags.SHA1, "ROM/File SHA-1");
            NamedHashFlags.Add(HashFlags.RomHash | HashFlags.FileHash | HashFlags.SHA256, "ROM/File SHA-256");

            NamedHashFlags.Add(HashFlags.RomHash_ByteSwap | HashFlags.FileHash | HashFlags.CRC32, "Byte-swapped ROM/File CRC32");
            NamedHashFlags.Add(HashFlags.RomHash_ByteSwap | HashFlags.FileHash | HashFlags.MD5, "Byte-swapped ROM/File MD5");
            NamedHashFlags.Add(HashFlags.RomHash_ByteSwap | HashFlags.FileHash | HashFlags.SHA1, "Byte-swapped ROM/File SHA-1");
            NamedHashFlags.Add(HashFlags.RomHash_ByteSwap | HashFlags.FileHash | HashFlags.SHA256, "Byte-swapped ROM/File SHA-256");

            NamedHashFlags.Add(HashFlags.PrgHash | HashFlags.CRC32, "PRG CRC32");
            NamedHashFlags.Add(HashFlags.PrgHash | HashFlags.MD5, "PRG MD5");
            NamedHashFlags.Add(HashFlags.PrgHash | HashFlags.SHA1, "PRG SHA-1");
            NamedHashFlags.Add(HashFlags.PrgHash | HashFlags.SHA256, "PRG SHA-256");

            NamedHashFlags.Add(HashFlags.ChrHash | HashFlags.CRC32, "CHR CRC32");
            NamedHashFlags.Add(HashFlags.ChrHash | HashFlags.MD5, "CHR MD5");
            NamedHashFlags.Add(HashFlags.ChrHash | HashFlags.SHA1, "CHR SHA-1");
            NamedHashFlags.Add(HashFlags.ChrHash | HashFlags.SHA256, "CHR SHA-256");
        }
        /// <summary>
        /// Gets a friendly name for a ROM hash type
        /// </summary>
        /// <param name="flags">Hash type</param>
        /// <returns>A string representation of the hash type</returns>
        public static string GetHashName(HashFlags flags) {
            string result;
            if (NamedHashFlags.TryGetValue(flags, out result)) return result;

            return "(" + flags.ToString() + ")";
        }

        /// <summary>
        /// Identifies whether the program configuration requres the specified hash for a given platform
        /// </summary>
        /// <param name="p">The platform in question</param>
        /// <param name="hash">The hash algorithm and type</param>
        /// <returns>True if the hash is required, otherwise false.</returns>
        public static bool IsHashRequired(Platform p, HashFlags hash) {
            if (Program.Config.SkipExtraHashes) {
                // Skip PRG, CHR, and 
                if ((hash & HashFlags.SHA256) != 0) return false;
                if ((hash & (HashFlags.ChrHash | HashFlags.PrgHash)) != 0) return false;

                return true;
            } else {
                return true;
            }
        }

        public const HashFlags AllHashAlgorithms = HashFlags.MD5 | HashFlags.CRC32 | HashFlags.SHA256 | HashFlags.SHA1;
    }

    /// <summary>
    /// Enumerates hash algorithms and applicability
    /// </summary>
    [Flags]
    public enum HashFlags
    {
        //
        // ANY CHANGES MADE HERE SHOULD BE REFLECTED IN _HashFlagsExt
        //

        /// <summary>MD5 hash algorithm</summary>
        MD5 = 1,
        /// <summary>SHA-1 hash algorithm</summary>
        SHA1 = 2,
        /// <summary>SHA-256 hash algorithm</summary>
        SHA256 = 128,
        /// <summary>CRC32 algorithm</summary>
        CRC32 = 4,
        /// <summary>Hash applies to a file</summary>
        FileHash = 8,
        /// <summary>Hash applies to a ROM image (composite image if a file contains multiple ROMs)</summary>
        RomHash = 16,
        /// <summary>Hash applies to a PRG ROM image (NES only)</summary>
        PrgHash = 32,
        /// <summary>Hash applies to a CHR ROM image (NES only)</summary>
        ChrHash = 64,
        /// <summary>Hash applies to a byte-swapped ROM image</summary>
        RomHash_ByteSwap = 256,
    }

    /// <summary>
    /// Utility methods and constants for HashFlags
    /// </summary>
    static class _HashFlagsExt
    {
        /// <summary>
        /// A value that can be ANDed with a HashFlags value to obtain the algorithm flag
        /// </summary>
        public const HashFlags HashAlgorithmFilter = HashFlags.SHA1 | HashFlags.SHA256 | HashFlags.MD5 | HashFlags.CRC32;
        /// <summary>
        /// A value that can be ANDed with a HashFlags value to obtain the 'content' flag (e.g. file hash vs rom hash etc)
        /// </summary>
        public const HashFlags HashContentFilter = HashFlags.FileHash | HashFlags.RomHash | HashFlags.ChrHash | HashFlags.PrgHash | HashFlags.RomHash_ByteSwap;

        /// <summary>
        /// Gets the algorithm for this hash, i.e. MD5, SHA1, etc
        /// </summary>
        public static HashFlags GetAlgorithm(this HashFlags h) { return h & HashAlgorithmFilter; }
        /// <summary>
        /// Gets the content this hash is for, i.e. ROM, entire file, etc
        /// </summary>
        public static HashFlags GetContents(this HashFlags h) { return h & HashContentFilter; }
    }
}
