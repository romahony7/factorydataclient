using System.ServiceProcess;

namespace FactoryDataPlcSvc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new FactoryDataPlcSvc()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
