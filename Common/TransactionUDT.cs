namespace Common
{
    public class TransactionUDT
    {
        public int TransactionType { get; set; }

        public string DBSource { get; set; }

        public string DBName { get; set; }

        public string DBSid { get; set; }

        public string DBPwd { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public int Data { get; set; }

        public bool Trigger { get; set; }

        public bool Ack { get; set; }

        public string TS { get; set; }

    }
}
