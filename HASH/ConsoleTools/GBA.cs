using System;
using System.Collections.Generic;
using System.Text;

namespace HASH.ConsoleTools
{
    /// <summary>
    /// Provides utility methods related to GBA
    /// </summary>
    static class GBA
    {
        /// <summary>The sum of all the bytes making up the standard gameboy logo image in the GB header</summary>
        const ushort GbaLogoChecksum = 0x4927;
        /// <summary>Location of the GB logo within the ROM</summary>
        const int GbaLogoOffset = 0x04;
        /// <summary>Size of the GB logo image in bytes</summary>
        const int GbaLogoSize = 152;

        /// <summary>
        /// Verifies the "GameBoy" logo embedded in the ROM is valid by calculating a checksum.
        /// </summary>
        /// <param name="rom">A byte array containing the ROM or header data.</param>
        /// <param name="headerOffset">The location of the header offset. Fora ROM this should be GbHeader.HeaderOffset.</param>
        /// <returns>A boolean indicatin whether the logo data appears to be valid.</returns>
        public static bool VerifyLogo(byte[] rom, int headerOffset) {
            if (rom.Length < headerOffset + GbaLogoOffset + GbaLogoSize)
                return false;

            int sum = 0;
            for (int i = 0; i < GbaLogoSize; i++) {
                sum += rom[GbaLogoOffset + i];
            }

            return sum == GbaLogoChecksum;
        }

        /// <summary>Calculates header checksum</summary>
        /// <param name="rom"></param>
        /// <param name="headerOffset">Offset of header (should be 0x100 for a ROM image)</param>
        /// <remarks>Does not do range checking</remarks>
        /// <returns>An 8-bit checksum of the header</returns>
        public static byte CalculateHeaderChecksum(byte[] rom, int headerOffset) {

            int firstByte = headerOffset + 0xA0;
            int lastByte = headerOffset + 0xBC;
            int sum = 0;
            for (int i = firstByte; i <= lastByte; i++) {
                sum -= rom[i];
            }

            // Truncate
            return (byte)(sum - 0x19);
        }
    }

    /// <summary>
    /// Represents a Game Boy Advance internal ROM header
    /// </summary>
    class GbaHeader
    {
        public const int GbaHeaderSize = 0xC0;
        //  Address Bytes Expl.
        //  $000     4     ROM Entry Point  (32bit ARM branch opcode, eg. "B rom_start")
        //  $004     156   Nintendo Logo    (compressed bitmap, required!)
        //  $0A0     12    Game Title       (uppercase ascii, max 12 characters)
        //  $0AC     4     Game Code        (uppercase ascii, 4 characters)
        //  $0B0     2     Maker Code       (uppercase ascii, 2 characters)
        //  $0B2     1     Fixed value      (must be 96h, required!)
        //  $0B3     1     Main unit code   (00h for current GBA models)
        //  $0B4     1     Device type      (usually 00h)
        //  $0B5     7     Reserved Area    (should be zero filled)
        //  $0BC     1     Software version (usually 00h)
        //  $0BD     1     Complement check (header checksum, required!)
        //  $0BE     2     Reserved Area    (should be zero filled)
        //   --- Additional Multiboot Header Entries ---
        //  $0C0     4     RAM Entry Point  (32bit ARM branch opcode, eg. "B ram_start")
        //  $0C4     1     Boot mode        (init as 00h - BIOS overwrites this value!)
        //  $0C5     1     Slave ID Number  (init as 00h - BIOS overwrites this value!)
        //  $0C6     26    Not used         (seems to be unused)
        //  $0E0     4     JOYBUS Entry Pt. (32bit ARM branch opcode, eg. "B joy_start")

        /// <summary>
        /// Initializes a GbaHeader object from raw ROM header data
        /// </summary>
        /// <param name="rom">ROM or header data</param>
        /// <param name="offset">Offset of header within array.</param>
        public GbaHeader(byte[] rom, int offset) {
            ValidGbaLogo = GBA.VerifyLogo(rom, offset);
            Title = Util.ParseAscii(rom, offset + 0xa0, 12);
            GameCode = Util.ParseAscii(rom, offset + 0xac, 4);
            MakerCode = Util.ParseAscii(rom, offset + 0xb0, 2);
            RomVersion = rom[offset + 0xbc];
            HeaderChecksum = rom[offset + 0xbd];
            HeaderChecksumValid = HeaderChecksum == GBA.CalculateHeaderChecksum(rom, offset);
        }

        /// <summary>True if the logo data in the header appears to be valid</summary>
        public bool ValidGbaLogo { get; private set; }
        /// <summary>Title of the game</summary>
        public string Title { get; private set; }
        /// <summary>Game's product code</summary>
        public string GameCode { get; private set; }
        /// <summary>Game maker code</summary>
        public string MakerCode { get; private set; }
        /// <summary>ROM version</summary>
        public int RomVersion { get; private set; }
        /// <summary>Header checksum</summary>
        public byte HeaderChecksum { get; private set; }
        /// <summary>True if the header checksum is valid.</summary>
        public bool HeaderChecksumValid { get; private set; }

       
    }
}
