using HASH.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HASH
{
    /// <summary>
    /// Parses a ClrMamePro DAT
    /// </summary>
    /// <remarks>This class was originally written for a No-Intro-related project
    /// and may contain remnant references to No-Into</remarks>
    class ClrMameProParser
    {
        CmpReader reader;
        string document;
        RomDB result;
        DBConfig.Database info;

        private ClrMameProParser(DBConfig.Database info, string document, IList<Platforms> platform) {
            this.info = info;
            this.reader = new CmpReader(document);
            this.document = document;
            result = new RomDB(platform, info.Name);
        }

        /// <summary>
        /// Parses a DAT and returns a RomDB object containing ROM information and hashes.
        /// </summary>
        /// <param name="info">Database parameters</param>
        /// <param name="document">Document to parse</param>
        /// <param name="platform">A modifiable list object. Platform IDs for the database's supported platforms will be added to this list.</param>
        /// <returns>A parsed ROM database</returns>
        public static RomDB ParseDAT(DBConfig.Database info, string document, IList<Platforms> platform) {
            var parser = new ClrMameProParser(info, document, platform);
            return parser.Parse();
        }


        private RomDB Parse() {
            // Exceptions are caught by outside caller

            //try {
            while (reader.NextItem()) { // Keep looping over top-level items until we run out
                if (reader.SelectionName == "game") { // Is this a game entry?
                    TryParseGame(reader);
                } else if (reader.SelectionName == "clrmamepro") {
                    string version, comment;
                    TryParseDatHeader(reader, out version, out comment);

                    if (version != null) {
                        DateTime? date = null;
                        int y, m, d;
                        if (version.Length >= 8) {
                            bool validDate = true;
                            validDate &= int.TryParse(version.Substring(0, 4), out y);
                            validDate &= int.TryParse(version.Substring(4, 2), out m);
                            validDate &= int.TryParse(version.Substring(6, 2), out d);

                            if (validDate) date = new DateTime(y, m, d);
                        }

                        result.SetVersion(version, date);
                    }
                    if (comment != null) result.Comment = comment;
                }
            }
            //} catch (CmpDocumentException ex) {
            //    throw; // TxOxDxO: Error message to user!
            //}


            // So it goes like this: we only care about top-level groups named "game"
            // For each game group, we want the "name" value and some info from the "rom" subgroup:
            // "md5", "sha1", and "crc" (crc32)

            return result;
        }

        private void TryParseDatHeader(CmpReader reader, out string version, out string comment) {
            version = comment = null;

            if (reader.OpenItem()) {
                while (reader.NextItem()) {
                    if (reader.SelectionName == "version") {
                        if (version == null) version = reader.SelectionValue;
                    } else if (reader.SelectionName == "comment") {
                        if (comment == null) comment = reader.SelectionValue;
                    }
                }
            }
            reader.CloseItem();
        }
        private void TryParseGame(CmpReader reader) {
            if (reader.OpenItem()) {
                RomDB.Entry entry = new RomDB.Entry();

                string name = null;
                string crc32 = null;
                string sha1 = null;
                string md5 = null;
                string size = null;
                bool romRead = false;

                while (reader.NextItem()) {
                    if (reader.SelectionName == "name" & name == null) {
                        name = reader.SelectionValue;

                    } else if (reader.SelectionName == "rom" & !romRead) {
                        if (reader.OpenItem()) {
                            romRead = true;
                            while (reader.NextItem()) {
                                if (reader.SelectionName == "size" & size == null) {
                                    size = reader.SelectionValue;
                                } else if (reader.SelectionName == "crc" & crc32 == null) {
                                    crc32 = reader.SelectionValue;
                                } else if (reader.SelectionName == "sha1" & sha1 == null) {
                                    sha1 = reader.SelectionValue;
                                } else if (reader.SelectionName == "md5" && md5 == null) {
                                    md5 = reader.SelectionValue;
                                }
                            }
                        }
                        reader.CloseItem();
                    }


                }
                if (name != null) {
                    entry.name = name;
                    if (size != null) {
                        entry.size = ParseInt(size);
                    }
                    // Todo: report invalidly sized hashes?
                    if (crc32 != null) {
                        var crc32hash = new RomHash(ParseHexBytes(crc32), GetHashFlags(HashFlags.CRC32));
                        if (crc32hash.Value.Length == 4)
                            entry.Hashes.Add(crc32hash);
                    }
                    if (sha1 != null) {
                        var sha1Hash = new RomHash(ParseHexBytes(sha1), GetHashFlags(HashFlags.SHA1));
                        if (sha1Hash.Value.Length == 20)
                            entry.Hashes.Add(sha1Hash);
                    }
                    if (md5 != null) {
                        var md5Hash = new RomHash(ParseHexBytes(md5), GetHashFlags(HashFlags.MD5));
                        if (md5Hash.Value.Length == 16)
                            entry.Hashes.Add(md5Hash);
                    }

                }
                reader.CloseItem();

                result.AddEntry(entry);
            }
        }

        /// <summary>
        /// Given a HashFlags value, applies DB hints provided in DB config
        /// </summary>
        /// <remarks>Some Examples: CRC32 will be converted to RomHash | CRC32 for No-Intro,
        /// or RomHash_ByteSwapped | CRC32 for No-Intro N64</remarks>
        private HashFlags GetHashFlags(HashFlags hashFlags) {
            if ((hashFlags & (HashFlags.FileHash | HashFlags.RomHash)) == 0) {
                // File or ROM not specified? Use DB default.
                if ((info.Hints & DBHints.ByteSwapped) != 0) {
                    return hashFlags | HashFlags.RomHash_ByteSwap;
                } else if ((info.Hints & DBHints.DefaultHash_ROM) != 0) {
                    return hashFlags | HashFlags.RomHash;
                } else if ((info.Hints & DBHints.DefaultHash_File) != 0) {
                    return hashFlags | HashFlags.FileHash;
                }

                // No specified for DB? Guess.
                return hashFlags | HashFlags.FileHash;
            } else {
                return hashFlags;
            }
        }

        private static byte[] ParseHexBytes(string hex) {
            if ((hex.Length % 2) != 0) throw new ArgumentException("String is not a valid hex number");

            byte[] result = new byte[hex.Length / 2];

            int iResult = 0;
            for (int i = 0; i < hex.Length; i += 2) {
                result[iResult] = parsebyte(hex, i);
                iResult++;
            }

            return result;

        }

        private static byte parsebyte(string s, int index) {
            int result = 0;

            if (index + 2 > s.Length) throw new ArgumentException("String is not a valid hex number");

            char c = s[index];
            if (c >= '0' & c <= '9') {
                result = c - '0';
            } else if (c >= 'a' & c <= 'f') {
                result = c - 'a' + 10;
            } else if (c >= 'A' & c <= 'F') {
                result = c - 'A' + 10;
            } else {
                throw new ArgumentException("String is not a valid hex number");
            }

            // Move nibble to high
            result <<= 4;


            c = s[index + 1];
            if (c >= '0' & c <= '9') {
                result |= c - '0';
            } else if (c >= 'a' & c <= 'f') {
                result |= c - 'a' + 10;
            } else if (c >= 'A' & c <= 'F') {
                result |= c - 'A' + 10;
            } else {
                throw new ArgumentException("String is not a valid hex number");
            }

            return (byte)result;
        }

        private static ulong ParseHex64(string s, int index) {
            const int numdigits = 16;

            // If we aren't using the whole string, extract the part we want
            if (index > 0 || index + numdigits < s.Length) {
                // No more that 8 digits
                int len = Math.Min(s.Length - index, numdigits);
                s = s.Substring(index, len);
            }

            return UInt64.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }

        private static uint ParseHex32(string s, int index) {
            const int numdigits = 8;

            // If we aren't using the whole string, extract the part we want
            if (index > 0 || index + numdigits < s.Length) {
                // No more that 8 digits
                int len = Math.Min(s.Length - index, numdigits);
            }

            return uint.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }


        private static ulong ParseInt(string s) {
            return uint.Parse(s);
        }


        /// <summary>
        /// Provides low-level document navigation and parsing
        /// </summary>
        class CmpReader
        {
            string document;
            int currentLevel = 0;
            bool itemSelected = false;
            string currentItemName = null;
            string currentItemValue = null;
            CmpType currentItemType = CmpType.None;
            int cursorPos = 0;

            public CmpReader(string doc) {
                this.document = doc;

                //NextItem();
            }

            /// <summary>
            /// Moves to the next item in the current scope (skips over subitems of the current item). Returns
            /// false if there are no more items in the current scope.
            /// </summary>
            /// <returns>True if an item is selected after the operation.</returns>
            public bool NextItem() {
                // If we are currently pointing at the beginning of a group, we need to skip past it.
                if (currentItemType == CmpType.Group) {
                    // Do this by opening (half-assedly) the group, then closing it
                    cursorPos++;
                    currentLevel++;
                    CloseItem();
                }

                // Selection will be empty unless we find something
                UnselectItem();

                // Grab item name. If we didn't find an item name, set itemSelected to false.
                SkipWhitespace();
                if (cursorPos < document.Length) {
                    currentItemName = GrabIdentifier();
                }
                itemSelected = currentItemName != null;

                if (itemSelected) {
                    SkipWhitespace(); // Move to value

                    if (cursorPos >= document.Length) throw new CmpDocumentException("Item does not have a value.");
                    if (document[cursorPos] == '"') {
                        // string
                        cursorPos++;
                        var startOfString = cursorPos;
                        SkipPastString();
                        var strLen = cursorPos - startOfString - 1;
                        currentItemValue = document.Substring(startOfString, strLen);
                        currentItemType = CmpType.String;
                    } else if (document[cursorPos] == '(') {
                        // Group
                        currentItemType = CmpType.Group;
                    } else {
                        // Unquoted string ("Literal")
                        var value = GrabLiteral();
                        if (value == null) throw new CmpDocumentException("Item does not have a value.");
                        currentItemValue = value;
                        currentItemType = CmpType.Literal;
                    }

                } else {
                    // If we didn't find an item, one of the following is true
                    // - We are at the end of a group (a closing paren must be present)
                    // - We are at the end of the document (cursor points past last char)
                    // - There is a syntax error

                    if (cursorPos < document.Length) {
                        if (document[cursorPos] != ')')
                            throw new CmpDocumentException("Unexpected text in document at position " + cursorPos.ToString());
                    } else {
                        // If we reached the end of the document and there is a group that wasn't closed, that's an error
                        if (currentLevel > 0)
                            ErrEndOfDoc();
                    }
                }

                return itemSelected;
            }

            private void UnselectItem() {
                currentItemName = currentItemValue = null;
                currentItemType = CmpType.None;
                itemSelected = false;
            }

            /// <summary>
            /// If the current selection is a group, moves the selection inside the currently selected item's child scope, immediately before the first child.
            /// </summary>
            /// <returns>True if the selected item was a group, otherwise false.</returns>
            public bool OpenItem() {
                if (currentItemType != CmpType.Group) return false;

                // Redundant/sanity check
                if (cursorPos >= document.Length) ErrEndOfDoc();
                if (document[cursorPos] != '(') throw new CmpDocumentException("Item is not a group");

                UnselectItem(); // Important! When we call NextItem

                cursorPos++; // Eat up the opening paren
                SkipWhitespace();
                currentLevel++;
                return true;
            }
            /// <summary>
            /// Moves the selection outside the current scope, immediately before the next item in the outer scope.
            /// </summary>
            public void CloseItem() {
                UnselectItem();

                // If we aren't inside a group, we in effect close the document
                if (currentLevel == 0) {
                    currentLevel = -1;
                    cursorPos = document.Length;
                    return;
                }

                // Otherwise scan for unmatched closing paren
                int relativeLevel = 0; // Need to seek to encompassing scope, relative level -1

                while (cursorPos < document.Length) {
                    char c = document[cursorPos];

                    if (c == '"') {
                        cursorPos++;
                        SkipPastString();
                    } else if (c == '(') {
                        relativeLevel++;
                        cursorPos++;
                    } else if (c == ')') {
                        relativeLevel--;
                        cursorPos++;
                    } else {
                        cursorPos++;
                    }


                    if (relativeLevel == -1) {
                        currentLevel--;
                        return;
                    }
                }

                throw new CmpDocumentException("Group item missing closing bracket");
            }

            /// <summary>
            /// The cursor should be placed AFTER the opening quote. This function seeks to past the closing quote.
            /// </summary>
            private void SkipPastString() {
                while (cursorPos < document.Length) {
                    char c = document[cursorPos];
                    cursorPos++;

                    if (c == '"') return;
                }
                throw new CmpDocumentException("String not terminated with quote");
            }

            private void SkipWhitespace() {
                while (cursorPos < document.Length && char.IsWhiteSpace(document[cursorPos])) {
                    cursorPos++;
                }
            }
            /// <summary>
            /// Parses an identifier and moves the cursor past it. Returns null if the cursor is not at an identifier.
            /// </summary>
            /// <returns></returns>
            private string GrabIdentifier() {
                if (cursorPos >= document.Length) return null;
                if (!char.IsLetter(document[cursorPos])) return null;

                int idStart = cursorPos;
                cursorPos++;

                char c;

                // Seek to end of identifier
                while (cursorPos < document.Length && (char.IsLetter((c = document[cursorPos])) || char.IsDigit(c))) { // CAUTION: side effects!
                    cursorPos++;
                }

                return document.Substring(idStart, cursorPos - idStart);
            }
            /// <summary>
            /// Parses an identifier and moves the cursor past it. Returns null if the cursor is not at an identifier.
            /// </summary>
            /// <returns></returns>
            private string GrabLiteral() {
                if (cursorPos >= document.Length) return null;

                int idStart = cursorPos;
                cursorPos++;

                char c;

                // Seek to end of literal (whitespace, "(", or ")")
                while (cursorPos < document.Length && ((!char.IsWhiteSpace(c = document[cursorPos])) && (c != '(') && c != ')')) { // CAUTION: side effects!
                    cursorPos++;
                }

                return document.Substring(idStart, cursorPos - idStart);
            }
            private static void ErrEndOfDoc() {
                throw new CmpDocumentException("Document ended unexpectedly");
            }

            /// <summary>
            /// Gets the current selection's node type, or CmpType.None if nothing is selected.
            /// </summary>
            public CmpType SelectionType { get { return currentItemType; } }
            /// <summary>
            /// Returns true if the current selection is a group
            /// </summary>
            public bool SelectionIsGroup { get { return currentItemType == CmpType.Group; } }
            /// <summary>
            /// Returns the name of the currently selected node, if a node is selected.
            /// </summary>
            public string SelectionName { get { return currentItemName; } }
            /// <summary>
            /// Returns the currently selected node's value if a node is selected that is not a group.
            /// </summary>
            public string SelectionValue { get { return currentItemValue; } }
            /// <summary>
            /// Returns true if a node is currently selected.
            /// </summary>
            public bool ItemSelected { get { return itemSelected; } }
        }

        /// <summary>
        /// Thrown when an error occurs while parsing a ClrMamePro DAT
        /// </summary>
        [Serializable()]
        public class CmpDocumentException : Exception
        {
            public CmpDocumentException() {
                // Add any type-specific logic, and supply the default message.
            }

            public CmpDocumentException(string message)
                : base(message) {
                // Add any type-specific logic.
            }
            public CmpDocumentException(string message, Exception innerException) :
                base(message, innerException) {
                // Add any type-specific logic for inner exceptions.
            }
            protected CmpDocumentException(SerializationInfo info,
               StreamingContext context)
                : base(info, context) {
                // Implement type-specific serialization constructor logic.
            }
        }

        /// <summary>
        /// Enumerates ClrMamePro DAT node types
        /// </summary>
        public enum CmpType
        {
            /// <summary>
            /// No node
            /// </summary>
            None,
            /// <summary>
            /// An un-quoted text value
            /// </summary>
            Literal,
            /// <summary>
            /// A quoted text value
            /// </summary>
            String,
            /// <summary>
            /// A node with child nodes
            /// </summary>
            Group
        }

    }
}
