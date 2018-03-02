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
        /// Defines a Platform object for Game Boy
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        private class Platform_GB : Platform
        {
            internal Platform_GB() {
                KnownExtensions = new string[] { "gb gbc" };
            }
            public override bool IsPlatformMatch(byte[] rom) {
                return GB.VerifyLogo(rom, GbHeader.GbHeaderOffset);
            }

            public override Platforms ID { get { return HASH.Platforms.Gameboy; } }
            
            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash, HashFlags.RomHash);
            }

            protected override void AddPlatformExtendedData(RomExDataBuilder builder, byte[] rom, RomData data) {
                if (rom.Length >= GbHeader.GbHeaderOffset + GbHeader.GbHeaderSize) {
                    var HeaderCat = RomExDataBuilder.HeaderCat;
                    var header = new GbHeader(rom, GbHeader.GbHeaderOffset);

                    builder.AddData(RomExDataBuilder.GeneralCat, "ROM checksum", header.RomChecksum.ToString("X4"));
                    builder.AddData(RomExDataBuilder.GeneralCat, "Checksum valid", header.RomChecksumValid.AsYesNo());

                    builder.AddData(HeaderCat, "Logo present", header.ValidGbLogo.AsYesNo());
                    builder.AddData(HeaderCat, "Header checksum", header.HeaderChecksum.ToString("X2"));
                    builder.AddData(HeaderCat, "Header checksum valid", header.HeaderChecksumValid.AsYesNo());
                    builder.AddData(HeaderCat, "ROM checksum", header.RomChecksum.ToString("X4"));
                    builder.AddData(HeaderCat, "ROM checksum valid", header.RomChecksumValid.AsYesNo());
                    builder.AddData(HeaderCat, "Title", header.Title);
                    builder.AddData(HeaderCat, "Manufacturer", header.Manufacturer);
                    builder.AddData(HeaderCat, "Gameboy Color support", header.CGBFlag.GetDescription());
                    builder.AddData(HeaderCat, "Super Gameboy support", header.SupportsSgb.AsYesNo());
                    builder.AddData(HeaderCat, "Cart type", header.CartType.GetDescription());

                    builder.AddData(HeaderCat, "ROM size", header.RomSize.GetDescription());
                    builder.AddData(HeaderCat, "RAM size", header.RamSize.GetDescription());

                    builder.AddData(HeaderCat, "Mask ROM version", header.RomVersion.ToString());


                    builder.AddData(HeaderCat, "Licensee code", "$" + header.Licensee.ToString("X2"));
                    builder.AddData(HeaderCat, "Licensee code (extended)", header.LicenseeEx);
                }
            }

            public override int GetRomSize(byte[] rom) {
                return rom.Length;
            }

            public override string GetRomFormatName(byte[] rom) {
                return "Gameboy rom image";
            }
            public override bool? HasHeader(byte[] rom) {
                return false;
            }

            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_gb;
                }
            }
        }

    }


}
