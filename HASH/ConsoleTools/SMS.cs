using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace HASH.ConsoleTools
{
    /// <summary>
    /// Contains utility functions for Sega Master System and Game Gear
    /// </summary>
    static class SMS
    {
        /// <summary>
        /// Value used to identify SMS header
        /// </summary>
        static byte[] HeaderMagicNumber = { 0x54, 0x4D, 0x52, 0x20, 0x53, 0x45, 0x47, 0x41 };
        /// <summary>
        /// Offset of the SMS header
        /// </summary>
        public const int HeaderOffset = 0x7FF0;

        /// <summary>
        /// Determines whether the specified header appears to be valid, based on checking the magic number
        /// </summary>
        /// <param name="rom">ROM image or header data</param>
        /// <param name="headerOffset">Offset of header data in array</param>
        /// <returns>A value indicating whether the header appears to be valid</returns>
        public static bool VerifyMagicNumber(byte[] rom, int headerOffset) {
            if (rom.Length < headerOffset + HeaderMagicNumber.Length) return false;

            for (int i = 0; i < HeaderMagicNumber.Length; i++) {
                if (rom[i + headerOffset] != HeaderMagicNumber[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates an SMS ROM checksum
        /// </summary>
        /// <param name="rom">A ROM image</param>
        /// <returns>A 16-bit checksum of the ROM</returns>
        public static ushort CalculateChecksum(byte[] rom) {
            int sum = 0;

            // Bytes
            for (int i = 0; i < rom.Length; i++) {
                sum += (rom[i]);
            }

            // Exclude header
            if (rom.Length >= 0x8000) {
                for (int i = 0x7ff0; i < 0x8000; i++) {
                    sum -= (rom[i]);

                }
            }

            return (ushort)sum;
        }
    }

    /// <summary>
    /// Represent header data from Master System and Game Gear games
    /// </summary>
    class SmsHeader
    {
        /// <summary>
        /// Location of a header within a ROM
        /// </summary>
        public const int HeaderOffset = 0x7FF0;

        /// <summary>
        /// Initializes an SmsHeader object from raw header data
        /// </summary>
        /// <param name="rom">ROM image or header data</param>
        /// <param name="headerOffset">Location of header within array</param>
        public SmsHeader(byte[] rom, int headerOffset) {
            if (rom.Length < headerOffset + 0x10) return;

            HeaderPresent = SMS.VerifyMagicNumber(rom, headerOffset);
            Checksum = (ushort)(rom[headerOffset + 0xA] + (rom[headerOffset + 0xB] << 8));
            // 3 bytes, : (last 2 digits, as BCD), (preceeding 2 digits, as BCD), (first digit(s), as binary, upper nibble only)
            ProductCode =
                (rom[headerOffset + 0xE] >> 4).ToString() +
                (rom[headerOffset + 0xD].ToString("X2")) +
                (rom[headerOffset + 0xC].ToString("X2"));
            Version = rom[headerOffset + 0xE] & 0xF;
            Region = (SmsRegion)(rom[headerOffset + 0xF] >> 4);
            Size = (SmsSize)(rom[headerOffset + 0xF] & 0xF);
            ChecksumValid = Checksum == SMS.CalculateChecksum(rom);
        }
        /// <summary>True if a valid header was found</summary>
        public bool HeaderPresent { get; private set; }
        /// <summary>ROM checksum</summary>
        public ushort Checksum { get; private set; }
        /// <summary>True if the checksum is valid</summary>
        public bool ChecksumValid { get; private set; }
        /// <summary>Game product code</summary>
        public string ProductCode { get; private set; }
        /// <summary>Game region</summary>
        public SmsRegion Region { get; private set; }
        /// <summary>ROM version</summary>
        public int Version { get; set; }
        /// <summary>ROM size</summary>
        public SmsSize Size { get; private set; }
    }

    /// <summary>
    /// Enumerates region values in an SMS header
    /// </summary>
    enum SmsRegion
    {
        /// <summary>Japan (Master System)</summary>
        [Description("Japan (Master System)")]
        SMS_Japan = 3,
        /// <summary>Export (Master System)</summary>
        [Description("Export (Master System)")]
        SMS_Export = 4,
        /// <summary>Japan (Game Gear)</summary>
        [Description("Japan (Game Gear)")]
        GG_Japan = 5,
        /// <summary>Export (Game Gear)</summary>
        [Description("Export (Game Gear)")]
        GG_Export = 6,
        /// <summary>International (Game Gear)</summary>
        [Description("International (Game Gear)")]
        GG_International = 7,
    }
    /// <summary>
    /// Enumerates ROM size values in an SMS header
    /// </summary>
    enum SmsSize
    {
        [Description("256 KB")]
        Size_256KB = 0x0,
        [Description("512 KB")]
        Size_512KB = 0x1,
        [Description("8 KB")]
        Size_8KB = 0xa,
        [Description("16 KB")]
        Size_16KB = 0xb,
        [Description("32 KB")]
        Size_32KB = 0xc,
        [Description("48 KB")]
        Size_48KB = 0xd,
        [Description("64 KB")]
        Size_64KB = 0xe,
        [Description("128 KB")]
        Size_128KB = 0xf,
    }

}
