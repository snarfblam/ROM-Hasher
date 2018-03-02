using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;

namespace HASH
{
    /// <summary>
    /// Manages a ROM hashing and detailing operation
    /// </summary>
    class HashJob:IHashWorkManager
    {
        /// <summary>
        /// Path of the ROM file
        /// </summary>
        public string RomPath { get; private set; }
        /// <summary>
        /// Main program UI
        /// </summary>
        public HashForm MainForm { get; private set; }

        /// <summary>
        /// Gets the initialized RomData object when the operation is complete
        /// </summary>
        public RomData Result { get; private set; }
        /// <summary>
        /// Gets the game's platform when the operation is complete
        /// </summary>
        public Platform Platform { get; private set; }
        /// <summary>
        /// True if the operation is in progress
        /// </summary>
        public bool Working { get; private set; }
        /// <summary>
        /// True if the operation has completed successfully
        /// </summary>
        public bool Complete { get; private set; }
        /// <summary>
        /// True if the operation was aborted before completion
        /// </summary>
        public bool Aborted { get; private set; }
        /// <summary>
        /// True if an abort request is pending. The operation may still complete successfully if
        /// the abort request isn't processed in time.
        /// </summary>
        public bool AbortPending { get; private set; }
        /// <summary>
        /// After completion, returns a list of calculated hashes
        /// </summary>
        public IList<RomHash> Hashes { get; private set; }
        List<RomHash> _Hashes = new List<RomHash>();
        object HashesLock = new object();

        /// <summary>
        /// Gets a value between 0 and 1 indicating the percentage of completion.
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// Busy animation UI
        /// </summary>
        private frmBusy BusyForm { get; set; }
        /// <summary>
        /// If true, the user will be prompted to select the platform, even if it could be inferred
        /// </summary>
        public bool PromptForPlatform { get; private set; }

        /// <summary>
        /// True if the ROM is considered secondary and extended data/DB matches are not required
        /// </summary>
        public bool IsSecondaryROM { get; private set; }

        /// <summary>
        /// Used as a lock for changes in operation status (begin, abort, complete)
        /// </summary>
        private object StatusLock = new object();
        /// <summary>
        /// Used to manage the function from a non-UI thread
        /// </summary>
        private BackgroundWorker worker = new BackgroundWorker();

        /// <summary>
        /// Times the operation progress so that the busy animation can be displayed after a significant delay
        /// </summary>
        Stopwatch workTimer = new Stopwatch();
        frmBusy busyForm = null;
        /// <summary>
        /// If true, Busy form will be shown ASAP (for jobs that will very likely run long)
        /// </summary>
        bool immediateBusyForm = false; 

        /// <summary>
        /// Raised when the operation completes successfully
        /// </summary>
        public event EventHandler WorkComplete;
        /// <summary>
        /// Raised when progress is reported by the operation
        /// </summary>
        public event EventHandler WorkProgressed;
        /// <summary>
        /// Raised when progress terminates without success
        /// </summary>
        public event EventHandler WorkAborted;

