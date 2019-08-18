using Common;
using DataAccess;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace FactoryDataSqlSvc
{
    public partial class FactoryDataSqlSvc : ServiceBase
    {
        MessageQueue readMQ;
        TagListMsg msgTagList;

        public FactoryDataSqlSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] arg)
        {
            //Logger Instance
            Log.Logger = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger();

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
            Log.Information("FactoryData Sql Service Start Pending");

            // Code to do work here
            CreateMQ();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            Log.Information("FactoryData Sql Service Running");
        }

        public void CreateMQ()
        {
            //Create MQ's if necessary
            if (!MessageQueue.Exists(SqlSvcMQ.readMQ))
            {
                MessageQueue.Create(SqlSvcMQ.readMQ);
                    
                Log.Information("ReadMQ Created");
            }

            // Format MQ
            if (MessageQueue.Exists(SqlSvcMQ.readMQ))
            {
                try
                {
                    readMQ = new MessageQueue(SqlSvcMQ.readMQ)
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
                    Log.Information("ReadMQ Listening For Messages");

                }
                catch (Exception ex)
                {
                    Log.Error("ReadMQ Begin Recieve Exception: " + ex.Message);
                }

            }


        }

        private void ReadMQ_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            Log.Information("Read Message Queue Event Triggered");
            //Read recieved message
            var msgRead = new Message();
            msgRead = readMQ.EndReceive(e.AsyncResult);
            Log.Information(msgRead.Label + " Recieved MsgID: " + msgRead.Id);

            if (msgRead.Label == "Plc_Config_Request")
            {
                msgRead.Formatter = new XmlMessageFormatter(new Type[] { typeof(PlcConfigReqMsg) });
                var msgData = ((PlcConfigReqMsg)msgRead.Body);

                if (msgData.getPlcConfig == true)
                {
                    using (var _context = new FactoryDataDbContext())
                    {
                        try
                        {
                            var plcConfig = _context.Plcs.SingleOrDefault(p => p.Id == 1);

                            if (plcConfig == null)
                            {
                                Log.Error("Plc config query executed with null result");
                            }
                            else
                            {
                                Log.Debug("Plc config query executed successfully");
                            }

                            var config = new PlcConfigMsg()
                            {
                                Name = plcConfig.Name,
                                IPAddress = plcConfig.IPAddress,
                                DisableSubscriptions = plcConfig.DisableSubscriptions,
                                PollRateOverride = plcConfig.PollRateOverride,
                                ProcessorSlot = plcConfig.ProcessorSlot,
                                Port = plcConfig.Port,
                                EventPollRate = plcConfig.EventPollRate,
                                SubscriptionPollRate = plcConfig.SubscriptionPollRate,
                                TransactionPollRate = plcConfig.TransactionPollRate
                            };

                            var type = new Type[] { typeof(PlcConfigMsg) };

                            using (var msg = new SendMessage())
                            {
                                msg.Send(config, "Plc_Config", type);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Plc config query exception" + ex.Message );
                        }
                        finally
                        {
                            _context.Dispose();
                        }

                    }

                    Log.Information("Plc Config Request Message Processed" + " MsgID: " + msgRead.Id);
                }
            }

            if (msgRead.Label == "TagList_Request")
            {
                msgRead.Formatter = new XmlMessageFormatter(new Type[] { typeof(TagListReqMsg) });
                var msgData = ((TagListReqMsg)msgRead.Body);

                if (msgData.getTagList == true)
                {
                    using (var _context = new FactoryDataDbContext())
                    {
                        try
                        {
                            var tagList = _context.Tags.ToList().Where(t=> t.IsActive == true);

                            if (tagList == null)
                            {
                                Log.Error("Tag list query executed with null result");
                            }
                            else
                            {
                                Log.Debug("Tag list query executed successfully");
                            }

                            msgTagList = new TagListMsg()
                            {
                                List = new List<ListTag>()
                            };

                            foreach (Tag elem in tagList)
                            {
                                var tag = new ListTag
                                {
                                    Id = elem.Id,
                                    Name = elem.Name,
                                    TagTypeId = elem.TagTypeId,
                                    PlcId = elem.PlcId,
                                    IsActive = elem.IsActive
                                };

                                msgTagList.List.Add(tag);
                                Log.Debug("Tag added to message list: " + tag.Name);
                            };
                            
                            var type = new Type[] { typeof(TagListMsg) };

                            using (var msg = new SendMessage())
                            {
                                Log.Debug("Tag list message passed to send");
                                msg.Send(msgTagList, "TagList_Config", type);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Tag List query exception: " + ex.Message);
                        }
                        finally
                        {
                            _context.Dispose();
                        }

                    }

                    Log.Information("Tag List Request Message Processed" + " MsgID: " + msgRead.Id);
                    
                }
            }

            if (msgRead.Label == "SubscriptionTag_Results")
            {
                msgRead.Formatter = new XmlMessageFormatter(new Type[] { typeof(TagReadResultsMsg) });
                var msgData = ((TagReadResultsMsg)msgRead.Body);

                using (var _context = new FactoryDataDbContext())
                {
                    try
                    {

                        foreach (RecordTag elem in msgData.List)
                        {
                            var subscriptionRecord = new SubscriptionTagRecord
                            {
                                
                                TagId = elem.TagId,
                                Data = elem.Data,
                                PlcTS = elem.PlcTS,
                                RecordTS = DateTime.Now
                               
                            };

                            _context.SubscriptionTagRecords.Add(subscriptionRecord);
                            _context.SaveChanges();
                            Log.Debug("Subscription Tag Record Added to database");
                        };
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Subscription Tag Record add to database exception: " 
                            + ex.Message + "Inner Exception: " 
                            + ex.InnerException.ToString());
                    }
                }

                Log.Information("Subscription Tag Results Message Processed" + " MsgID: " + msgRead.Id);
            }

            if (msgRead.Label == "EventTag_Results")
            {
                msgRead.Formatter = new XmlMessageFormatter(new Type[] { typeof(TagReadResultsMsg) });
                var msgData = ((TagReadResultsMsg)msgRead.Body);

                using (var _context = new FactoryDataDbContext())
                {
                    try
                    {

                        foreach (RecordTag elem in msgData.List)
                        {
                            var eventRecord = new EventTagRecord
                            {

                                TagId = elem.TagId,
                                Data = elem.Data,
                                PlcTS = elem.PlcTS,
                                RecordTS = DateTime.Now

                            };

                            _context.EventTagRecords.Add(eventRecord);
                            _context.SaveChanges();
                            Log.Debug("Event Tag Record Added to database");
                        };
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Event Tag Record add to database exception: " + ex.Message + "Inner Exception: " + ex.InnerException.ToString());
                    }
                }

                Log.Information("Event Tag Results Message Processed" + " MsgID: " + msgRead.Id);
            }

            if (msgRead.Label == "TransactionTag_Results")
            {
                msgRead.Formatter = new XmlMessageFormatter(new Type[] { typeof(TransactionResultsMsg) });
                var msgData = ((TransactionResultsMsg)msgRead.Body);

                foreach (TransactionRecord record in msgData.Records)
                {
                    if (record.Udt.TransactionType == TransactionTypeConstants.Insert)
                    {
                        try
                        {
                            var rowCount = DBUtils.SqlInsert(record.Udt, record.TagName);
                            Log.Information("Transaction udt Sql Insert complete");
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Transaction udt Sql Insert Exception: " + ex.Message);
                        }

                    }

                    if (record.Udt.TransactionType == TransactionTypeConstants.Update)
                    {
                        try
                        {
                            var rowCount = DBUtils.SqlUpdate(record.Udt, record.TagName);
                            Log.Information("Transaction udt Sql Update complete");
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Transaction udt Sql Update Exception: " + ex.Message);
                        }

                    }

                }

            }

            // Resume listening for messages
            try
            {
                readMQ.BeginReceive();
                Log.Information("ReadMQ Resume Listening For Messages");
            }
            catch (Exception ex)
            {
                Log.Error("ReadMQ Resume Listening For Messages Exception: " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            Log.Information("FactoryData Plc Service Stop Pending");

            // Code to do work here
            Dispose();

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
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
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
    }
}

