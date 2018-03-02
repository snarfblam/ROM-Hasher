using HASH.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HASH
{
    /// <summary>
    /// Provides functions and file paths used for file operations in the program
    /// </summary>
    static class FileSystem
    {
        /// <summary>
        /// Path to the startup executable.
        /// </summary>
        internal static string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        /// <summary>
        /// Path to the ROM database folder.
        /// </summary>
        internal static string DbPath = System.IO.Path.Combine(AppPath, "DBs"); 
        
        /// <summary>
        /// Program configuration filename
        /// </summary>
        const string ConfigFileName = "settings.config";
        /// <summary>
        /// PRogram configuration path.
        /// </summary>
        public static string ConfigFilePath = Path.Combine(AppPath, ConfigFileName);
        /// <summary>
        /// Database configuration filename
        /// </summary>
        const string dbConfigFilename = "databases.config";
        /// <summary>
        /// Database configuration path
        /// </summary>
        public static string dbConfigFilePath = Path.Combine(AppPath, dbConfigFilename);

        /// <summary>
        /// Gets the absolute path for the specified database
        /// </summary>
        /// <param name="db">Database</param>
        /// <returns>The path of the database</returns>
        internal static string GetDbFilePath(DBConfig.Database db) {
            if (Path.IsPathRooted(db.Filename))
                return db.Filename;

            return Path.Combine(DbPath, db.Filename);
        }

        /// <summary>
        /// Returns the contents of the specified file, or null if there is an error.
        /// </summary>
        /// <param name="path">That path, not including the filename.</param>
        /// <param name="file">Filename.</param>
        /// <param name="error">Error details, or null if there is no error.</param>
        /// <returns>contents of the specified file, or null if there is an error.</returns>
        internal static string ReadAllText(string path, string file, out FsError error) {
            error = null;

            string fullPath = (file == null) ? path : Path.Combine(path, file);
            FsErrorType err = FsErrorType.Unspecified;

            try {
                return File.ReadAllText(fullPath);
            } catch (FileNotFoundException) {
                err = FsErrorType.FileNotFound;
            } catch (DirectoryNotFoundException) {
                err = FsErrorType.DirectoryNotFound;
            } catch (PathTooLongException) {
                err = FsErrorType.PathTooLong;
            } catch (NotSupportedException) {
                err = FsErrorType.InvalidPath;
            } catch (System.Security.SecurityException) {
                err = FsErrorType.SecurityException;
            } catch (IOException) {
                err = FsErrorType.IOError;
            } catch (ArgumentException) {
                err = FsErrorType.InvalidPath;
            }

            error = new FsError(fullPath, err);
            return null;
        }

        /// <summary>
        /// Performs the action and swallows file system exceptions AS WELL AS
        /// SOME EXCEPTIONS THAT MAY NOT BE CAUSE BY FILE SYSTEM. Use with caution.
        /// </summary>
        /// <param name="a"></param>
        public static void IgnoreFileErrors(FileAction a) {
            try {
                a();
            } catch (FileNotFoundException) {
                return;
            } catch (DirectoryNotFoundException) {
                return;
            } catch (PathTooLongException) {
                return;
            } catch (NotSupportedException) {
                return;
            } catch (System.Security.SecurityException) {
                return;
            } catch (IOException) {
                return;
            } catch (ArgumentException) {
                return;
            }
        }

        /// <summary>
        /// Performs the action and swallows file system exceptions AS WELL AS
        /// SOME EXCEPTIONS THAT MAY NOT BE CAUSE BY FILE SYSTEM. Use with caution.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="error"></param>
        public static void PerformFileAction(FileAction a, out FsError error) {
            error = null;

            FsErrorType err = FsErrorType.Unspecified;

            try {
                a();
                return;
            } catch (FileNotFoundException) {
                err = FsErrorType.FileNotFound;
            } catch (DirectoryNotFoundException) {
                err = FsErrorType.DirectoryNotFound;
            } catch (PathTooLongException) {
                err = FsErrorType.PathTooLong;
            } catch (NotSupportedException) {
                err = FsErrorType.InvalidPath;
            } catch (System.Security.SecurityException) {
                err = FsErrorType.SecurityException;
            } catch (IOException) {
                err = FsErrorType.IOError;
            } catch (ArgumentException) {
                err = FsErrorType.InvalidPath;
            }

            error = new FsError(string.Empty, err);
        }

        /// <summary>
        /// Delegate type for basic file actions
        /// </summary>
        public delegate void FileAction();


        /// <summary>
        /// Modifies the filename of the specified path to reflect a filename that is not currently in use
        /// </summary>
        /// <param name="filepath">File path to modify</param>
        /// <returns>A file path with a filename that is not used by an existing file</returns>
        internal static string GetUniqueFilename(string filepath) {
            if (!File.Exists(filepath)) return filepath;

            string dir = Path.GetDirectoryName(filepath);
            string name = Path.GetFileNameWithoutExtension(filepath);
            string ext = Path.GetExtension(filepath);

            int index = 0;
            string newname;
            do {
                index++;
                newname = Path.Combine(dir, name) +" " + index.ToString() +  ext;
            } while (File.Exists(newname));

            return newname;
        }

        /// <summary>
        /// Attempts to convert the an absolute path to a relative path. If the path can not be converted,
        /// the absolute path is returned. If necessary use the Path.FullPath function to ensure that the
        /// 'relativeTo' parameter is rooted.
        /// </summary>
        /// <param name="relativeTo"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string MakePathRelative(string relativeTo, string path) {
            bool endsWithSeparator = relativeTo.EndsWith(Path.DirectorySeparatorChar.ToString()) || relativeTo.EndsWith(Path.AltDirectorySeparatorChar.ToString());
            if (!endsWithSeparator) relativeTo += Path.DirectorySeparatorChar;

            // If paths aren't on same share or drive, we can't make a relative path
            string rootRelativeTo = Path.GetPathRoot(relativeTo);
            string rootPath = Path.GetPathRoot(path);
            if (!rootRelativeTo.Equals(rootPath, StringComparison.OrdinalIgnoreCase)) {
                return path;
            }
            

            Uri uriRelativeTo = new Uri(relativeTo);
            Uri uriPath = new Uri(path);

            Uri relative ;
            try {
                relative = uriRelativeTo.MakeRelativeUri(uriPath);
            } catch (InvalidOperationException) {
                // Thrown if 'relativeTo' is not absolute
                return path;
            }

            string result = Uri.UnescapeDataString(relative.ToString());

            return result.Replace('/', Path.DirectorySeparatorChar);

        }
    }

    /// <summary>
    /// Represents a file system error
    /// </summary>
    class FsError{
        public FsError(string path, FsErrorType error) {
            this.Path = path;
            this.Error = error;
        }
        /// <summary>
        /// The path that produced the error. This value may not be available.
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// The nature of the error.
        /// </summary>
        public FsErrorType Error { get; set; }
    }

    enum FsErrorType{
        Unspecified,
        FileNotFound,
        InvalidPath,
        PathTooLong,
        DirectoryNotFound,
        UnauthorizedAccess,
        SecurityException,
        IOError
    }

}
