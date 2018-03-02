using System;
using System.Collections.Generic;
using System.Text;
using HASH.ConsoleTools;

namespace HASH
{
    abstract partial class Platform
    {
        /// <summary>
        /// Defines a Platform object for N64
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        class Platform_N64 : Platform
        {
            public Platform_N64() {
                KnownExtensions = new string[] { "n64 v64 z64" };
            }

            /// <summary>
            /// Added to MiscData to identify ROM as byte-swapped
            /// </summary>
            static readonly object ByteswapTagID = new object();

            internal override void InitRomData(RomData data, byte[] rom) {
                base.InitRomData(data, rom);

                data.MiscData.Add(ByteswapTagID, N64.IsByteswapped(rom));
            }

            /// <summary>
            /// Identifies whether the specified RomData object has been marked as byte-swapped
            /// </summary>
            /// <param name="data">RomData object to examine</param>
            /// <returns>True if the RomData object has tag indicating it is byte-swapped</returns>
            public static N64ByteSwap IsByteswapped(RomData data) {
                object value;

                data.MiscData.TryGetValue(ByteswapTagID, out value);
                if (value is N64ByteSwap) return (N64ByteSwap)value;
                return N64ByteSwap.Unknown;
            }

            public override bool IsPlatformMatch(byte[] rom) {
                // Check for a valid normal or byteswapped header
                return N64.IsByteswapped(rom) != N64ByteSwap.Unknown;
            }

            public override Platforms ID {
                get { return HASH.Platforms.Nintendo64; }
            }
            
            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                // N64 hashes are calculated using the thread pool because the ROMs tend to be large

                bool byteswapped = N64.IsByteswapped(rom) == N64ByteSwap.Swapped;

                byte[] swappedRom = new byte[rom.Length];
                // Watch out for odd # of bytes!
                int len = rom.Length & (~1);
                for (int i = 0; i < len; i += 2) {
                    swappedRom[i] = rom[i + 1];
                    swappedRom[i + 1] = rom[i];
                }

                HashFlags originalType = byteswapped ? HashFlags.RomHash_ByteSwap : HashFlags.RomHash;
                HashFlags swappedType = byteswapped ? HashFlags.RomHash : HashFlags.RomHash_ByteSwap;

                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, HashFlags.SHA256, originalType, HashFlags.FileHash);
                    }
                });
                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, HashFlags.SHA1, originalType, HashFlags.FileHash);
                    }
                });
                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, HashFlags.CRC32, originalType, HashFlags.FileHash);
                    }
                });
                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, HashFlags.MD5, originalType, HashFlags.FileHash);
                    }
                });

                // We can not byte-swap the ROM while it is being hashed
                worker.WaitAll();


                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(swappedRom, 0, swappedRom.Length, () => worker.AbortPending, HashFlags.SHA256, swappedType);
                    }
                });
                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(swappedRom, 0, swappedRom.Length, () => worker.AbortPending, HashFlags.SHA1, swappedType);
                    }
                });
                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(swappedRom, 0, swappedRom.Length, () => worker.AbortPending, HashFlags.CRC32, swappedType);
                    }
                });
                worker.QueueTask((SimpleDelegate)delegate {
                    if (!worker.AbortPending) {
                        worker.AddHashes(swappedRom, 0, swappedRom.Length, () => worker.AbortPending, HashFlags.MD5, swappedType);
                    }
                });

                // We can not return until all hashes are calculated
                worker.WaitAll();

            }

            protected override void AddPlatformExtendedData(Platform.RomExDataBuilder builder, byte[] rom, RomData data) {
                builder.AddData(RomExDataBuilder.GeneralCat, "Byte-swapped", N64.IsByteswapped(rom).GetDescription());
            }


            public override int GetRomSize(byte[] rom) {
                return rom.Length;

            }

            public override string GetRomFormatName(byte[] rom) {
                var swapped = N64.IsByteswapped(rom);
                if (swapped == N64ByteSwap.NotSwapped) return "N64 ROM image";
                if (swapped == N64ByteSwap.Swapped) return "N64 ROM image (byte-swapped)";

                return "N64 ROM image (byte-swapping unknown)";
            }

            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_n64;
                }
            }
        }
    }
}
