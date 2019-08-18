using System.ServiceProcess;

namespace FactoryDataSqlSvc
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
                new FactoryDataSqlSvc()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }

}
