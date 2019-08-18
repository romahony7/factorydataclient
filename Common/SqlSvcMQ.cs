namespace Common
{
    public class SqlSvcMQ
    {
        //MQ to read from
        public const string readMQ = @".\FactoryData-SqlMQ";
        //MQ to write to
        public const string writeMQ = @"FD-WebHost\FactoryData-LogixMQ";
    }
}
