using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace HASH
{
    abstract partial class Platform
    {
        /// <summary>
        /// Defines a Platform object for FDS
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        private class Platform_FDS : Platform
        {
            internal Platform_FDS() {
                KnownExtensions = new string[] { "fds" };
            }
            public override bool IsPlatformMatch(byte[] rom) {
                return HasFdsMagicNumber(rom);
            }

            /// <summary>
            /// Identifies whether a ROM has an FDS header by verifying its magic number
            /// </summary>
            /// <param name="rom"></param>
            /// <returns></returns>
            private static bool HasFdsMagicNumber(byte[] rom) {
                return (rom.Length > 0x10 && (rom[0] == 0x46 && rom[1] == 0x44 && rom[2] == 0x53 && rom[3] == 0x1a));
            }


            public override Platforms ID { get { return HASH.Platforms.FDS; } }

            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                if (HasFdsMagicNumber(rom)) { // Headered
                    worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash);

                    // Calculate headerless hashes and PRG/CHR hashes
                    int ROMSize = rom.Length - 0x10;

                    worker.AddHashes(rom, 0x10, ROMSize, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.RomHash);
                } else {
                    // Unheadered (file hash IS rom hash)
                    worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash, HashFlags.RomHash);
                }
            }

            protected override void AddPlatformExtendedData(RomExDataBuilder builder, byte[] rom, RomData data) {
            }

            public override int GetRomSize(byte[] rom) {
                return HasFdsMagicNumber(rom) ? rom.Length - 0x10 : rom.Length;
            }

            public override string GetRomFormatName(byte[] rom) {
                return HasFdsMagicNumber(rom) ? "FDS with header" : "FDS disk image";
            }
            public override bool? HasHeader(byte[] rom) {
                return HasFdsMagicNumber(rom);
            }


            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_fds;
                }
            }
        }

    }


}
