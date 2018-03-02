using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using HASH.ConsoleTools;

namespace HASH
{
    abstract partial class Platform
    {
    
        /// <summary>
        /// Defines a Platform object for NES
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        private class Platform_NES : Platform
        {
            internal Platform_NES() {
                KnownExtensions = new string[] { "nes" };
            }
            public override bool IsPlatformMatch(byte[] rom) {
                return HasINesHeader(rom);
            }

            /// <summary>
            /// Identifies iNES header by magic number
            /// </summary>
            /// <param name="rom">ROM to examine</param>
            /// <returns>True if the magic number is found</returns>
            private static bool HasINesHeader(byte[] rom) {
                if (rom.Length > 0x10 && (rom[0] == 0x4e && rom[1] == 0x45 && rom[2] == 0x53 && rom[3] == 0x1a))
                    return true;
                return false;
            }

            public override Platforms ID { get { return HASH.Platforms.NES; } }



            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                iNesHeader header = null;
                if (rom.Length > 0x10) {
                    header = new iNesHeader(rom, 0);
                }

                if (header != null && header.MagicNumberIsCorrect) { // Headered
                    worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash);

                    // Calculate headerless hashes and PRG/CHR hashes
                    int ROMSize = header.PrgRomSize + header.ChrRomSize;
                    ROMSize = Math.Min(ROMSize, rom.Length - 0x10);

                    worker.AddHashes(rom, 0x10, ROMSize, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.RomHash);
                } else {
                    // Unheadered (file hash IS rom hash)
                    worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash, HashFlags.RomHash);
                }

            }

            protected override void AddPlatformExtendedData(RomExDataBuilder builder, byte[] rom, RomData data) {
                var HeaderCat = RomExDataBuilder.HeaderCat;


                if (rom.Length < 0x10) return;

                var header = new iNesHeader(rom, 0);
                if (header.MagicNumberIsCorrect) {
                    builder.AddData(HeaderCat, "CHR Banks", header.ChrRomCount.ToString());
                    builder.AddData(HeaderCat, "PRG Banks", header.PrgRomCount.ToString());
                    builder.AddData(HeaderCat, "Battery backed", header.BatteryPacked ? "Yes" : "No");

                    if ((int)header.Mapper > 9) {
                        builder.AddData(HeaderCat, "Mapper", ((int)header.Mapper).ToString() + " ($" + (((int)header.Mapper).ToString("x")) + ") - " + header.Mapper.ToString());
                    } else {
                        builder.AddData(HeaderCat, "Mapper", ((int)header.Mapper).ToString() + " - " + header.Mapper.ToString());
                    }
                    builder.AddData(HeaderCat, "Mirroring", header.Mirroring.ToString());
                    builder.AddData(HeaderCat, "Region", header.PalFlagSet ? "PAL" : "NTSC"); ;
                    //builder.AddData(HeaderCat, "SRAM Present", header.SRamFlag ? "Yes" : "No"); ;
                    //builder.AddData(HeaderCat, "SRAM Size", (header.PrgRamCount * 8).ToString() + " kb");

                    builder.AddData(HeaderCat, "Trainer present", header.HasTrainer ? "Yes" : "No");
                    builder.AddData(HeaderCat, "Bus conflicts", header.HasBusConflicts ? "Yes" : "No");
                    builder.AddData(HeaderCat, "VS Unisystem", header.VsUnisystem ? "Yes" : "No");
                    builder.AddData(HeaderCat, "Playchoice 10", header.PlayChoice10 ? "Yes" : "No");

                    builder.AddData(HeaderCat, "iNES 2.0", header.iNES2_0 ? "Yes" : "No");
                }
            }

            public override int GetRomSize(byte[] rom) {
                return HasINesHeader(rom) ? rom.Length - 0x10 : rom.Length;
            }

            public override string GetRomFormatName(byte[] rom) {
                return HasINesHeader(rom) ? "INES" : "NES rom image";
            }
            public override bool? HasHeader(byte[] rom) {
                return HasINesHeader(rom);
            }

            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_nes;
                }
            }
        }
     
    }


}
