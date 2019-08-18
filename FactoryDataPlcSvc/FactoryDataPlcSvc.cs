using Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;


namespace FactoryDataPlcSvc
{
    public partial class FactoryDataPlcSvc : ServiceBase
    {
        Timer eventTimer ;
        Timer subscriptionTimer ;
        Timer transactionTimer ;
        MessageQueue readMQ ;
        EthernetIPforCLXCom logixEventDrv;
        EthernetIPforCLXCom logixSubscriptionDrv ;
        EthernetIPforCLXCom logixTransactionDrv ;
        List<ListTag> eventList ;
        List<ListTag> subscriptionList ;
        List<ListTag> transactionList ;
        

        public FactoryDataPlcSvc()
        {
            InitializeComponent();
        }

        // Start up actions
        protected override void OnStart(string[] args)
        { 
            //Logger Instance
            Log.Logger = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger();

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(ServiceHandle, ref serviceStatus);
            Log.Information("FactoryData Plc Service Start Pending");

            // Code to do start-up work here
            CreateMQ();
            
            // Get plc configuration from database
           var msgObject = new PlcConfigReqMsg
           {
                getPlcConfig = true
           };

            var type = new Type[] { typeof(PlcConfigReqMsg) };

            using (var msg = new SendMessage())
            {
                msg.Send(msgObject, "Plc_Config_Request", type);
            }

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
            Log.Information("FactoryData Plc Service Running");
        }

        // MSMQ 
        public void CreateMQ()
        {
            //Create MQ
            if (!MessageQueue.Exists(PlcSvcMQ.readMQ))
            {
                MessageQueue.Create(PlcSvcMQ.readMQ);
                Log.Information("ReadMQ Created");
            }

            // Format MQ
            if (MessageQueue.Exists(PlcSvcMQ.readMQ))
            {
                try
                {
                    readMQ = new MessageQueue(PlcSvcMQ.readMQ)
                    {
                        Formatter = new XmlMessageFormatter()
                    };
                    Log.Information("ReadMQ Formatted");
                }
                catch (Exception ex)
                {
                    Log.Error("ReadMQ Format Exception: " + ex.Message);
                }

                // Add an event handler for the ReceiveCompleted event
                readMQ.ReceiveCompleted += new ReceiveCompletedEventHandler(ReadMQ_ReceiveCompleted);

                //Calling BeginReceive causes application to wait for a message to appear on the queue
                // that wait Is now processed on a separate thread. 
                try
                {
                    readMQ.BeginReceive();
                   
                }
                catch (Exception ex)
                {
                    Log.Error("ReadMQ Begin Recieve Exception: " + ex.Message);
                }

                Log.Information("ReadMQ Listening For Messages");
            }


        }

        // Recieve message event
        private void ReadMQ_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            Log.Debug("Read Message Queue Event Triggered");
            //Read recieved message
            var msgRead = new Message();
            msgRead = readMQ.EndReceive(e.AsyncResult);
            Log.Debug(msgRead.Label + " Recieved MsgID: " + msgRead.Id);

