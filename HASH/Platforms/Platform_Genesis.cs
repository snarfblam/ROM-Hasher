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
        /// Defines a Platform object for Genesis
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        private class Platform_Genesis : Platform
        {
            internal Platform_Genesis() {
                KnownExtensions = "gen smd bin md".Split(' ');
            }

            /// <summary>
            /// Maintains a weak reference to a BIN format image to potentially prevent the need to re-convert the ROM
            /// </summary>
            WeakReference BinRomImage;

            static object Tag_32x = new object();

            public override bool IsPlatformMatch(byte[] rom) {
                if (Gen.HasValidSmdHeader(rom)) return true;
                if (Gen.HasInternalHeader(rom)) return true;
                return false;
            }

            public override Platforms ID {
                get { return HASH.Platforms.Genesis; }
            }

            internal override void InitRomData(RomData data, byte[] rom) {
                base.InitRomData(data, rom);

                bool eHeader, iHeader, interleaved, s32X;
                iHeader = Gen.HasInternalHeader(rom, out interleaved, out eHeader, out s32X);

                if (s32X) data.MiscData.Add(Tag_32x,Tag_32x);
            }

            internal bool Is32X(RomData data) {
                return data.MiscData.ContainsKey(Tag_32x);
            }

            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                bool eHeader, iHeader, interleaved, s32X;
                iHeader = Gen.HasInternalHeader(rom, out interleaved, out eHeader, out s32X);

                byte[] binRomImage = GetBinFormat(rom);
                this.BinRomImage = new WeakReference(binRomImage);


                if (binRomImage == rom) {
                    worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash, HashFlags.RomHash);
                } else {
                    worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash);
                    worker.AddHashes(binRomImage, 0, binRomImage.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.RomHash);
                }

            }

            /// <summary>
            /// Converts a ROM file to a headerless, de-interleaved ROM image if it is not already. The returned
            /// array will be the orginial byte array if unmodified, otherwise a new byte array will be
            /// created to hold the ROM image.
            /// </summary>
            /// <param name="rom">ROM file to get the ROM image for</param>
            /// <returns>The returned
            /// array will be the orginial byte array if unmodified, otherwise a new byte array will be
            /// created to hold the ROM image.</returns>
            public static byte[] GetBinFormat(byte[] rom) {
                // Todo: move this function to ConsoleTools.Gen

                bool eHeader, iHeader, interleaved, s32X;
                iHeader = Gen.HasInternalHeader(rom, out interleaved, out eHeader, out s32X);

                byte[] romImage = rom;

                // Remove external header, if present
                if (eHeader) {
                    romImage = new byte[rom.Length - Gen.ExternalHeaderSize];
                    Array.Copy(rom, Gen.ExternalHeaderSize, romImage, 0, rom.Length - Gen.ExternalHeaderSize);
                }

                // De-interleave ROM, if necessary
                if (interleaved) {
                    // Make sure we aren't modifying the ROM passed in as a parameter
                    bool recycleRomImageBuffer = (rom != romImage);

                    romImage = Gen.DeInterleaveRom(romImage, false, recycleRomImageBuffer);
                }
                return romImage;
            }

            protected override void AddPlatformExtendedData(RomExDataBuilder builder, byte[] rom, RomData data) {
                byte[] romImage;
                // If our previously de-interleaved ROM is still in memory, use that instead of re-de-interleaving
                if (BinRomImage == null) {
                    romImage = GetBinFormat(rom);
                } else {
                    romImage = BinRomImage.Target as byte[];
                    if (romImage == null) romImage = GetBinFormat(rom);
                }
                
                bool  interleaved, ext, s32x;
                bool InternalHeaderFound = Gen.HasInternalHeader(rom, out interleaved, out ext, out s32x);
                builder.AddData(RomExDataBuilder.GeneralCat, "32X", s32x.AsYesNo());
                builder.AddData(RomExDataBuilder.GeneralCat, "Internal Header Found", InternalHeaderFound.AsYesNo());
                builder.AddData(RomExDataBuilder.GeneralCat, "Interleaved", interleaved.AsYesNo());

                if (InternalHeaderFound) {
                    const string hdr = RomExDataBuilder.HeaderCat;
                    GenHeader header = new GenHeader(romImage);
                    builder.AddData(hdr, "Platform", header.Platform);
                    builder.AddData(hdr, "Name", header.GameName);
                    builder.AddData(hdr, "International Name", header.AltName);
                    builder.AddData(hdr, "Copyright", GetCopyrightString(header.Copyright));
                    builder.AddData(hdr, "Memo", header.Memo);
                    builder.AddData(hdr, "Modem", header.Modem);
                    builder.AddData(hdr, "Product Type/ID", header.ProductID);
                    builder.AddData(hdr, "Region", header.Region);
                    builder.AddData(hdr, "ROM range", header.RomStart.ToString("X8") + "-" + header.RomEnd.ToString("X8"));
                    builder.AddData(hdr, "RAM range", header.RamStart.ToString("X8") + "-" + header.RamEnd.ToString("X8"));
                    builder.AddData(hdr, "Checksum", header.Checksum.ToString("X4"));
                    builder.AddData(hdr, "IO Devices", GetIOString(header.IOSupport));


                }
            }

            /// <summary>
            /// Converts a series of IO support codes to a friendly format
            /// </summary>
            /// <param name="codes"></param>
            /// <returns></returns>
            private string GetIOString(string codes) {
                string description = string.Empty;
                bool unknownCodes = false;

                for (int i = 0; i < codes.Length; i++) {
                    var code = codes[i];
                    if (code != ' ') {
                        string device = GenHeader.GetIODevice(code);
                        if (device == null) {
                            unknownCodes = true;
                        } else {
                            if (description != string.Empty) description += ", ";
                            description += device;
                        }
                    }
                }

                if (unknownCodes) {
                    if (description != string.Empty) description += ", ";
                    description += "unknown codes";
                }

                if (description != string.Empty) {
                    return codes + " (" + description + ")";
                } else {
                    return codes;
                }
            }

            /// <summary>
            /// Attempts to decode company codes in a copyright string.
            /// </summary>
            /// <param name="headerCopyright"></param>
            /// <returns></returns>
            static string GetCopyrightString(string headerCopyright) {
                if (headerCopyright.StartsWith("(C)T", StringComparison.OrdinalIgnoreCase) & headerCopyright.Length >= 7) {
                    string code = headerCopyright.Substring(4, 3);
                    if(code[0] == '-') code = code.Substring(1);

                    int codeNum;
                    if(int.TryParse(code,out codeNum)){
                        for (int i = 0; i < GenHeader.CompanyCodes.Count; i++) {
                            if (GenHeader.CompanyCodes[i].Code == codeNum) {
                                return headerCopyright + " (" + GenHeader.CompanyCodes[i].Company + ")";
                            }
                        }
                    }

                    switch (headerCopyright.Substring(3, 4).ToUpper()) {
                        case "ACLD": return headerCopyright + "Ballistic";
                        case "ASCI": return headerCopyright + "Asciiware";
                        case "RSI": return headerCopyright + "Razorsoft";
                        case "TREC": return headerCopyright + "Treco";
                        case "VRGN": return headerCopyright + "Virgin Games";
                        case "WSTN": return headerCopyright + "Westone";
                    }
                }

                return headerCopyright;
            }


            public override int GetRomSize(byte[] rom) {
                return Gen.HasExternalHeader(rom) ? rom.Length - Gen.ExternalHeaderSize : rom.Length;
            }

            public override string GetRomFormatName(byte[] rom) {
                //return Gen.HasExternalHeader(rom) ? "Headered ROM (SMD/BIN)" : "Unheadered ROM (SMD/BIN)";
                bool iHeader,eHeader, interloven,sega32X;
                iHeader = Gen.HasInternalHeader(rom, out interloven, out eHeader, out sega32X);

                if (interloven) {
                    return eHeader ? "SMD (External header, Interleaved)" : "SMD (No header, Interleaved)";
                } else {
                    return eHeader ? "BIN (External header)" : "BIN";
                }
            }
            public override bool? HasHeader(byte[] rom) {
                return Gen.HasExternalHeader(rom);
            }


            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_gen;
                }
            }
        }

    }


}
