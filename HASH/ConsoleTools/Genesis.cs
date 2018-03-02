using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HASH.ConsoleTools
{
    /// <summary>
    /// Contains utility methods related to Sega Genesis
    /// </summary>
    public static class Gen
    {
        /// <summary>
        /// Size of external SMD header
        /// </summary>
        public const int ExternalHeaderSize = 512;
        /// <summary>
        /// Examines a ROM image to determine if an SMD header is present
        /// </summary>
        /// <param name="ROM">ROM to examine</param>
        /// <returns>A boolean indicating whether an SMD header is present</returns>
        public static bool HasExternalHeader(byte[] ROM) {
            return (ROM != null) && ROM.Length % 1024 == ExternalHeaderSize;
        }

        /// <summary>
        /// Returns true if the ROM can be determined to have a valid SMD header
        /// </summary>
        public static bool HasValidSmdHeader(byte[] rom) {
            if (!HasExternalHeader(rom)) return false;

            // Verify size specified
            int size = rom.Length - ExternalHeaderSize;
            int smdSizeValue = rom[0] | (rom[1] << 8);
            int smdSize = smdSizeValue * 16 * 1024; // Size is specified in units of 16kB

            // Apparently, this is some sort of magic number
            if (rom[8] != 0xAA | rom[9] != 0xBB) return false;

            // Verify zero padding
            if (rom.Length != smdSize + ExternalHeaderSize) return false;
            for (int i = 12; i < 512; i++) {
                if (rom[i] != 0) return false;
            }

            return true;
        
        }

        /// <summary>
        /// Strings used to identify a Genesis ROM header
        /// </summary>
        public static readonly string[] GenesisNames = { "SEGA GENESIS", "SEGA MEGADRIVE", "SEGA MEGA DRIVE"};
        /// <summary>
        /// Strings used to identify a 32X ROM header
        /// </summary>
        public static readonly string[] Sega32xNames = {"SEGA 32X"};
        /// <summary>
        /// Strings used to identify an interleaved Genesis ROM header
        /// </summary>
        public static readonly string[] GenesisNames_Interleaved = { "SG EEI", "SG EARV", "SG EADIE", "EAGNSS", "EAMGDIE", "EAMG RV" };
        /// <summary>
        /// Strings used to identify an interleaved 32X ROM header
        /// </summary>
        public static readonly string[] Sega32xNames_Interleaved = { "SG 2", "EA2" };
                /// <summary>
        /// Checks for a standard string in the header of a ROM, with or without a header, and interleaved or uninterleaved.
        /// </summary>
        public static bool HasInternalHeader(byte[] rom) {
            bool awhop, bop, baloobop;

            return HasInternalHeader(rom, out awhop, out bop, out baloobop);
        }
        /// <summary>
        /// Checks for a standard string in the header of a ROM, with or without a header, and interleaved or uninterleaved.
        /// </summary>
        public static bool HasInternalHeader(byte[] rom, out bool interleaved, out bool externalHeader, out bool sega32x) {
            if (rom.Length < 0x400) {
                externalHeader = false;
                sega32x = false;
                interleaved = false;
                return false;
            }

            // No header, no interleave (.BIN)
            if (CheckForIntHeader_Uninterleaved(rom, 0x100, out sega32x)) {
                externalHeader = false;
                interleaved = false;
                return true;
            }
            // Header + uninterleaved ROM probably doesn't exist in the wild
            if (CheckForIntHeader_Uninterleaved(rom, 0x300, out sega32x)) {
                externalHeader = true;
                interleaved = false;
                return true;
            }

            // Unheadered, interleaved (interloven?) (.MD maybe?)
            if (CheckForIntHeader_Interleaved(rom, 0x80, out sega32x)) {
                externalHeader = false;
                interleaved = true;
                return true;
            }
            // Headered, interloven (.SMD)
            if (CheckForIntHeader_Interleaved(rom, 0x280, out sega32x)) {
                externalHeader = true;
                interleaved = true;
                return true;
            }

            // Everything you know is false. Yes, even the interloven.
            interleaved = false;
            externalHeader = false;
            sega32x = false;
            return false;
        }

        private static bool CheckForIntHeader_Uninterleaved(byte[] rom, int offset, out bool sega32x) {
            sega32x = false;
            // Get internal header console name
            var bytes = new byte[0x10];
            Array.Copy(rom, offset, bytes, 0, 0x10);
            string text = new string(System.Text.Encoding.ASCII.GetChars(bytes));
            text = text.Replace('_', ' ').TrimStart();

            // Is it a known genesis name?
            for (int i = 0; i < GenesisNames.Length; i++) {
                if (text.StartsWith(GenesisNames[i]))
                    return true;
            }
            // Is it a known 32x name?
            for (int i = 0; i < Sega32xNames.Length; i++) {
                if (text.StartsWith(Sega32xNames[i])) {
                    sega32x = true;
                    return true;
                }
            }

            return false;
        }
        private static bool CheckForIntHeader_Interleaved(byte[] rom, int offset, out bool sega32x) {
            sega32x = false;

            // Get internal header console name
            var bytes = new byte[0x10];
            Array.Copy(rom, offset, bytes, 0, 0x10);
            string text = new string( System.Text.Encoding.ASCII.GetChars(bytes));
            text = text.Replace('_', ' ').TrimStart();

            // Is it a known genesis name?
            for (int i = 0; i < GenesisNames_Interleaved.Length; i++) {
                if (text.StartsWith(GenesisNames_Interleaved[i]))
                    return true;
            }
            // Is it a known 32x name?
            for (int i = 0; i < Sega32xNames_Interleaved.Length; i++) {
                if (text.StartsWith(Sega32xNames_Interleaved[i])) {
                    sega32x = true;
                    return true;
                }
            }

            return false;
        }



        /// <summary>
        /// Size of an interleaved chunk
        /// </summary>
        const int interleaveChunkSize = 0x4000;
        /// <summary>
        /// Size of a bitplane from an interleaved chunk
        /// </summary>
        const int interleaveHalfChunkSize = 0x2000;
        /// <summary>
        /// De-interleaves a genesis ROM.
        /// </summary>
        /// <param name="rom">ROM image.</param>
        /// <param name="hasExternalHeader">If true, the first 512 bytes will not be considered part of the ROM iamge.</param>
        /// <param name="recycleInputBuffer">Specify true to modify the ROM image in place, false </param>
        /// <returns>A deinterlaced ROM. This may be the same buffer as speicified for the 'rom' parameter, depending on
        /// the value of 'recycleInputBuffer'.</returns>
        internal static byte[] DeInterleaveRom(byte[] rom, bool hasExternalHeader, bool? recycleInputBuffer) {


            // If not specified, we will recycle the input buffer for ROMs larger than ~2 megs
            if (recycleInputBuffer == null) recycleInputBuffer = rom.Length > 200400;

            // Offset of chunk currently being de-interleaved
            int chunkOffset = hasExternalHeader ? ExternalHeaderSize : 0;
            // Buffer to write deinterleaved ROM to
            byte[] outputRom;
            // Buffer used to deinterleave chunk
            byte[] deinterleaveBuffer;
            // Location within deinterleave buffer to use
            int deinterleaveWriteOffset;

            if (recycleInputBuffer == true) {
                // Write deinterleaved data to ROM. We will need a buffer to de-interleave each chunk. No need to copy SMD header.
                outputRom = rom;
                deinterleaveBuffer = new byte[interleaveChunkSize];
                deinterleaveWriteOffset = 0;
            } else {
                // Write deinterleaved data to NEW ROM. No need for a de-interleaving buffer. SMD header needs to be copied if present.
                outputRom = new byte[rom.Length];
                deinterleaveBuffer = outputRom;
                deinterleaveWriteOffset = chunkOffset;

                if (hasExternalHeader) {
                    Array.Copy(rom, 0, outputRom, 0, ExternalHeaderSize);
                }
            }

            while (chunkOffset + interleaveChunkSize <= rom.Length) {
                DeinterleaveCore(rom, chunkOffset, deinterleaveBuffer, deinterleaveWriteOffset);

                if (recycleInputBuffer == true) {
                    // Chunk was de-interleaved into a buffer. Copy this to the output ROM.
                    Array.Copy(deinterleaveBuffer, deinterleaveWriteOffset, outputRom, chunkOffset, interleaveChunkSize);
                } else {
                    // Chunk was de-interleaved directly into new ROM. Move the de-interleave pointer to next chunk
                    deinterleaveWriteOffset += interleaveChunkSize;
                }

                // Next chunk
                chunkOffset += interleaveChunkSize;
            }

            return outputRom;
        }

        private static void DeinterleaveCore(byte[] rom, int romOffset, byte[] workMem, int workMemOffset) {
            for (int iRead = 0; iRead < interleaveHalfChunkSize; iRead++) {
                // Read even byte from even block
                workMem[workMemOffset + 1] = rom[romOffset + iRead];
                // Read odd byte from odd block
                workMem[workMemOffset] = rom[romOffset + iRead + interleaveHalfChunkSize];

                workMemOffset += 2;
            }
        }
    }

    /// <summary>
    /// Retreives data from a Genesis ROM and presents it in an easy to read manner
    /// </summary>
    public class GenHeader
    {
        /// <summary>
        /// Acts as a dictionary where the index corresponds to an ASCII character and the value corresponds to the associated IO device (e.g. IOCodes['J'] = "Joystick"
        /// </summary>
        static string[] IOCodes = new string[128]; // One (potential) code per ascii code
        const string IOCodeString = "J=Joypad/6=6-button Joypad/K=Keyboard/P=Printer/B=Control Ball/F=Floppy Disk Drive/L=Activator/4=Team Play/0=Joystick for MS/R=Serial RS232C/T=Tablet/V=Paddle Controller/C=CD-ROM/M=Mega Mouse";
        
        static GenHeader() {
            string[] iocodeSplit = IOCodeString.Split('/');
            for (int i = 0; i < iocodeSplit.Length; i++) {
                // Exmaple: "J=Joypad" -> IOCodes[asc('J')] = "Joypad"
                int code = iocodeSplit[i][0] & 127; // First 128 unicode code points match ASCII, right?
                IOCodes[code] = iocodeSplit[i].Substring(2);
            }
        }

        /// <summary>
        /// Gets the name of the IO device associated with the specified IO device code, or null if the code is not known.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetIODevice(char code) {
            return IOCodes[code & 127];
        }
        /// <summary>
        /// Creates a GenHeader initialized from binary header data
        /// </summary>
        /// <param name="rom">ROM image</param>
        public GenHeader(byte[] rom) {
            if (rom.Length >= 0x200) {
                Platform = ReadStringTrim(rom, 0x100, 0x10);
                Copyright = ReadStringTrim(rom, 0x110, 0x10);
                GameName = ReadStringTrim(rom, 0x120, 0x30);
                AltName = ReadStringTrim(rom, 0x120, 0x30);
                ProductID= ReadStringTrim(rom, 0x180, 0xE);
                IOSupport = ReadStringTrim(rom, 0x190, 0x10);
                Modem = ReadStringTrim(rom, 0x1bc, 0xc);
                Memo = ReadStringTrim(rom, 0x1C8, 0x28);
                Region = ReadStringTrim(rom, 0x1f0, 0x10);

                RomStart = ReadUInt32(rom, 0x1a0);
                RomEnd= ReadUInt32(rom, 0x1a4);
                RamStart = ReadUInt32(rom, 0x1a8);
                RamEnd = ReadUInt32(rom, 0x1ac);

                Checksum = ReadUInt16(rom,0x18e);
            }
        }

        private string ReadStringTrim(byte[] rom, int offset, int len) {
            string result = System.Text.Encoding.ASCII.GetString(rom, offset, len).Trim();
            return result.Replace("\0", " ");
        }

        private uint ReadUInt32(byte[] rom, int offset) {
            if (offset + 4 > rom.Length) return 0;

            return (uint)(rom[offset] << 24 | rom[offset + 1] << 16 | rom[offset + 2] << 8 | rom[offset + 3]);
        }
        private ushort ReadUInt16(byte[] rom, int offset) {
            if (offset + 2 > rom.Length) return 0;

            return (ushort)(rom[offset] << 8 | rom[offset + 1]);
        }

        /// <summary>Game platform</summary>
        public string Platform { get; private set; }
        /// <summary>Copyright notice</summary>
        public string Copyright { get; private set; }
        /// <summary>Game title</summary>
        public string GameName { get; private set; }
        /// <summary>Alternate game title</summary>
        public string AltName { get; private set; }
        /// <summary>Product ID</summary>
        public string ProductID { get; private set; }
        /// <summary>IO support codes</summary>
        public string IOSupport { get; private set; }
        /// <summary>Modem string</summary>
        public string Modem { get; private set; }
        /// <summary>Memo</summary>
        public string Memo { get; private set; }
        /// <summary>Game region</summary>
        public string Region { get; private set; }

        /// <summary>ROM start address</summary>
        public uint RomStart { get; private set; }
        /// <summary>ROM end address</summary>
        public uint RomEnd { get; private set; }
        /// <summary>RAM start address</summary>
        public uint RamStart { get; private set; }
        /// <summary>RAM end address</summary>
        public uint RamEnd { get; private set; }

        /// <summary>
        /// ROM checksum
        /// </summary>
        public ushort Checksum { get; private set; }

        /// <summary>
        /// Stores associated numeric code and company name
        /// </summary>
        public struct CompanyCode
        {
            /// <summary>
            /// Creates a new company code object
            /// </summary>
            /// <param name="code"></param>
            /// <param name="company"></param>
            public CompanyCode(int code, string company)
                :this() {
                this.Code = code;
                this.Company = company;
            }
            /// <summary>
            /// Numeric company code
            /// </summary>
            public int Code { get; private set; }
            /// <summary>
            /// Associated company name
            /// </summary>
            public string Company { get; private set; }
        }

        public static readonly IList<CompanyCode> CompanyCodes = Array.AsReadOnly(new CompanyCode[] {
            new CompanyCode(10 , "Takara"                     ),
            new CompanyCode(11 , "Taito or Accolade"          ),
            new CompanyCode(12 , "Capcom"                     ),
            new CompanyCode(13 , "Data East"                  ),
            new CompanyCode(14 , "Namco or Tengen"            ),
            new CompanyCode(15 , "Sunsoft"                    ),
            new CompanyCode(16 , "Bandai"                     ),
            new CompanyCode(17 , "Dempa"                      ),
            new CompanyCode(18 , "Technosoft"                 ),
            new CompanyCode(19 , "Technosoft"                 ),
            new CompanyCode(20 , "Asmik"                      ),
            new CompanyCode(22 , "Micronet"                   ),
            new CompanyCode(23 , "Vic Tokai"                  ),
            new CompanyCode(24 , "American Sammy"             ),
            new CompanyCode(29 , "Kyugo"                      ),
            new CompanyCode(32 , "Wolfteam"                   ),
            new CompanyCode(33 , "Kaneko"                     ),
            new CompanyCode(35 , "Toaplan"                    ),
            new CompanyCode(36 , "Tecmo"                      ),
            new CompanyCode(40 , "Toaplan"                    ),
            new CompanyCode(42 , "UFL Company Limited"        ),
            new CompanyCode(43 , "Human"                      ),
            new CompanyCode(45 , "Game Arts"                  ),
            new CompanyCode(47 , "Sage's Creation"            ),
            new CompanyCode(48 , "Tengen"                     ),
            new CompanyCode(49 , "Renovation or Telenet"      ),
            new CompanyCode(50 , "Eletronic Arts"             ),
            new CompanyCode(56 , "Razorsoft"                  ),
            new CompanyCode(58 , "Mentrix"                    ),
            new CompanyCode(60 , "Victor Musical Industries"  ),
            new CompanyCode(69 , "Arena"                      ),
            new CompanyCode(70 , "Virgin"                     ),
            new CompanyCode(73 , "Soft Vision"                ),
            new CompanyCode(74 , "Palsoft"                    ),
            new CompanyCode(76 , "Koei"                       ),
            new CompanyCode(79 , "U.S. Gold"                  ),
            new CompanyCode(81 , "Acclaim/Flying Edge"        ),
            new CompanyCode(83 , "Gametek"                    ),
            new CompanyCode(86 , "Absolute"                   ),
            new CompanyCode(93 , "Sony"                       ),
            new CompanyCode(95 , "Konami"                     ),
            new CompanyCode(97 , "Tradewest"                  ),
            new CompanyCode(100, "T*HQ Software"              ),
            new CompanyCode(101, "Tecmagik"                   ),
            new CompanyCode(112, "Designer Software"          ),
            new CompanyCode(113, "Psygnosis"                  ),
            new CompanyCode(119, "Accolade"                   ),
            new CompanyCode(120, "Code Masters"               ),
            new CompanyCode(125, "Interplay"                  ),
            new CompanyCode(130, "Activision"                 ),
            new CompanyCode(132, "Shiny & Playmates"          ),
            new CompanyCode(144, "Atlus"                      ),
            new CompanyCode(151, "Infogrames"                 ),
            new CompanyCode(161, "Fox Interactive"            ),
            new CompanyCode(239, "Disney Interactive"         ),

        });
    }
}