            if (msgRead.Label == "Plc_Config")
            {
                msgRead.Formatter = new XmlMessageFormatter(new Type[] { typeof(PlcConfigMsg) });
                var msgData = ((PlcConfigMsg)msgRead.Body);

                // Instantiate Logix drivers
                logixEventDrv = new EthernetIPforCLXCom()
                {
                    IPAddress = msgData.IPAddress,
                    DisableSubscriptions = msgData.DisableSubscriptions,
                    PollRateOverride = msgData.PollRateOverride,
                    ProcessorSlot = msgData.ProcessorSlot,
                    Port = msgData.Port
                };
                Log.Information("Logix Event driver created");

                logixSubscriptionDrv = new EthernetIPforCLXCom()
                {
                    IPAddress = msgData.IPAddress,
                    DisableSubscriptions = msgData.DisableSubscriptions,
                    PollRateOverride = msgData.PollRateOverride,
                    ProcessorSlot = msgData.ProcessorSlot,
                    Port = msgData.Port
                };
                Log.Information("Logix Subscription driver created");

                logixTransactionDrv = new EthernetIPforCLXCom()
                {
                    IPAddress = msgData.IPAddress,
                    DisableSubscriptions = msgData.DisableSubscriptions,
                    PollRateOverride = msgData.PollRateOverride,
                    ProcessorSlot = msgData.ProcessorSlot,
                    Port = msgData.Port
                };
                Log.Information("Logix Transaction driver created");

                // Set up read timers
                eventTimer = new Timer
                {
                    Interval = msgData.EventPollRate
                };
                eventTimer.Elapsed += new ElapsedEventHandler(OnEventTimer);

                subscriptionTimer = new Timer
                {
                    Interval = msgData.SubscriptionPollRate
                };
                subscriptionTimer.Elapsed += new ElapsedEventHandler(OnSubscriptionTimer);

                transactionTimer = new Timer
                {
                    Interval = msgData.TransactionPollRate
                };
                transactionTimer.Elapsed += new ElapsedEventHandler(OnTransactionTimer);

                // Get tag list from database
                var msgObject = new TagListReqMsg
                {
                    getTagList = true
                };

                var type = new Type[] { typeof(TagListReqMsg) };

                using (var msg = new SendMessage())
                {
                    msg.Send(msgObject, "TagList_Request", type);
                }

                Log.Debug("Plc Config Message Processed" + " MsgID: " + msgRead.Id);
                
            }

            if (msgRead.Label == "TagList_Config")
            {
                msgRead.Formatter = new XmlMessageFormatter(new Type[] { typeof(TagListMsg) });
                var msgData = ((TagListMsg)msgRead.Body);

                eventList = new List<ListTag>();
                transactionList = new List<ListTag>();
                subscriptionList = new List<ListTag>();


                foreach (ListTag elem in msgData.List)
                {
                    if (elem.TagTypeId == TagTypeConstants.eventType)
                    {
                        var tag = elem;
                        eventList.Add(tag);
                        Log.Debug("Tag added to event tag list: " + tag.Name);
                    }

                    if (elem.TagTypeId == TagTypeConstants.subscriptionType)
                    {
                        var tag = elem;
                        subscriptionList.Add(tag);
                        Log.Debug("Tag added to subscription tag list: " + tag.Name);
                    }

                    if (elem.TagTypeId == TagTypeConstants.transactionType)
                    {
                        var tag = elem;
                        transactionList.Add(tag);
                        Log.Debug("Tag added to transaction tag list: " + tag.Name);
                    }

                };

                // Start read timers
                eventTimer.Start();
                transactionTimer.Start();
                subscriptionTimer.Start();
                Log.Debug("Read timers started ");
            }

            // Resume listening for messages
            try
            {
                readMQ.BeginReceive();
                Log.Information("ReadMQ Listening For Messages");
            }
            catch (Exception ex)
            {
                Log.Error("ReadMQ Begin Recieve Exception: " + ex.Message);
            }
        }

        // Event tag timer event
        public void OnEventTimer(object sender, ElapsedEventArgs args)
        {
            eventTimer.Stop();
            Log.Debug("FactoryData Plc Service Event Timer Event");
            
            var recordList = new List<RecordTag>();
            var resultsAvail = false;

            try
            {

                foreach (ListTag elem in eventList)
                {
                    string trigger = string.Concat(elem.Name,".Trigger");
                    string ack = string.Concat(elem.Name,".Ack");
                    string data = string.Concat(elem.Name,".Data");
                    string timestamp = string.Concat(elem.Name,".TS");

                    var triggerCheck = bool.Parse(logixEventDrv.Read(trigger));

                    if (triggerCheck == true)
                    {
                        resultsAvail = true;

                        var record = new RecordTag
                        {
                            TagId = elem.Id,
                            Data = long.Parse(logixEventDrv.Read(data)),
                            PlcTS = DateTime.Parse(logixEventDrv.Read(timestamp)),
                        };

                        logixEventDrv.Write(ack,1);

                        Log.Debug("Subscription Tag Read:" + elem.Name + " Value:" + record.Data + " Timestamp: " + record.PlcTS);
                        recordList.Add(record);

                        logixEventDrv.Write(ack, 0);
                    }

                    
                };
            }
            catch (Exception ex)
            {
                Log.Error("Read Event Tag List exception: " + ex.Message);
            }

            if (resultsAvail == true)
            {
                var msgObject = new TagReadResultsMsg
                {
                    List = recordList
                };

                var type = new Type[] { typeof(TagReadResultsMsg) };

                using (var msg = new SendMessage())
                {
                    msg.Send(msgObject, "EventTag_Results", type);
                }

                resultsAvail = false;
            }

            eventTimer.Start();
        }

