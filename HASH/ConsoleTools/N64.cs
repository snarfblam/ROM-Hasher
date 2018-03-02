using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace HASH.ConsoleTools
{
    /// <summary>
    /// Contains Utility methods related to Nintendo64
    /// </summary>
    static class N64
    {
        /// <summary>
        /// First four bytes of a normal N64 ROM
        /// </summary>
        public static readonly byte[] N64Identifier = { 0x80, 0x37, 0x12, 0x40 };
        /// <summary>
        /// First four bytes of a byte-swapped N64 ROM
        /// </summary>
        public static readonly byte[] N64Identifier_Byteswapped = { 0x37, 0x80, 0x40, 0x12 };

        /// <summary>
        /// Returns a value indicating whether the ROM is byteswapped, or a value
        /// of 'Unknown' if it can not be determined or the ROM is not an N64 ROM.
        /// </summary>
        /// <param name="rom">ROM image</param>
        /// <returns></returns>
        public static N64ByteSwap IsByteswapped(byte[] rom) {
            if (rom.Length < 4) return N64ByteSwap.Unknown;

            bool match = true;
            for (int i = 0; i < N64Identifier.Length; i++) {
                if (N64Identifier[i] != rom[i]) match = false;
            }

            if (match) return N64ByteSwap.NotSwapped;

            match = true;
            for (int i = 0; i < N64Identifier_Byteswapped.Length; i++) {
                if (N64Identifier_Byteswapped[i] != rom[i]) match = false;
            }

            if (match) return N64ByteSwap.Swapped;

            return N64ByteSwap.Unknown;
        }

        /// <summary>
        /// Byte-swaps a ROM in-place
        /// </summary>
        /// <param name="rom">ROM image to byte-swap</param>
        internal static void SwapBytes(byte[] rom) {
            
            byte swap;

            for (int i = 0; i < rom.Length; i+= 2) {
                swap = rom[i];
                rom[i] = rom[i + 1];
                rom[i + 1] = swap;
            }
        }
    }
    /// <summary>
    /// Descibes the layout of an N64 ROM
    /// </summary>
    enum N64ByteSwap
    {
        /// <summary>
        /// It is now known whether the ROM is byte-swapped
        /// </summary>
        [Description("Unknown")]
        Unknown,
        /// <summary>
        /// The ROM is not byte-swapped
        /// </summary>
        [Description("Not Byte-swapped")]
        NotSwapped,
        /// <summary>
        /// The ROM is byte-swapped
        /// </summary>
        [Description("Byte-swapped")]
        Swapped
    }
}
