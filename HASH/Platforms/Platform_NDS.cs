using System;
using System.Collections.Generic;
using System.Text;

namespace HASH
{
    abstract partial class Platform
    {
        /// <summary>
        /// Defines a Platform object for Nintendo DS
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        class Platform_NDS : Platform
        {
            public Platform_NDS() {
                KnownExtensions = new string[] { "nds" };
            }


            internal override void InitRomData(RomData data, byte[] rom) {
                base.InitRomData(data, rom);
            }

            const int HeaderCrcRegionSize = 0x15E;

            public override bool IsPlatformMatch(byte[] rom) {
                  //15Eh    2     Header Checksum, CRC-16 of [000h-15Dh]
                if (rom.Length < 0x200) return false;

                byte[] bytesToHash = new byte[HeaderCrcRegionSize];
                Array.Copy(rom, 0, bytesToHash, 0, HeaderCrcRegionSize);

                var hash = Crc16.CalculateHash(bytesToHash, 0, bytesToHash.Length);
                if ((hash & 0xFF) == rom[0x15e] && (hash >> 8) == rom[0x15F])
                    return true;

                return false;
            }

            public override Platforms ID {
                get { return HASH.Platforms.NintendoDS; }
            }

            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                // Hashes are calculated using the thread pool due to the fact that NDS roms are MASSIVE

                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, HashFlags.SHA256, HashFlags.FileHash, HashFlags.RomHash);
                    }
                });
                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, HashFlags.SHA1, HashFlags.FileHash, HashFlags.RomHash);
                    }
                });
                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, HashFlags.CRC32, HashFlags.FileHash, HashFlags.RomHash);
                    }
                });
                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, HashFlags.MD5, HashFlags.FileHash, HashFlags.RomHash);
                    }
                });

                // Can not return until all hashes are ready
                worker.WaitAll();

            }

            protected override void AddPlatformExtendedData(Platform.RomExDataBuilder builder, byte[] rom, RomData data) {
                // Nope
            }


            public override int GetRomSize(byte[] rom) {
                return rom.Length;

            }

            public override string GetRomFormatName(byte[] rom) {
                return "Nintendo DS ROM";
            }

            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_ds;
                }
            }
        }
    }
}
