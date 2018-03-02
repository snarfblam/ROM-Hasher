using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace HASH.ConsoleTools
{
    /// <summary>
    /// Contains utility functions related to SNES
    /// </summary>
    static class SNES
    {
        /// <summary>
        /// Size of SNES external header (SMC, SWC, or Pro Fighter)
        /// </summary>
        public const int ExternalHeaderSize = 512;
        /// <summary>
        /// Identifies whether the ROM has an external ROM header (SMC, SWC, or Pro Fighter)
        /// </summary>
        /// <param name="ROM">ROM image to examine</param>
        /// <returns>True if an external header is found</returns>
        public static bool HasExternalHeader(byte[] ROM) {
            // Extra 512 bytes?
            return (ROM != null) && ROM.Length % 1024 == ExternalHeaderSize;
        }

        /// <summary>
        /// Calculates the 16-bit checksum of the ROM
        /// </summary>
        public static ushort CalculateChecksum(byte[] ROM) {
            //
            // NOTE: I've found two SNES checksum algorithms, one of
            // which mirrors data to a power-of-two size, and one of
            // which mirrors data to a multiple of 4 MBits. Both failed
            // some of the ROMs tested.

            if (ROM.Length < 0x1000) return 0;

            int mirroredSize, unmirroredSize;
            int baseOffset = HasExternalHeader(ROM) ? ExternalHeaderSize : 0;

            GetRomSize(ROM, out unmirroredSize, out mirroredSize);

            uint result = 0;



            // Sum unmirrored data
            int endOfUnmirrored = baseOffset + unmirroredSize;
            for (int iByte = baseOffset; iByte < endOfUnmirrored; iByte++) {
                result += ROM[iByte];
            }

            if(mirroredSize == 0)
                return (ushort)(result & 0xFFFF);

            int startOfMirrored = baseOffset + unmirroredSize;
            int endOfMirrored = startOfMirrored + mirroredSize;
            int mirrorCount = unmirroredSize / mirroredSize;

            // Sum mirrored data
            for (int iMirror = 0; iMirror < mirrorCount; iMirror++) {
                for (int iByte = startOfMirrored; iByte < endOfMirrored; iByte++) {
                    result += ROM[iByte];
                }
            }

            return (ushort)(result & 0xFFFF);
        }


        /// <summary>
        /// Returns true if the ROM can be determined to have a valid SMC header.
        /// </summary>
        public static bool HasGoodSmcHeader(byte[] rom) {
            if (!HasExternalHeader(rom)) return false;

            // Verify size specified
            int size = rom.Length - ExternalHeaderSize;
            int smcSizeValue = rom[0] | (rom[1] << 8);
            int smcSize = smcSizeValue * 8 * 1024; // Size is specified in units of 8kB

            // Verify zero padding
            if (rom.Length != smcSize + ExternalHeaderSize) return false;
            for (int i = 0; i < 509; i++) {
                if (rom[i + 3] != 0) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if the ROM can be determined to have a valid SWC header
        /// </summary>
        public static bool HasGoodSwcHeader(byte[] rom) {
            if (!HasExternalHeader(rom)) return false;

            // Verify size specified
            int size = rom.Length - ExternalHeaderSize;
            int smcSizeValue = rom[0] | (rom[1] << 8);
            int smcSize = smcSizeValue * 8 * 1024; // Size is specified in units of 8kB

            // Apparently, this is some sort of magic number
            if (rom[8] != 0xAA | rom[9] != 0xBB | rom[10] != 0x04) return false;

            // Verify zero padding
            if (rom.Length != smcSize + ExternalHeaderSize) return false;
            for (int i = 0; i < 501; i++) {
                if (rom[i + 11] != 0) return false;
            }

            return true;
        }

        /// <summary>
        /// This method is an alternate checksum routine. Do not use.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [Obsolete("This is only for testing purposes")]
        public static ushort GetOtherChecksum_DoNotUse(byte[] Data) {
            ushort checksum = 0;            //Calculated checksum.
            ushort remainderBanks = 0;   //Gets the checksum of the leftover banks if any exist.
            int banklog2 = 0;            //Gets Log(base 2) of the number of banks.
            int validSize;               //The data maximum size fitting within a valid region. (e.g. 1MB, 2MB, etc.)

            //This will calculate the size of the ROM fitting in a valid region.
            validSize = (Data.Length) >> 1;
            while (validSize > 0) {
                banklog2++;
                validSize >>= 1;
            }
            validSize = 1 << banklog2;

            //The remainder is, obviously, just what's left over.
            int remainderSize = (Data.Length) - validSize;

            //fixed (byte* ptr = &Data[0]) {
                for (int index = 0; index < validSize; index++)         //checksum within the valid region.
                    checksum += Data[index];
                for (int index = 0; index < remainderSize; index++)     //checksum of the remaining banks
                    remainderBanks += Data[validSize + index];
            //}

            //Add to the checksum whatever the remainder checksum is, multiplied by the number of leftover banks over its size.
            if (remainderSize > 0)
                checksum += (ushort)(remainderBanks * (validSize / remainderSize));
            return checksum;
        }

        /// <summary>
        /// Returns the internal checksum. The result may be incorrect or NULL if the ROM is not
        /// large enough or HIROM/LOROM can't be detected.
        /// </summary>
        /// <param name="rom"></param>
        /// <returns></returns>
        public static ushort? GetInternalChecksum(byte[] rom) {
            int romImageOffset = HasExternalHeader(rom) ? ExternalHeaderSize : 0;

            //if (offset + 1 >= rom.Length) return 0;
            //return (ushort)(rom[offset] | (rom[offset + 1] << 8));
            bool hirom, lorom;
            HiromOrLorom(rom, romImageOffset, out lorom, out hirom);
            
            if (hirom && (rom.Length >= hiromChecksumOffset + 1)) {
                return (ushort)(rom[romImageOffset + hiromChecksumOffset] | rom[romImageOffset + hiromChecksumOffset + 1] << 8);
            } else if (rom.Length >= loromChecksumOffset + 1) {
                return (ushort)(rom[romImageOffset + loromChecksumOffset] | rom[romImageOffset + loromChecksumOffset + 1] << 8);
            }
            return null;
        }



        /// <summary>
        /// Gets the size of mirrored and unmirrored data in the ROM.
        /// </summary>
        public static void GetRomSize(byte[] ROM, out int unmirroredSize, out int mirroredSize) {
            int totalSize = ROM.Length;
            if (HasExternalHeader(ROM)) totalSize -= ExternalHeaderSize;

            unmirroredSize = 1024;

            // We'll keep checking larger powers of two until we
            // find one that is >= total ROM size
            while (true) {
                // Is the size exactly a power of 2? Then nothing is mirrored.
                if (unmirroredSize == totalSize) {
                    mirroredSize = 0;
                    return;
                }

                int nextSize = unmirroredSize << 1;

                // Size is not a power of two? Remainder is mirrored.
                if (nextSize > totalSize) {
                    mirroredSize = totalSize - unmirroredSize;
                    return;
                }

                unmirroredSize = nextSize;
            }
        }

        /// <summary>Location of checksum in a HIROM game</summary>
        const int hiromChecksumOffset = 0xFFDE;
        /// <summary>Location of checksum in a LOROM game</summary>
        const int loromChecksumOffset = 0x7FDE;
        /// <summary>Location of checksum complement in a HIROM game</summary>
        const int hiromChecksumCompOffset = 0xFFDC;
        /// <summary>Location of checksum complement in a LOROM game</summary>
        const int loromChecksumCompOffset = 0x7FDC;
        /// <summary>
        /// Attempts to detect hirom or lorom by locating the header by comparing checksum and checksum complement. NOTE: It is possible
        /// that both or neither will return true.
        /// </summary>
        /// <param name="ROM">ROM iamge</param>
        /// <param name="romImageOffset">Location of the ROM image (should point past header if there is one)</param>
        /// <param name="lorom">Returns wether checksum matches the complement for a LOROM layout</param>
        /// <param name="hirom">Returns wether checksum matches the complement for a HIROM layout</param>
        internal static void HiromOrLorom(byte[] ROM, int romImageOffset, out bool lorom, out bool hirom) {
            if (ROM.Length < romImageOffset + 0x10000) {
                lorom = hirom = false;
                return;
            }
            lorom =
                ((ROM[romImageOffset + loromChecksumOffset] ^ ROM[romImageOffset + loromChecksumCompOffset]) == 0xFF) &
                ((ROM[romImageOffset + loromChecksumOffset + 1] ^ ROM[romImageOffset + loromChecksumCompOffset + 1]) == 0xFF);
            hirom =
                ((ROM[romImageOffset + hiromChecksumOffset] ^ ROM[romImageOffset + hiromChecksumCompOffset]) == 0xFF) &
                ((ROM[romImageOffset + hiromChecksumOffset + 1] ^ ROM[romImageOffset + hiromChecksumCompOffset + 1]) == 0xFF);
        }
    }

    /// <summary>
    /// Contains data from SNES ROM internal header
    /// </summary>
    class SnesHeader
    {
        /// <summary>
        /// Location of header in LOROM layout
        /// </summary>
        const int loromHeaderOffset = 0x7fc0;
        /// <summary>
        /// Location of header in HIROM layout
        /// </summary>
        const int hiromHeaderOffset = 0xffc0;

        /// <summary>
        /// The minimum size a ROM must be to create an SnesHeader object for it.
        /// </summary>
        public const int MinimumRomSize = 0x10000;

        /// <summary>
        /// Creates an SnesHeader initialized from raw header data
        /// </summary>
        /// <param name="ROM">ROM image</param>
        /// <param name="romImageOffset">Location of ROM image (should account for external header)</param>
        public SnesHeader(byte[] ROM, int romImageOffset) {
            if (ROM.Length < romImageOffset + MinimumRomSize) throw new ArgumentException("ROM image too small.");

            bool lorom, hirom;
            SNES.HiromOrLorom(ROM, romImageOffset, out lorom, out hirom);

            int headerOffset;
            if (lorom & !hirom) {
                headerOffset = romImageOffset + loromHeaderOffset;
                Mapping = SnesMapping.LoROM;
            } else if (hirom & !lorom) {
                headerOffset =romImageOffset + hiromHeaderOffset;
                Mapping = SnesMapping.HiROM;
            } else {
                headerOffset = romImageOffset +loromHeaderOffset; // Guess?
                Mapping = SnesMapping.Unknown;
            }

            Checksum = (ushort)(0xFFFF & ((ROM[headerOffset + 0x1F] << 8) | (ROM[headerOffset + 0x1e])));
            ChecksumComplement = (ushort)(0xFFFF & ((ROM[headerOffset + 0x1D] << 8) | (ROM[headerOffset + 0x1C])));
        }

        //#	  SNES Address	PC Address (headered)	Length	Name
        //1	  $00:FFB0	0x0081B0	2 bytes	Maker code
        //2	  $00:FFB2	0x0081B2	4 bytes	Game code
        //3	  $00:FFB6	0x0081B6	7 bytes	Fixed Value ($00)
        //4	  $00:FFBD	0x0081BD	1 byte	Expansion RAM size
        //5	  $00:FFBE	0x0081BE	1 byte	Special version
        //6	  $00:FFBF	0x0081BF	1 byte	Cartridge type


        //ROM Specifications
        //#	  SNES Address	PC Address (headered)	Length	Name
        //7	 *  $00:FFC0	0x0081C0	21 bytes	Internal ROM Name
        //8	 X  $00:FFD5	0x0081D5	1 byte	Map Mode
        //9	 X  $00:FFD6	0x0081D6	1 byte	ROM Type
        //10 X	$00:FFD7	0x0081D7	1 byte	ROM Size
        //11 X	$00:FFD8	0x0081D8	1 byte	SRAM Size
        //12 *	$00:FFD9	0x0081D9	1 byte	Destination code 
        //13  	$00:FFDA	0x0081DA	1 byte	Fixed value ($33)  <---- Licensce ? (see sneskart.txt)
        //14 *	$00:FFDB	0x0081DB	1 byte	Version #
        //15 *	$00:FFDC	0x0081DC	2 bytes	Complement check
        //16 *	$00:FFDE	0x0081DE	2 bytes	Checksum

        public SnesMapping Mapping { get; private set; }
        public ushort Checksum { get; set; }
        public ushort ChecksumComplement { get; set; }
        public int Version { get; set; }
        public SnesRegionCode Region { get; private set; }
        public string GameName { get; private set; }

    }

    /// <summary>
    /// Enumerates region codes from SNES headers
    /// </summary>
    enum SnesRegionCode
    {
        [Description("Japan")]
        Japan = 0,
        [Description("United States")]
        USA = 1,
        [Description("Australia, Europe, Asia, Oceana")]
        AustraliaEuropeOceaniaAsia = 2,
        [Description("Sweden")]
        Sweden = 3,
        [Description("Finland")]
        Finland = 4,
        [Description("Denmark")]
        Denmark = 5,
        [Description("France")]
        France = 6,
        [Description("Holland")]
        Holland = 7,
        [Description("Spain")]
        Spain = 8,
        [Description("Germany, Austria, Switzerland")]
        GermanyAustriaSwitzerland = 9,
        [Description("Italy")]
        Italy = 10,
        [Description("Hong Kong and China")]
        HongKongChina = 11,
        [Description("Indonesia")]
        Indonesia = 12,
        [Description("Korea")]
        Korea = 13,
    }
    /// <summary>
    /// Enumerates SNES ROM mappings
    /// </summary>
    enum SnesMapping
    {
        /// <summary>
        /// Mapping is not known
        /// </summary>
        Unknown,
        /// <summary>
        /// Game uses HIROM mapping
        /// </summary>
        HiROM,
        /// <summary>
        /// Game uses LOROM mapping
        /// </summary>
        LoROM
    }
}
