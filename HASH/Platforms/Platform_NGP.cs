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
        /// Defines Platform object for Neo Geo Pocket
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        private class Platform_NGP : Platform
        {
            internal Platform_NGP() {
                KnownExtensions = new string[] { "ngp ngc"};
            }

            /// <summary>
            /// Value that identifies first-party ROMs
            /// </summary>
            static byte[] NgpCopyright = { 0x43, 0x4F, 0x50, 0x59, 0x52, 0x49, 0x47, 0x48, 0x54, 0x20, 0x42, 0x59, 0x20, 0x53, 0x4E, 0x4B };
            /// <summary>
            /// Value that identifies third-party ROMs
            /// </summary>
            static byte[] NgpLicense = { 0x20, 0x4C, 0x49, 0x43, 0x45, 0x4E, 0x53, 0x45, 0x44, 0x20, 0x42, 0x59, 0x20, 0x53, 0x4E, 0x4B };
            
            public override bool IsPlatformMatch(byte[] rom) {
                if (rom.Length < 0x10) return false;

                bool match = true;
                for (int i = 0; i < NgpCopyright.Length; i++) {
                    if (rom[i] != NgpCopyright[i]) {
                        match = false;
                        break;
                    }
                }
                if (match) return true;

                match = true;
                for (int i = 0; i < NgpLicense.Length; i++) {
                    if (rom[i] != NgpLicense[i]) {
                        match = false;
                        break;
                    }
                }
                return match;
            }

            public override Platforms ID { get { return HASH.Platforms.NeoGeoPocket; } }



            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash, HashFlags.RomHash);
               
            }

            protected override void AddPlatformExtendedData(RomExDataBuilder builder, byte[] rom, RomData data) {
                //var HeaderCat = RomExDataBuilder.HeaderCat;
               
            }

            public override int GetRomSize(byte[] rom) {
                return rom.Length;
            }

            public override string GetRomFormatName(byte[] rom) {
                return "Neo Geo Pocket ROM";
            }
            public override bool? HasHeader(byte[] rom) {
                return false;
            }

            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_ngp;
                }
            }
        }

    }


}
