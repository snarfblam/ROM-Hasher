using System;
using System.Collections.Generic;
using System.Text;
using HASH.ConsoleTools;

namespace HASH
{
    abstract partial class Platform
    {
        /// <summary>
        /// Defines a Platform object for Master System
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        class Platform_MasterSystem : Platform
        {
            public Platform_MasterSystem() {
                KnownExtensions = new string[] { "sms" };
            }
            public override bool IsPlatformMatch(byte[] rom) {
                return SMS.VerifyMagicNumber(rom, SmsHeader.HeaderOffset);
            }

            public override Platforms ID {
                get { return  HASH.Platforms.MasterSystem; }
            }

            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash, HashFlags.RomHash);
            }

            protected override void AddPlatformExtendedData(Platform.RomExDataBuilder builder, byte[] rom, RomData data) {
                string headerCat = RomExDataBuilder.HeaderCat;

                SmsHeader header = new SmsHeader(rom,SmsHeader.HeaderOffset);
                builder.AddData(RomExDataBuilder.GeneralCat, "Checksum valid", header.ChecksumValid.AsYesNo());

                builder.AddData(headerCat, "Header present", header.HeaderPresent.AsYesNo());
                builder.AddData(headerCat, "Checksum", header.Checksum.ToString("X4"));
                builder.AddData(headerCat, "Checksum valid", header.ChecksumValid.AsYesNo());

                if (header.HeaderPresent) {
                    builder.AddData(headerCat, "Product Code", header.ProductCode);
                    builder.AddData(headerCat, "Region", header.Region.GetDescription());
                    builder.AddData(headerCat, "Version", header.Version.ToString());
                    builder.AddData(headerCat, "Size", header.Size.GetDescription());
                }
            }

            public override int GetRomSize(byte[] rom) {
                return rom.Length;
            }

            public override string GetRomFormatName(byte[] rom) {
                return "Master System ROM";
            }
            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_sms;
                }
            }
        }
    }
}