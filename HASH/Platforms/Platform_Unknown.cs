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
        /// This is a special class that acts as a place-holder when a ROM's platform can not be determined. It is not included in the
        /// collection of platforms (HASH.Platform.Platforms). It provides minimal functionality.
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        private class Platform_Unknown : Platform
        {
            internal Platform_Unknown() {
                KnownExtensions = new string[] { };
            }
            public override bool IsPlatformMatch(byte[] rom) {
                return false;
            }


            public override Platforms ID { get { return HASH.Platforms.Unknown; } }



            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash);
            }

            protected override void AddPlatformExtendedData(RomExDataBuilder builder, byte[] rom, RomData data) {
            }

            public override int GetRomSize(byte[] rom) {
                return rom.Length;
            }

            public override string GetRomFormatName(byte[] rom) {
                return "Unknown format";
            }
            public override bool? HasHeader(byte[] rom) {
                return null;
            }

            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return null;
                }
            }
        }

    }


}
