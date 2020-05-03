using System;
using System.ServiceProcess;

namespace HeicToJPEG_service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                // In interactive mode, the service is executed similar to a console application
                Worker worker = new Worker();
                worker.Run(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Worker()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
