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
        /// Defines Platform object for SNES
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        private class Platform_SNES : Platform
        {
            internal Platform_SNES() {
                KnownExtensions = "smc swc sfc fig".Split(' ');
            }

            public override bool IsPlatformMatch(byte[] rom) {
                if (rom.Length < 0x2000) return false;

                if (rom.Length < 4200000 && rom.Length >= 0x8000) { // Too slow for massive ROMS
                    var checksum = SNES.CalculateChecksum(rom);

                    if (checksum != 0 && checksum == SNES.GetInternalChecksum(rom)) return true;
                }
                if (SNES.HasGoodSmcHeader(rom)) return true;
                if (SNES.HasGoodSwcHeader(rom)) return true;

                return false;
            }

            public override Platforms ID { get { return HASH.Platforms.SuperNES; } }

            public override void CalculateHashes(byte[] rom, IHashWorkManager worker, float startProgress, float endProgress) {
                iNesHeader header = null;
                if (rom.Length > 0x10) {
                    header = new iNesHeader(rom, 0);
                }

                if (SNES.HasExternalHeader(rom)) { // Headered
                    worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash);

                    // Calculate headerless hashes 
                    int ROMSize = rom.Length - SNES.ExternalHeaderSize;
                    ROMSize = Math.Min(ROMSize, rom.Length - SNES.ExternalHeaderSize);

                    worker.AddHashes(rom, SNES.ExternalHeaderSize, ROMSize, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.RomHash);
                } else {
                    // Unheadered (file hash IS rom hash)
                    worker.AddHashes(rom, 0, rom.Length, () => worker.AbortPending, RomHash.AllHashAlgorithms, HashFlags.FileHash, HashFlags.RomHash);
                }


            }


            protected override void AddPlatformExtendedData(RomExDataBuilder builder, byte[] rom, RomData data) {
                var checksum = SNES.CalculateChecksum(rom);
                int romOffset = SNES.HasExternalHeader(rom) ? SNES.ExternalHeaderSize : 0;

                string header = RomExDataBuilder.HeaderCat;

                object o_snesheader;
                SnesHeader snesheader;
                if(data.MiscData.TryGetValue(DataTags.DecodedHeader, out o_snesheader)) {
                //if (snesheader != null) {
                    snesheader =  o_snesheader as SnesHeader;

                    builder.AddData(RomExDataBuilder.GeneralCat, "Checksum valid", (checksum == snesheader.Checksum) ? "Yes" : "No");

                    builder.AddData(header, "Mapping", snesheader.Mapping.ToString());

                    //builder.AddData(header,"Name", GetAscii
                }
            }


            internal override void InitRomData(RomData data, byte[] rom) {
                base.InitRomData(data, rom);

                int romOffset = SNES.HasExternalHeader(rom) ? SNES.ExternalHeaderSize : 0;

                if (rom.Length - romOffset >= SnesHeader.MinimumRomSize) {
                    SnesHeader snesheader = new SnesHeader(rom, romOffset);
                    data.MiscData.Add(DataTags.DecodedHeader, snesheader);
                }
                
            }

            public override int GetRomSize(byte[] rom) {
                return SNES.HasExternalHeader(rom) ? rom.Length - SNES.ExternalHeaderSize : rom.Length;
            }

            public override string GetRomFormatName(byte[] rom) {
                return SNES.HasExternalHeader(rom) ? "Super Magicom" : "SNES rom image";
            }
            public override bool? HasHeader(byte[] rom) {
                return SNES.HasExternalHeader(rom);
            }

            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_snes;
                }
            }
        }


    }


}