        // Transaction udt timer event
        private void OnTransactionTimer(object sender, ElapsedEventArgs e)
        {
            transactionTimer.Stop();

            Log.Debug("FactoryData Plc Service Transaction Timer Event");
            var resultList = new List<TransactionRecord>();
            var resultsAvail = false;

            foreach (ListTag elem in transactionList)
            {
                Type udtType = typeof(TransactionUDT);
                var udt = new TransactionUDT();
                string trigger = string.Concat(elem.Name, ".Trigger");
                string ack = string.Concat(elem.Name, ".Ack");
               
                

                // Trigger bit check
                var triggerCheck = bool.Parse(logixTransactionDrv.Read(trigger));

                if (triggerCheck == true)
                {
                    resultsAvail = true;

                    try
                    {
                        udt = (TransactionUDT)logixTransactionDrv.ReadUDT(elem.Name, udtType);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Read transaction udt Exception: " +ex.Message);
                    }

                    // Transaction record
                    var result = new TransactionRecord
                    {
                        TagName = elem.Name,
                        Udt = udt
                    };

                    // Populate result list
                    resultList.Add(result);

                    // Set acknowledge bit
                    logixTransactionDrv.Write(ack, 1);
                    logixTransactionDrv.Write(ack, 0);

                }

            }

            if (resultsAvail == true)
            {
                var msgObject = new TransactionResultsMsg
                {
                    Records = resultList
                };

                var type = new Type[] { typeof(TransactionResultsMsg) };

                using (var msg = new SendMessage())
                {
                    msg.Send(msgObject, "TransactionTag_Results", type);
                }

                resultsAvail = false;
            }

            transactionTimer.Start();
        }

        // Subscription tag timer event
        private void OnSubscriptionTimer(object sender, ElapsedEventArgs e)
        {
            subscriptionTimer.Stop();
            Log.Debug("FactoryData Plc Service Subscription Timer Event");

            var recordList = new List<RecordTag>();

            try
            {

                foreach (ListTag elem in subscriptionList)
                {
                   
                    var record = new RecordTag
                    {
                        TagId = elem.Id,
                        Data = long.Parse(logixSubscriptionDrv.Read(elem.Name)),
                        PlcTS = DateTime.Parse(logixSubscriptionDrv.Read("TimestampPLC")),
                    };

                    Log.Debug("Subscription Tag Read:" + elem.Name + " Value:" + record.Data + " Timestamp: " + record.PlcTS);
                    recordList.Add(record);
                };
            }
            catch (Exception ex)
            {
                Log.Error("Read Subscription Tag List exception: " + ex.Message);
            }

            var msgObject = new TagReadResultsMsg
            {
                List = recordList
            };

            var type = new Type[] { typeof(TagReadResultsMsg) };

            using (var msg = new SendMessage())
            {
                msg.Send(msgObject, "SubscriptionTag_Results", type);
            }

            subscriptionTimer.Start();
        }

        // Stop actions
        protected override void OnStop()
        {
            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_STOP_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(ServiceHandle, ref serviceStatus);
            Log.Information("FactoryData Plc Service Stop Pending");

            // Code to do shutdown work here
            logixEventDrv.Dispose();
            logixSubscriptionDrv.Dispose();
            logixTransactionDrv.Dispose();
            eventTimer.Dispose();
            subscriptionTimer.Dispose();
            transactionTimer.Dispose();
            Dispose();
            Log.Information("FactoryData Plc Service resources disposed");

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
            Log.Information("FactoryData Plc Service Stopped");
        }

        // Status pending implementation code
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

        // Status pending implementation code
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

        // Status pending implementation code
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}