        /// <summary>
        /// Prepares a hashing operation
        /// </summary>
        /// <param name="mainform">Main program UI</param>
        /// <param name="romPath">Path to ROM file</param>
        /// <param name="promptForPlatform">Specify true to prompt the user for the platform instead of inferring it</param>
        /// <param name="isSecondary">Specify true to avoid calculating extraneous data that is only needed for the primary ROM</param>
        public HashJob(HashForm mainform, string romPath, bool promptForPlatform, bool isSecondary) {
            this.RomPath = romPath;
            this.MainForm = mainform;
            this.PromptForPlatform = promptForPlatform;
            this.IsSecondaryROM = isSecondary;

            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        /// <summary>
        /// Begins the operation.
        /// </summary>
        public void DoWork() {
            lock (StatusLock) {
                if (Working | Complete | Aborted) throw new InvalidOperationException("The worker is already busy or completed.");

                Working = true;
                workTimer.Start();
            }

            worker.RunWorkerAsync();

        }

        /// <summary>
        /// Attempts to cancel the work. If the worker is
        /// already finished when it processes the abort request,
        /// the job will still complete successfully.
        /// </summary>
        public void Abort() {
            lock (StatusLock) {
                // Don't throw an exception when aborting a completed job, because caller may have just lost a race with the worker
                if (Complete) return;
                // Do throw if called when work hasn't even begun
                if (!Working) throw new InvalidOperationException("Can not abort a non-running worker.");

                AbortPending = true;
            }
        }



        void worker_DoWork(object sender, DoWorkEventArgs e) {
            // Do stuff!

            Hashes = _Hashes.AsReadOnly();

            var rom = File.ReadAllBytes(RomPath);
            if (rom.Length > Program.Config.FileSizeForImmediateBusyWindow) {
                immediateBusyForm = true;
                UpdateBusyDisplay();
            }

            Platform platform = null;
            string miscPlatform = null;

            if (PromptForPlatform) {
                RomData.GetRomPlatform(rom, Path.GetExtension(RomPath).TrimStart('.'), "Select a platform.", "Select Platform", out platform, out miscPlatform);
            } else {
                RomData.GetRomPlatform(rom, Path.GetExtension(RomPath).TrimStart('.'), out platform, out miscPlatform);
            }

            bool userCancelledDialog = (platform == null); // See RomData.GetRomPlatform documentation

            if (userCancelledDialog) {
                this.Platform = null;
                Result = null;
            } else {
                this.Platform = platform;
                Result = RomData.GetRomData(rom, platform, miscPlatform, !IsSecondaryROM, !IsSecondaryROM, this);
            }

            lock (StatusLock) {
                Working = false;
                if (Result == null)
                    Aborted = true;
                else
                    Complete = true;
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            MainForm.Invoke((SimpleDelegate)(() => WorkProgressed.Raise(this)));
            UpdateBusyDisplay();
        }


        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (Complete) {
                MainForm.Invoke((SimpleDelegate)(() => WorkComplete.Raise(this)));
            } else {
                MainForm.Invoke((SimpleDelegate)(() => WorkAborted.Raise(this)));
            }
            CloseBusyDisplay();
            workTimer.Stop();
       }

        /// <summary>
        /// Can be called from any thread
        /// </summary>
        private void UpdateBusyDisplay() {
            // If operation is taking too long and the busy window isn't showing, show it
            if (busyForm == null) {
                if (immediateBusyForm || workTimer.ElapsedMilliseconds > Program.Config.BusyWindowDelay) {
                    MainForm.Invoke((SimpleDelegate)delegate() {
                        busyForm = new frmBusy();
                        busyForm.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                        busyForm.Left = MainForm.Left + MainForm.Width / 2 - busyForm.Width / 2;
                        busyForm.Top = MainForm.Top + MainForm.Height / 2 - busyForm.Height / 2;
                        busyForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(busyForm_FormClosed);
                    });

                    MainForm.BeginInvoke((SimpleDelegate)delegate() {

                        if (Working) {
                            busyForm.ShowDialog(MainForm);
                        } else {
                            busyForm.Dispose();
                            busyForm = null;
                        }
                    });
                }
            }
        }

        /// <summary>
        /// To be called from UI thread only
        /// </summary>
        private void CloseBusyDisplay() {
            if (busyForm != null && !busyForm.IsDisposed) {
                busyForm.Close();
                busyForm.Dispose();
            }

            busyForm = null;
        }
        void busyForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e) {
            if (Working) {
                Abort();
            }
        }




        #region IHashWorkManager / Task Queue
        // See IHashWorkManager declaration for additional code comments

        /// <summary>
        /// Used to lock for modifications to the task queue
        /// </summary>
        object TaskQueueLock = new object();

        /// <summary>
        /// Used for WaitAll method. Notes to self: This list should not be accessed except with
        /// a lock on TaskQueueLock. The state of objects referenced by the
        /// list should not be modified until they are removed from the list except
        /// within a lock on TaskQueueLock, and objects should not be removed from the list
        /// unless their InUse property returns false. Upon calling EndUse, if the item is no
        /// longer is use, it must be removed from the list and disposed.
        /// </summary>
        List<TaskResetEvent> TaskQueueItems = new List<TaskResetEvent>();

        /// <summary>
        /// Manages a task and its wait handle
        /// </summary>
        class TaskResetEvent
        {
            /// <summary>
            /// Creates a new TaskResetEvent
            /// </summary>
            /// <param name="tast">The tast to reference</param>
            /// <param name="mre">The wait handle.</param>
            public TaskResetEvent(SimpleDelegate tast, ManualResetEvent mre) {
                this.Task = tast;
                this.WaitHandle = mre;
            }
            /// <summary>Gets the associated task</summary>
            public SimpleDelegate Task { get; private set; }
            /// <summary>The associated wait handle</summary>
            public ManualResetEvent WaitHandle { get; private set; }
            /// <summary>Returns a true value to indicate that this object is being used an can not be disposed.</summary>
            public bool InUse { get { return useLevel != 0; } }

            int useLevel = 0;
            /// <summary>
            /// Marks this object as in-use. The object can not be removed from the task queue or disposed
            /// until a corresponding EndUse call is made for each BeginUse call.
            /// </summary>
            public void BeginUse() {
                useLevel++;
            }
            /// <summary>
            /// Marks this object as no longer in use by the caller of BeginUse.
            /// </summary>
            /// <returns>True if the object is no longer marked as in-use by any code.</returns>
            public bool EndUse() {
                useLevel--;

                return useLevel == 0;
            }
        }

