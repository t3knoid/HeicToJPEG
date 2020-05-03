using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HeicToJPEG_service
{
    public class ServiceControl
    {
        /// <summary>
        /// Starts the Worker Service
        /// </summary>
        public static void Start()
        {
            try
            {
                Worker worker = new Worker();
                ServiceController sc = new ServiceController();
                sc.ServiceName = worker.ServiceName;
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Stops the Worker Service
        /// </summary>
        public static void Stop()
        {
            try
            {
                Worker Worker = new Worker();
                ServiceController sc = new ServiceController(Worker.ServiceName);
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static bool Installed
        {
            get
            {
                bool serviceExists = false;
                Worker Worker = new Worker();
                foreach (ServiceController sc in ServiceController.GetServices())
                {
                    if (sc.ServiceName == Worker.ServiceName)
                    {
                        serviceExists = true;
                        break;
                    }
                }
                return serviceExists;
            }
        }

        public static bool Running
        {
            get
            {
                if (Installed)
                {
                    Worker Worker = new Worker();
                    ServiceController sc = new ServiceController();
                    sc.ServiceName = Worker.ServiceName;
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        return true;
                    }
                    else { return false; }
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool Stopped
        {
            get
            {
                if (Installed)
                {
                    Worker Worker = new Worker();
                    ServiceController sc = new ServiceController();
                    sc.ServiceName = Worker.ServiceName;
                    if (sc.Status == ServiceControllerStatus.Stopped)
                    {
                        return true;
                    }
                    else { return false; }
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
