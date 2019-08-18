using System;


namespace Common
{
    //**********************************************************************************************
    //* Ethernet/IP for ControlLogix for WinForms Application
    //*
    //* Manufacturing Automation, LLC
    //* 01-AUG-16
    //*
    //*
    //* Copyright 2016 Manufacturing Automation, LLC
    //*
    //* This class is used to make a UI thread synchronized driver instance for 
    //* WinForms applications
    //*
    //*
    //* Distributed under the MIT software licensing
    //*
    //* IMPORTANT NOTE
    //* Visual Studio 2015 has a bug that will not let this show in the ToolBox
    //*******************************************************************************************************


    [System.ComponentModel.DefaultEvent("DataReceived")]
    [System.ComponentModel.DesignTimeVisible(true)]
    [System.ComponentModel.ToolboxItem(true)]
    public class EthernetIPforCLXCom : ClxDriver.EthernetIPforCLX, System.ComponentModel.ISupportInitialize, System.ComponentModel.IComponent 
    {


        private static readonly object EventDisposed = new object();

        private System.Threading.SynchronizationContext m_synchronizationContext;
        #region "Constructor"
        public EthernetIPforCLXCom(System.ComponentModel.IContainer container) : this()
        {

            //Required for Windows.Forms Class Composition Designer support
            container.Add(this);
        }

        public EthernetIPforCLXCom() : base()
        {

          // m_synchronizationContext = new System.Windows.Threading.DispatcherSynchronizationContext(Application.Current.Dispatcher);
            m_synchronizationContext = System.Threading.SynchronizationContext.Current;
        }



        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);