        void IHashWorkManager.CheckIn(float progress, out bool abort) {

            lock (StatusLock) { // Necessary?
                this.Progress = progress;
                abort = AbortPending;
            }

            worker.ReportProgress(((int)(progress * 100)).Clamp(0, 100));

        }

        void IHashWorkManager.QueueTask(SimpleDelegate task) {
            // Enqueue item in list
            TaskResetEvent item = new TaskResetEvent(task, new ManualResetEvent(false));
            lock (TaskQueueLock) {
                item.BeginUse();
                TaskQueueItems.Add(item);
            } 
            
            // Enqueu item in thread pool
            ThreadPool.QueueUserWorkItem((object threadContext) => {
                // Task will be performed
                item.Task();

                // Wait handle will be reset
                lock (TaskQueueLock) {
                    item.WaitHandle.Set();
                    item.EndUse();

                    // If item is no longer in use, item will be removed from list and disposed
                    if (!item.InUse) {
                        TaskQueueItems.Remove(item);
                        item.WaitHandle.Close();
                    }
                }
            });
        }



        void IHashWorkManager.WaitAll() {
            List<TaskResetEvent> tasks = new List<TaskResetEvent>();
            List<ManualResetEvent> events = new List<ManualResetEvent>();
            
            // Take a snapshot of task queue and list of currently active wait handles
            lock (TaskQueueLock) {
                if (TaskQueueItems.Count == 0) return;

                for (int i = 0; i < TaskQueueItems.Count; i++) {
                    tasks.Add(TaskQueueItems[i]);
                    events.Add(TaskQueueItems[i].WaitHandle);

                    // Captured items can not be disposed
                    TaskQueueItems[i].BeginUse();
                }
            }

            // Wait on all tasks in our snapshot
            WaitHandle.WaitAll(events.ToArray());

            // Free all tasks in our snapshot, and free any items that nobody is still using
            lock (TaskQueueLock) {
                foreach (var task in tasks) {
                    task.EndUse();
                    if (!task.InUse) {
                        TaskQueueItems.Remove(task);
                        task.WaitHandle.Close();
                    }
                }
            }
        }


        void IHashWorkManager.AddHashes(byte[] buffer, int start, int len, HashFlags algso, HashFlags type) {
            ((IHashWorkManager)this).AddHashes(buffer, start, len, null, algso, type);
        }

