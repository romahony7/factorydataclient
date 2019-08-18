using Common;
using Serilog;
using System;
using System.Messaging;

namespace FactoryDataPlcSvc
{
    internal class SendMessage : IDisposable
    {
        private object msgObject;
        private string label;
        private Type[] type;
        private MessageQueue writeMQ;

        // Empty Constructor
        public SendMessage()
        {

        }

        // Send method
        public void Send(object msgObject, string label, Type[] type)
        {
            this.msgObject = msgObject;
            this.label = label;
            this.type = type;

            try
            {
                writeMQ = new MessageQueue(PlcSvcMQ.writeMQ)
                {
                    Formatter = new XmlMessageFormatter(this.type)
                };

                Log.Information("WriteMQ Instance Opened");

                //Prepare the message for sending
                var objMessage = new Message()
                {
                    Formatter = new XmlMessageFormatter(this.type),
                    Label = this.label,
                    Body = this.msgObject,
                    Priority = MessagePriority.Highest,
                    Recoverable = true
                };

                writeMQ.Send(objMessage);
                Log.Information("Message Send: " + objMessage.Label + " MsgID: " + objMessage.Id);

            }

            catch (Exception ex)
            {
                Log.Error("Send Msg Exception: " + ex.Message);
            }

            finally
            {
                //Close instance of WriteMQ
                writeMQ.Close();
                Log.Information("WriteMQ Instance Closed");

            };
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SendMessage() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}