using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace HASH.ConsoleTools
{
    /// <summary>
    /// Utility methods related to gameboy
    /// </summary>
    static class GB
    {
        /// <summary>The sum of all the bytes making up the standard gameboy logo image in the GB header</summary>
        const ushort GbLogoChecksum = 0x1546;
        /// <summary>Location of the GB logo within the ROM</summary>
        const int GbLogoOffset = 0x0104;
        /// <summary>Size of the GB logo image in bytes</summary>
        const int GbLogoSize = 0x30;

        /// <summary>
        /// Verifies the "GameBoy" logo embedded in the ROM is valid by calculating a checksum.
        /// </summary>
        /// <param name="rom">A byte array containing the ROM or header data.</param>
        /// <param name="headerOffset">The location of the header offset. Fora ROM this should be GbHeader.HeaderOffset.</param>
        /// <returns>A boolean indicatin whether the logo data appears to be valid.</returns>
        public static bool VerifyLogo(byte[] rom, int headerOffset) {
            if (rom.Length < headerOffset + (GbLogoOffset - 0x100) + GbLogoSize)
                return false;

            int sum = 0;
            for (int i = 0; i < GbLogoSize; i++) {
                sum += rom[GbLogoOffset + i];
            }

            return sum == GbLogoChecksum;
        }

        /// <summary>Calculates header checksum</summary>
        /// <param name="rom"></param>
        /// <param name="headerOffset">Offset of header (should be 0x100 for a ROM image)</param>
        /// <remarks>Does not do range checking</remarks>
        /// <returns>An 8-bit checksum of the header</returns>
        public static byte CalculateHeaderChecksum(byte[] rom, int headerOffset) {
            int firstByte = headerOffset + 0x34;
            int lastByte = headerOffset + 0x4C;
            int sum = 0;
            for (int i = firstByte; i <= lastByte; i++)
			{
                sum -= rom[i] + 1;
			}

            // Truncate
            return (byte)sum;
        }
        /// <summary>
        /// Calculates a ROM's checksum. This can be compared to the
        /// checksum found in the internal header to verify the header.
        /// </summary>
        /// <param name="rom">ROM image.</param>
        /// <returns>A 16-bit checksum of the ROM.</returns>
        public static ushort CalculateRomChecksum(byte[] rom) {
            uint sum = 0;
            
            for (int i = 0; i < rom.Length; i++) {
                sum += rom[i];
            }

            // Checksum bytes are not to be included in checksum
            if (rom.Length >= 0x0150) {
                sum -= rom[0x14e];
                sum -= rom[0x14f];
            }

            return (ushort)sum;
        }

    }

    /// <summary>
    /// Represents a Game Boy (/Color) internal ROM header
    /// </summary>
    class GbHeader    
    {
        /// <summary>
        /// Offset of the internal header in a Game Boy ROM
        /// </summary>
        public const int GbHeaderOffset = 0x100;
        /// <summary>
        /// Size of the internal header in a Game Boy ROM, in bytes
        /// </summary>
        public const int GbHeaderSize = 0x50;

        /// <summary>
        /// Constructs a new GBHeader object initialized by a ROM.
        /// </summary>
        /// <param name="rom">ROM image or header data</param>
        /// <param name="offset">Header offset. Should be GbHeader.GbHeaderOffset for a ROM.</param>
        public GbHeader(byte[] rom, int offset) {
            ValidGbLogo = GB.VerifyLogo(rom, offset);

            Title = Util.ParseAscii(rom, offset + 0x34, 16);
            Manufacturer = Util.ParseAscii(rom, offset + 0x3F, 4);
            switch (rom[offset + 0x43]) {
                case 00:
                    CGBFlag = CgbFlag.None;
                    break;
                case 0x80:
                    CGBFlag = CgbFlag.ColorGB;
                    break;
                case 0xc0:
                    CGBFlag = CgbFlag.ColorGBOnly;
                    break;
                default:
                    CGBFlag = CgbFlag.UnknownValue;
                    break;
            }

            LicenseeEx = Util.ParseAscii(rom, offset + 0x44, 2);
            SupportsSgb = (rom[offset + 0x46] == 0x03);
            CartType = (GbCartType)rom[offset + 0x47];
            RomSize = (GbCartSize)rom[offset + 0x48];
            RamSize = (GbRamSize)rom[offset + 0x49];
            JapanRegion = rom[offset + 0x4a] == 0;
            Licensee = rom[offset + 0x4b];
            RomVersion = rom[offset + 0x4c];
            HeaderChecksum = rom[offset + 0x4d];
            RomChecksum = (ushort)(((int)rom[offset + 0x4e] << 8) | rom[offset + 0x4f]);

            HeaderChecksumValid = GB.CalculateHeaderChecksum(rom, offset) == HeaderChecksum;
            RomChecksumValid = GB.CalculateRomChecksum(rom) == RomChecksum;
        }

        /// <summary>Cart manufacturer</summary>
        public string Manufacturer { get; set; }
        /// <summary>A boolean indicating whether the "Game Boy" logo in the header appears to be valid.</summary>
        public bool ValidGbLogo { get; private set; }
        /// <summary>Game title</summary>
        public string Title { get; private set; }
        /// <summary>Whether the game supports or requires color functionality</summary>
        public CgbFlag CGBFlag { get; private set; }
        /// <summary>Whether the game supports Super Game Boy functionality</summary>
        public bool SupportsSgb { get; private set; }
        /// <summary>Cartridge hardware information</summary>
        public GbCartType CartType { get; private set; }
        /// <summary>Size of ROM</summary>
        public GbCartSize RomSize { get; private set; }
        /// <summary>Size of RAM present on cart</summary>
        public GbRamSize RamSize { get; private set; }
        /// <summary>True if the "Japan Region" flas is set in the header</summary>
        public bool JapanRegion { get; private set; }
        /// <summary>ROM version</summary>
        public byte RomVersion { get; private set; }
        /// <summary>Header checksum</summary>
        public byte HeaderChecksum { get; private set; }
        /// <summary>True if the header checksum is correct</summary>
        public bool HeaderChecksumValid { get; set; }
        /// <summary>Rom checksum</summary>
        public ushort RomChecksum { get; set; }
        /// <summary>True if the ROM checksum is correct</summary>
        public bool RomChecksumValid { get; set; }

        /// <summary>Extended licensee data. Not used in all carts.</summary>
        public string LicenseeEx { get; private set; }
        /// <summary>Licensee data. May be superceeded by extended licensee data.</summary>
        public byte Licensee { get; private set; }

    }

    /// <summary>
    /// Values indicating a cart's support for Game Boy Color
    /// </summary>
    enum CgbFlag
    {
        /// <summary>Game does not work on gameboy color</summary>
        [Description("Game does not work on gameboy color")]
        None,
        /// <summary>Game works on gameboy color (backwards compatible</summary>
        [Description("Game works on gameboy color (backwards compatible)")]
        ColorGB,
        /// <summary>Game works on gameboy color (not backwards compatible</summary>
        [Description("Game works on gameboy color (not backwards compatible)")]
        ColorGBOnly,
        /// <summary>Unknown value</summary>
        [Description("Unknown value")]
        UnknownValue
    }

    /// <summary>
    /// Values indicating a cart's hardware
    /// </summary>
    enum GbCartType
    {
        ROM_ONLY = 0x00,
        MBC1 = 0x01,
        MBC1_RAM = 0x02,
        MBC1_RAM_BATTERY = 0x03,
        MBC2 = 0x05,
        MBC2_BATTERY = 0x06,
        ROM_RAM = 0x08,
        ROM_RAM_BATTERY = 0x09,
        MMM01 = 0x0B,
        MMM01_RAM = 0x0C,
        MMM01_RAM_BATTERY = 0x0D,
        MBC3_TIMER_BATTERY = 0x0F,
        MBC3_TIMER_RAM_BATTERY = 0x10,
        MBC3 = 0x11,
        MBC3_RAM = 0x12,
        MBC3_RAM_BATTERY = 0x13,
        MBC4 = 0x15,
        MBC4_RAM = 0x16,
        MBC4_RAM_BATTERY = 0x17,
        MBC5 = 0x19,
        MBC5_RAM = 0x1A,
        MBC5_RAM_BATTERY = 0x1B,
        MBC5_RUMBLE = 0x1C,
        MBC5_RUMBLE_RAM = 0x1D,
        MBC5_RUMBLE_RAM_BATTERY = 0x1E,
        POCKET_CAMERA = 0xFC,
        BANDAI_TAMA5 = 0xFD,
        HuC3 = 0xFE,
        HuC1_RAM_BATTERY = 0xFF,
    }

    /// <summary>
    /// Values describing ROM sizes
    /// </summary>
    enum GbCartSize
    {
        [Description("$00 -  32 KByte (no ROM banking)")]
        Size_2_banks = 0x00,
        [Description("$01 -  64 KByte (4 banks)")]
        Size_4_banks = 0x01,
        [Description("$02 - 128 KByte (8 banks)")]
        Size_8_banks = 0x02,
        [Description("$03 - 256 KByte (16 banks)")]
        Size_16_banks = 0x03,
        [Description("$04 - 512 KByte (32 banks)")]
        Size_32_banks = 0x04,
        [Description("$05 -   1 MByte (64 banks)")]
        Size_64_banks = 0x05,
        [Description("$06 -   2 MByte (128 banks)")]
        Size_128_banks = 0x06,
        [Description("$07 -   4 MByte (256 banks)")]
        Size_256_banks = 0x07,
        [Description("$52 - 1.1 MByte (72 banks)")]
        Size_72_banks = 0x52,
        [Description("$53 - 1.2 MByte (80 banks)")]
        Size_80_banks = 0x53,
        [Description("$54 - 1.5 MByte (96 banks)")]
        Size_96_banks = 0x54,
    }

    /// <summary>
    /// Values describing RAM sizes
    /// </summary>
    enum GbRamSize
    {
        [Description("$00 -  0 KByte")]
        Zero,
        [Description("$01 -  2 KByte")]
        Size_2K,
        [Description("$02 -  8 KByte")]
        Size_8k,
        [Description("$03 -  32 KByte")]
        Size_32k,
    }
}
