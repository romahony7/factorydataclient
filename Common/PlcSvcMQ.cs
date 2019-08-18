namespace Common
{
    public class PlcSvcMQ
    {
        //MQ to write into
        public const string writeMQ = @"FD-JB-ROM\FactoryData-SqlMQ";
        //MQ to read from
        public const string readMQ = @".\FactoryData-LogixMQ";
    }
}