        void IHashWorkManager.AddHashes(byte[] buffer, int start, int len, HASH.Platform.AsyncCancelCheck cancelCheck, HashFlags algos, HashFlags type, params HashFlags[] additionalTypes) {
            var allalgos = getAlgos(algos);
            if (allalgos.Count == 0) throw new ArgumentException("Hash algorithm not specified.");

            foreach (var algo in allalgos) {
                if (RomHash.IsHashRequired(Platform, algo | type)) {
                    byte[] hash = null;
                    if (cancelCheck == null || cancelCheck() == false) {
                        switch (algo) {
                            case HashFlags.MD5:
                                hash = CalculateMD5(buffer, start, len);
                                break;
                            case HashFlags.SHA1:
                                hash = CalculateSha1(buffer, start, len, cancelCheck);
                                break;
                            case HashFlags.SHA256:
                                hash = CalculateSha256(buffer, start, len, cancelCheck);
                                break;
                            case HashFlags.CRC32:
                                hash = CalculateCRC32(buffer, start, len);
                                break;
                        }
                    }

                    if (hash != null) {
                        lock (HashesLock) {
                            _Hashes.Add(new RomHash(hash, algo | type));

                            if (additionalTypes != null) {
                                for (int i = 0; i < additionalTypes.Length; i++) {
                                    _Hashes.Add(new RomHash(hash, algo | additionalTypes[i]));

                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts a bit-field into an array of single-bit values
        /// </summary>
        /// <param name="allAlgos"></param>
        /// <returns></returns>
        IList<HashFlags> getAlgos(HashFlags allAlgos) {
            List<HashFlags> result = new List<HashFlags>();

            if ((allAlgos & HashFlags.CRC32) != 0) result.Add(HashFlags.CRC32);
            if ((allAlgos & HashFlags.SHA1) != 0) result.Add(HashFlags.SHA1);
            if ((allAlgos & HashFlags.SHA256) != 0) result.Add(HashFlags.SHA256);
            if ((allAlgos & HashFlags.MD5) != 0) result.Add(HashFlags.MD5);

            return result;
        }


        private static byte[] CalculateCRC32(byte[] rom, int start, int len) {
            return CRC32.ComputeCrc32(rom, start, len);
        }
        private static byte[] CalculateMD5(byte[] rom, int start, int len) {
            return MD5.Create().ComputeHash(rom, start, len);
        }
        private static byte[] CalculateSha1(byte[] rom, int start, int len) {
            return SHA1.Create().ComputeHash(rom, start, len);
        }
        private static byte[] CalculateSha256(byte[] rom, int start, int len) {
            return SHA256.Create().ComputeHash(rom, start, len);
        }
        private static byte[] CalculateSha1(byte[] rom, int start, int len, HASH.Platform.AsyncCancelCheck cancelChecker) {
            //byte[] copy = new byte[rom.Length];


            var hash = SHA1.Create();
            int size = 0x1000;

            int offset = start;
            while (rom.Length - offset >= size) {
                if (cancelChecker != null && cancelChecker()) return null;
                offset += hash.TransformBlock(rom, offset, size, rom, offset);
            }
            hash.TransformFinalBlock(rom, offset, rom.Length - offset);

            return hash.Hash;
        }
        private static byte[] CalculateSha256(byte[] rom, int start, int len, HASH.Platform.AsyncCancelCheck cancelChecker) {
            //byte[] copy = new byte[rom.Length];


            var hash = SHA256.Create();
            int size = 0x1000;

            int offset = 0;
            while (rom.Length - offset >= size) {
                if (cancelChecker != null && cancelChecker()) return null;
                offset += hash.TransformBlock(rom, offset, size, rom, offset);
            }
            hash.TransformFinalBlock(rom, offset, rom.Length - offset);

            return hash.Hash;
        }

        #endregion
    }

    /// <summary>
    /// Presented to functions and objects async work is delegated to for managing the operation.
    /// </summary>
    interface IHashWorkManager
    {
        /// <summary>
        /// Acknowledges that an incremental step is complete.
        /// </summary>
        /// <param name="progress">A value between 0f and 1f indicating the percentage of work complete.</param>
        /// <param name="abort">Returns true if a request has been made to abort the operation. SEE REMARKS.</param>
        /// <remarks>If abort returns true, the worker should return immediately. If abort returns true on the final step
        /// it is acceptable but not required for the worker to wrap up and complete instead. If the caller and
        /// worker do not have a mechanism to communicate that work completed despite the abort request, it must
        /// be assumed that the operation was aborted.</remarks>
        void CheckIn(float progress, out bool abort);

        /// <summary>
        /// Queues the task to be performed by thread pool.
        /// </summary>
        /// <param name="task"></param>
        void QueueTask(SimpleDelegate task);

        /// <summary>
        /// Blocks until all currently pending tasks are completed.
        /// </summary>
        void WaitAll();

        /// <summary>
        /// Returns true if the user has cancelled the operation.
        /// </summary>
        bool AbortPending { get; }

        /// <summary>
        /// Calculates the specified hash and adds it to the hash collection, unless settings indicate the hash is not needed or the operation was cancelled, in which case it is ignored. This methods is executed synchronously. The method is thread safe. The ROM buffers should not be modified during the operation.
        /// </summary>
        /// <param name="buffer">Buffer containing data to hash</param>
        /// <param name="start">Starting index for hash</param>
        /// <param name="len">Length of data to hash</param>
        /// <param name="algos">1 or more hash algorithm flags</param>
        /// <param name="type">Hash type</param>
        void AddHashes(byte[] buffer, int start, int len, HashFlags algos, HashFlags type);
        /// <summary>
        /// Calculates the specified hash and adds it to the hash collection, unless settings indicate the hash is not needed or the operation was cancelled, in which case it is ignored. This methods is executed synchronously. The method is thread safe. The ROM buffers should not be modified during the operation.
        /// </summary>
        /// <param name="buffer">Buffer containing data to hash</param>
        /// <param name="start">Starting index for hash</param>
        /// <param name="len">Length of data to hash</param>
        /// <param name="algos">1 or more hash algorithm flags</param>
        /// <param name="type">Hash type</param>
        /// <param name="check">Can be null</param>
        /// <param name="additionalTypes">Additional hash types</param>
        void AddHashes(byte[] buffer, int start, int len, HASH.Platform.AsyncCancelCheck check, HashFlags algos, HashFlags type, params HashFlags[] additionalTypes);

        /// <summary>
        /// Gets a list of computed hashes. Value is undefined when the hash operation is incomplete or aborted.
        /// </summary>
        IList<RomHash> Hashes { get; }
    }

    /// <summary>I do not understand your parameters and return types. I am just a simple delegate.</summary>
    delegate void SimpleDelegate();
}
