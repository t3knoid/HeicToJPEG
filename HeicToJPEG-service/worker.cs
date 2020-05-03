using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HeicToJPEG_service
{
    public partial class Worker : ServiceBase
    {
        System.Timers.Timer timerDoWork = new System.Timers.Timer();
        bool Interactive = false;

        // Service pending statuses
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
        public Worker()
        {
            InitializeComponent();
        }

        internal void Run(string[] args)
        {
            var parser = new CommandLine();
            parser.Parse(args);

            if (parser.Arguments.Count > 0)
            {
                // Install service
                if (parser.Arguments.ContainsKey("install"))
                {
                    try
                    {
                        if (!ServiceControl.Installed)
                        {
                            Console.WriteLine("Installing worker service.");
                            InstallerOutput output = Installer.Install();
                            if (output.Status == false)
                            {
                                Console.WriteLine(string.Format("Failed to install worker service. {0}", output.Message));
                            }
                            else
                            {
                                Console.WriteLine("Worker service installed.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Worker service already installed.");
                        }
                        Environment.Exit(0);
                    }
                    catch (Exception)
                    {
                        usage();
                        Environment.Exit(1);
                    }
                }

                // Uninstall service
                if (parser.Arguments.ContainsKey("uninstall"))
                {                   
                    try
                    {
                        if (ServiceControl.Installed)
                        {
                            Console.WriteLine("Uninstalling worker service.");
                            InstallerOutput output = Installer.Uninstall();
                            if (output.Status == false)
                            {
                                Console.WriteLine(string.Format("Failed to uninstall worker service. {0}", output.Message));
                            }
                            else
                            {
                                Console.WriteLine("Worker service uninstalled.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Worker service not installed.");
                        }
                        Environment.Exit(0);
                    }
                    catch (Exception)
                    {
                        usage();
                        Environment.Exit(1);
                    }                 
                }

                // Start service
                if (parser.Arguments.ContainsKey("start"))
                {
                    try
                    {
                        if (ServiceControl.Installed)
                        {
                            Console.WriteLine("Starting worker service.");
                            try
                            {
                                ServiceControl.Start();
                                Console.WriteLine("Worker service started.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(string.Format("Failed to start worker service. {0}", ex.Message));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Worker service not installed.");
                        }
                        Environment.Exit(0);
                    }
                    catch (Exception)
                    {
                        usage();
                        Environment.Exit(1);
                    }
                }

                // Stop service
                if (parser.Arguments.ContainsKey("stop"))
                {
                    try
                    {
                        if (ServiceControl.Installed)
                        {
                            Console.WriteLine("Stopping worker service.");
                            try
                            {
                                ServiceControl.Stop();
                                Console.WriteLine("Worker service stopped.");
                            }
                            catch (Exception ex)
                            { 
                                Console.WriteLine(string.Format("Failed to stop worker service. {0}", ex.Message));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Worker service not installed.");
                        }
                        Environment.Exit(0);
                    }
                    catch (Exception)
                    {
                        usage();
                        Environment.Exit(1);
                    }
                }

                usage();
                Environment.Exit(1); // If it gets to here. User passed unknown parameter
            }
            else
            {
                Console.WriteLine("Running interactive mode");
                Interactive = true;
                this.OnStart(args);
                Console.WriteLine("Running interactive mode. Press any key to stop service.");
                Console.ReadKey();
                OnStop();
            }

        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Timer properties
            timerDoWork.Interval = 5000; // Trigger timer event every 5 seconds
            timerDoWork.Elapsed += new ElapsedEventHandler(timerDoWork_Event); // Event handler to trigger when timer elapses

            // Start service here



            // Start timer after service has been initialized
            Console.WriteLine("Starting timer.");
            timerDoWork.Enabled = true; 

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            Console.WriteLine("Stopping Worker service.");

            ServiceStatus serviceStatus = new ServiceStatus();
            if (!Interactive)
            {
                // Update the service state to Stop Pending.
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
                serviceStatus.dwWaitHint = 100000; // Give it 10 seconds to stop
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }


            // Stop Worker thread if running
            Console.WriteLine("Stopping worker worker thread");
            if (backgroundWorker.IsBusy == true)
                backgroundWorker.CancelAsync();

            // Stop the worker timer
            timerDoWork.Stop();
            timerDoWork.Dispose();

            if (!Interactive)
            {
                // Update the service state to Stopped.
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }
            Console.WriteLine("Worker service stopped.");
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            if (bw.CancellationPending == true) return;

            Console.WriteLine("Stopping timer event.");
            timerDoWork.Stop();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            string message;

            if (e.Error != null)
            {
                message = "Worker Error: " + e.Error.Message + " Worker exiting.";
            }
            else if (e.Cancelled == true)
            {
                message = "Worker stopped. Worker exiting.";
            }
            else
            {
                message = "Worker exited. Worker exiting.";
            }

            Console.WriteLine(message);

            // Start timer again
            Console.WriteLine("Starting timer.");
            timerDoWork.Enabled = true;
        }

        private void timerDoWork_Event(object sender, ElapsedEventArgs e)
        {
            // Wake up worker to perform work
            Console.WriteLine(String.Format("The Elapsed event was raised on {0}", e.SignalTime));
            Console.WriteLine("Running Worker task");

            if (backgroundWorker.IsBusy != true) // Make sure worker is not busy
            {
                // Start the asynchronous operation.
                backgroundWorker.RunWorkerAsync();
            }

        }

        static void usage()
        {
            Console.WriteLine("usage: HeicToJPEG-service [-install] | [-uninstall] | [start] | [stop] ");
            Console.WriteLine("\n\t -install \t Install service");
            Console.WriteLine("\t -uinstall \t Uninstall service");
            Console.WriteLine("\t -start \t Start service");
            Console.WriteLine("\t -stop  \t Stop service");
        }
    }
}
