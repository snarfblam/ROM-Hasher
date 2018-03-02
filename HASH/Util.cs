using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace HASH
{
    /// <summary>
    /// Provides general purpose utility and extension methods
    /// </summary>
    static class Util
    {
        /// <summary>
        /// Returns the description attribute of the specified enum value, or the name of the enum value if there is no description.
        /// </summary>
        /// <param name="e">Enum value</param>
        public static string GetDescription(this Enum e) {
            Type t = e.GetType();
            string name = e.ToString();

            var field = t.GetField(name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Public);
            if (field != null) {
                var atts = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (atts.Length > 0) return atts[0].Description;
            }
            return name;
        }

        /// <summary>
        /// Deselects toolstrip buttons in the specified toolstrip to address a bug in WinForms
        /// </summary>
        /// <param name="strip">ToolStrip to deselect buttons on</param>
        public static void DeselectButtons(this System.Windows.Forms.ToolStrip strip) {
            if (!Program.RunningOnMono) {
                typeof(System.Windows.Forms.ToolStrip).InvokeMember(
                    "ClearAllSelections", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, 
                    null, strip, null);
                    //.GetMethod("ClearAllSelections", BindingFlags.NonPublic | BindingFlags.Instance);
                //method.Invoke(yourToolStripName, null);
            }
        }

        /// <summary>
        /// Clamps a value
        /// </summary>
        /// <param name="i">Input value</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximmum</param>
        /// <returns>The specified value clamped to the given range</returns>
        public static int Clamp(this int i, int min, int max) {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }
        /// <summary>
        /// Interpolates a floating point value (linear)
        /// </summary>
        /// <param name="from">The 'lower' value</param>
        /// <param name="to">The 'upper' value</param>
        /// <param name="ratio">Interpolation ratio where zero results in the initial value and one results in the 'to' value</param>
        /// <returns>A floating point value</returns>
        public static float Interp(this float from, float to, float ratio) {
            return to + (to - from) * ratio;
        }
        /// <summary>
        /// Raises an event. Syntactic sugar.
        /// </summary>
        /// <param name="e">Event delegate</param>
        /// <param name="sender">Sender</param>
        public static void Raise(this EventHandler e, object sender) {
            if (e != null) e(sender, EventArgs.Empty);
        }
        /// <summary>
        /// Raises a generic event. Syntactic sugar.
        /// </summary>
        /// <typeparam name="T">Event handler type</typeparam>
        /// <param name="e">Event delegate</param>
        /// <param name="sender">Sender</param>
        /// <param name="eventArgs">Event parameters</param>
        public static void Raise<T>(this EventHandler<T> e, object sender, T eventArgs) where T:EventArgs {
            if (e != null) e(sender, eventArgs);
        }
        /// <summary>
        /// Converts a boolean to a yes or no string.
        /// </summary>
        /// <param name="value">Boolean value.</param>
        /// <returns>A yes or no string.</returns>
        public static string AsYesNo(this bool value) {
            return value ? "Yes" : "No";
        }

        /// <summary>
        /// Parses an ASCII string from a buffer. See remarks.
        /// </summary>
        /// <param name="data">Byte array containing ASCII data.</param>
        /// <param name="offset">Offset of the ASCII data.</param>
        /// <param name="length">Maximum length of the ASCII data.</param>
        /// <returns>A string containing the parsed text, or String.Empty if no text could be retreived.</returns>
        /// <remarks>If the specified offset is out of bounds an empty string is returned. Otherwise, ASCII is parsed until
        /// the first of the following occurs: end of buffer is reached, parsed data reaches specified length, or a null character
        /// is parsed. The null character will not be included in the returned value, if present.</remarks>
        public static string ParseAscii(byte[] data, int offset, int length) {
            // Parse ASCII for title
            byte[] titleBytes = new byte[length];

            length = Math.Min(length, data.Length - offset);
            if (length > 0 && offset >= 0) {
                Array.Copy(data, offset, titleBytes, 0, length);
                var result = new string(System.Text.Encoding.ASCII.GetChars(titleBytes));

                // Check for null terminator in title
                if (result.IndexOf('\0') >= 0) { result = result.Substring(0, result.IndexOf('\0')); }

                return result;
            } else {
                return string.Empty;
            }
        }
    }
}
