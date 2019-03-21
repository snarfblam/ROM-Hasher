using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using HASH.Config;

namespace HASH
{
    /// <summary>
    /// ROM Hasher!
    /// </summary>
    static class Program
    {
        static ProgramConfig _Config = ProgramConfig.CreateDefaultConfig();
        static DBConfig _DatabaseConfig = new DBConfig();
        
        /// <summary>
        /// Gets the current program configuration.
        /// </summary>
        /// <remarks>This will be persisted to a settings file in the application directory.</remarks>
        public static ProgramConfig Config { get { return _Config; } private set { _Config = value; } }
        /// <summary>
        /// Gets the current database configuration.
        /// </summary>
        /// <remarks>This will be persisted to a settings file in the application directory.</remarks>
        public static DBConfig DatabaseConfig { get { return _DatabaseConfig; } private set { _DatabaseConfig = value; } }

        static bool _mono = Type.GetType ("Mono.Runtime") != null;
        /// <summary>
        /// Returns true if the program is running in the Mono runtime
        /// </summary>
        public static bool RunningOnMono { get { return _mono; } }
        static bool _win7OrHigher = ! _mono && Environment.OSVersion.Version.Major >= 6;
        /// <summary>
        /// Returns true if the program is running in Windows 7 or higher
        /// </summary>
        public static bool WinVersion7OrHigher { get { return _win7OrHigher; } }

        /// <summary>
        /// A void, parameterless function pointer.
        /// </summary>
        public delegate void Task();

        /// <summary>Stores a list of pending tasks to be executed on application idle.</summary>
        static Queue<Task> TaskQueue = new Queue<Task>();
        /// <summary>Enqueues a task to be performed when the message queue clears.</summary>
        /// <param name="task">Task to execute upon application idle.</param>
        public static void QueueTask(Task task) { TaskQueue.Enqueue(task); }

        [STAThread]
        static void Main(string[] args) {
            bool terminate;
            
            LoadConfig(out terminate);
            if (terminate) return;

            LoadDBConfig(out terminate);
            if (terminate) return;


            if (args.Length == 0) {
                // For our application-idle task queue
                Application.Idle += new EventHandler(Application_Idle);

                // Called before RomDB.LoadDBs so if there is an error, the error window looks pretty. Yes, really.
                Application.EnableVisualStyles();

                // Load and parse all DBs
                RomDB.LoadDBs();

                Application.Run(new HashForm());
            }

            SaveConfig();
            // Database config is save when (and only when) user re-configured DBs

        }

        static void Application_Idle(object sender, EventArgs e) {
            // Execute all pending task items
            while (TaskQueue.Count > 0) {
                TaskQueue.Dequeue()();
                Application.DoEvents();
            }
        }



        private static void CreateDefaultDbConfig() {
            MessageBox.Show("The database configuration file was not found or could not be loaded.");
            // I can't make a DB config out of thin air! What was I thinking?
            throw new NotImplementedException();
        }

        private static void SaveConfig() {
            XmlSerializer configSerial = new XmlSerializer(typeof(ProgramConfig));
            using (var file = File.Open(FileSystem.ConfigFilePath, FileMode.Create)) {
                configSerial.Serialize(file, Config);
            }

        }
        private static void SaveDBConfig() {
            XmlSerializer dbConfigSerial = new XmlSerializer(typeof(DBConfig));
            using (var file = File.Open(FileSystem.dbConfigFilePath, FileMode.Create)) {
                dbConfigSerial.Serialize(file, DatabaseConfig);
            }


        }

        private static void LoadConfig(out bool TerminateApplication) {
            TerminateApplication = false;

            if (File.Exists(FileSystem.ConfigFilePath)) {
                XmlSerializer configSerial = new XmlSerializer(typeof(ProgramConfig));
                using (var file = File.Open(FileSystem.ConfigFilePath, FileMode.Open)) {
                    try {
                        Config = (ProgramConfig)configSerial.Deserialize(file);
                    } catch (InvalidOperationException ex) {
                        var msgResult = MessageBox.Show(
                            "There was an error loading program settings. Click OK to reset the config" + Environment.NewLine +
                            "file and open the program.",
                            "Warning", MessageBoxButtons.OKCancel);
                        if (msgResult == DialogResult.Cancel) {
                            TerminateApplication = true;
                            return;
                        }
                    }

                    if (Config != null) {
                        Config.UpdateToCurrentVersion();
                    }
                }
            }
        }
        private static void LoadDBConfig(out bool TerminateApplication) {
            TerminateApplication = false;

            if (File.Exists(FileSystem.dbConfigFilePath)) {
                XmlSerializer dbconfigSerial = new XmlSerializer(typeof(DBConfig));
                using (var file = File.Open(FileSystem.dbConfigFilePath, FileMode.Open)) {
                    try {
                        DatabaseConfig = (DBConfig)dbconfigSerial.Deserialize(file);
                    } catch (InvalidOperationException ex) {
                        var msgResult = MessageBox.Show(
                            "There was an error loading database settings. Click OK to reset the settings" + Environment.NewLine +
                            "and open the program.",
                            "Warning", MessageBoxButtons.OKCancel);

                        if (msgResult == DialogResult.Cancel) {
                            TerminateApplication = true;
                            return;
                        } else {
                            CreateDefaultDbConfig();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Displays a database config dialog to the users, and re-loads databases when the dialog is dismissed.
        /// </summary>
        internal static void ConfigureDatabases() {
            (new frmDBConfig()).ShowDialog();

            RomDB.UnloadDbs();
            Program.SaveConfig();
            SaveDBConfig();

            RomDB.LoadDBs();
        }


    }
}
