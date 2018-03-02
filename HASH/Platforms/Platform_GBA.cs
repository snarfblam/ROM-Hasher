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
        /// Defines a Platform object for GBA
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        private class Platform_GBA : Platform
        {
            internal Platform_GBA() {
                KnownExtensions = new string[] { "gba" };
            }
            public override bool IsPlatformMatch(byte[] rom) {
                return GBA.VerifyLogo(rom, 0);
            }

            public override Platforms ID { get { return HASH.Platforms.GameboyAdvance; } }



            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash, HashFlags.RomHash);
            }

            protected override void AddPlatformExtendedData(RomExDataBuilder builder, byte[] rom, RomData data) {
                if (rom.Length >= GbaHeader.GbaHeaderSize) {
                    var HeaderCat = RomExDataBuilder.HeaderCat;
                    var header = new GbaHeader(rom, 0);

                    builder.AddData(HeaderCat, "Logo present", header.ValidGbaLogo.AsYesNo());
                    builder.AddData(HeaderCat, "Header checksum", header.HeaderChecksum.ToString("X2"));
                    builder.AddData(HeaderCat, "Header checksum valid", header.HeaderChecksumValid.AsYesNo());
                    builder.AddData(HeaderCat, "Title", header.Title);
                    builder.AddData(HeaderCat, "Game Maker", header.MakerCode);
                    builder.AddData(HeaderCat, "Game Code", header.GameCode);

                    builder.AddData(HeaderCat, "Mask ROM version", header.RomVersion.ToString());
                }
            }

            public override int GetRomSize(byte[] rom) {
                return rom.Length;
            }

            public override string GetRomFormatName(byte[] rom) {
                return "Gameboy Advance rom image";
            }
            public override bool? HasHeader(byte[] rom) {
                return false;
            }

            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_gba;
                }
            }
        }

    }


}
