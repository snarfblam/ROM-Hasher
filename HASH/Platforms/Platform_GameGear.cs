using System;
using System.Collections.Generic;
using System.Text;

namespace HASH
{
    abstract partial class Platform
    {
        /// <summary>
        /// Defines Platform object for Game Gear. Most functionality is inherited from Platform_MasterSystem
        /// </summary>
        /// <remarks>For documentation, see Platform class</remarks>
        class Platform_GameGear : Platform_MasterSystem
        {
            public Platform_GameGear() {
                KnownExtensions = new string[] { "gg" };
            }


            public override Platforms ID {
                get { return HASH.Platforms.GameGear; }
            }

            public override string GetRomFormatName(byte[] rom) {
                return "Game Gear ROM";
            }
            public override System.Drawing.Image SmallPlatformImage {
                get {
                    return Res.logo_gamegear;
                }
            }
        }
    }
}