            if (disposing)
            {
            }
        }
        #endregion

        #region "Properties"
        private System.ComponentModel.ISite m_site ;

        public System.ComponentModel.ISite Site
        {
            get
            {
                return m_site;
//                'throw new NotImplementedException();
            }

            set
            {
                m_site = value;
  //              throw new NotImplementedException();
            }
        }


        private string m_HostName;
        public override string IPAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(m_HostName))
                {
                    return m_HostName;
                }
                else
                {
                    return base.IPAddress;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (base.IPAddress != value)
                    {
                        System.Net.IPAddress address = new System.Net.IPAddress(0);
                        if (System.Net.IPAddress.TryParse(value, out address))
                        {
                            base.IPAddress = value;
                            m_HostName = "";
                        }
                        else
                        {
                            System.Net.IPHostEntry IP = default(System.Net.IPHostEntry);
                            try
                            {
                                IP = System.Net.Dns.GetHostEntry(value);
                                int i;
                                for (i = 0; i <= IP.AddressList.Length - 1; i++)
                                {
                                    if (IP.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                    {
                                        base.IPAddress = IP.AddressList[i].ToString();
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                            m_HostName = value;
                        }
                    }
                }
            }
        }



        #endregion


        #region "Events


        public event EventHandler Disposed;

     

    //private MyEvent clicked;

    //event MyEvent Itest.Clicked
    //{
    //    add
    //    {
    //        clicked += value;
    //    }
    //    remove
    //    {
    //        clicked -= value;
    //    }
    //}


    protected override void OnConnectionClosed(EventArgs e)
        {
            // m_SynchronizingObject IsNot Nothing Then
            if (m_synchronizationContext != null)
            {
                try
                {
                    m_synchronizationContext.Post(ConnectionClosedSync, EventArgs.Empty);
                }
                catch (Exception )
                {
                }
            }
            else
            {
                base.OnConnectionClosed(e);
            }
        }

        private void ConnectionClosedSync(object e)
        {
            EventArgs e1 = (EventArgs)e;
            base.OnConnectionClosed(e1);
        }
        //********************************************************************************************************************************


        protected override void OnConnectionEstablished(EventArgs e)
        {
            if (m_synchronizationContext != null)
            {
                m_synchronizationContext.Post(ConnectionEstabishedSync, EventArgs.Empty);
            }
            else
            {
                base.OnConnectionEstablished(e);
            }
        }


        private void ConnectionEstabishedSync(object e)
        {
            EventArgs e1 = (EventArgs)e;
            base.OnConnectionEstablished(e1);
        }


        protected override void OnDataReceived(ClxDriver.Common.PlcComEventArgs e)
        {
            if (m_synchronizationContext != null)
            {
                m_synchronizationContext.Post(DataReceivedSync, e);
            }
            else
            {
                base.OnDataReceived(e);
            }
        }


        private void DataReceivedSync(object e)
        {
            try
            {
                ClxDriver.Common.PlcComEventArgs e1 = (ClxDriver.Common.PlcComEventArgs)e;
                base.OnDataReceived(e1);
            }
            catch (Exception )
            {
                //Dim dbg = 0
            }
        }


        //***********************************************************************************************************
        protected override void OnComError(ClxDriver.Common.PlcComEventArgs e)
        {
            if (m_synchronizationContext != null)
            {
                m_synchronizationContext.Post(ErrorReceivedSync, e);
            }
            else
            {
                base.OnComError(e);
            }
        }

        private void ErrorReceivedSync(object e)
        {
            ClxDriver.Common.PlcComEventArgs e1 = (ClxDriver.Common.PlcComEventArgs)e;
            base.OnComError(e1);
        }
        //***********************************************************************************************************


        //***********************************************************************************************************
        protected override void OnSubscriptionDataReceived(ClxDriver.Common.SubscriptionEventArgs e)
        {
            if (m_synchronizationContext != null)
            {
                m_synchronizationContext.Post(DataRecSync, e);
            }
            else
            {
                base.OnSubscriptionDataReceived(e);
            }
        }

        private void DataRecSync(object e)
        {
            ClxDriver.Common.SubscriptionEventArgs e1 = (ClxDriver.Common.SubscriptionEventArgs)e;
            e1.dlgCallBack(this, e1);
        }

        #endregion

        #region "Public Methods"
        public void Write(string startAddress, byte[] dataToWrite)
        {
            //* Convert the byte values into strings
            string[] s = new string[dataToWrite.Length];
            for (int i = 0; i <= dataToWrite.Length - 1; i++)
            {
                s[i] = System.Text.Encoding.UTF8.GetString(dataToWrite);
            }

            //* Write to the PLC
            Write(startAddress, s.Length, s);
        }

        #endregion

        #region "IniFileHandling"
        private string m_IniFileName = "";
        public string IniFileName
        {
            get { return m_IniFileName; }
            set { m_IniFileName = value; }
        }

        private string m_IniFileSection;
        public string IniFileSection
        {
            get { return m_IniFileSection; }
            set { m_IniFileSection = value; }
        }


        private bool Initializing;
        public void BeginInit()
        {
            Initializing = true;
        }

        public void EndInit()
        {
            // If Not Me.DesignMode Then
            if (!string.IsNullOrEmpty(m_IniFileName))
            {
                try
                {
                    SetPropertiesByIniFile(this, m_IniFileName, m_IniFileSection);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("INI File - " + ex.Message);
                }
            }
            //End If
            Initializing = false;
        }

        public static void SetPropertiesByIniFile(object targetObject, string iniFileName, string iniFileSection)
        {
            if (!string.IsNullOrEmpty(iniFileName))
            {
                ClxDriver.Common.IniParser p = new ClxDriver.Common.IniParser(iniFileName);
                string[] settings = p.EnumSection(iniFileSection);
                int index;
                for (index = 0; index <= settings.Length - 1; index++)
                {
                    //* Write the value to the property that came from the end of the PLCAddress... property name
                    System.Reflection.PropertyInfo pi = default(System.Reflection.PropertyInfo);
                    pi = targetObject.GetType().GetProperty(settings[index], System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (pi != null)
                    {
                        object value = null;
                        value = Convert.ChangeType(p.GetSetting(iniFileSection, settings[index]), targetObject.GetType().GetProperty(pi.Name).PropertyType);
                        pi.SetValue(targetObject, value, null);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Ini File Error - " + settings[index] + " is not a valid property.");
                    }
                }
                dynamic x = p.GetSetting(iniFileSection, settings[0]);
            }
        }
        #endregion

    }
}

