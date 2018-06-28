using Fingrid.Messaging.Processor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fingrid.Messaging.Core;
using Fingrid.Messaging.Core.Enums;
using smscc;
using Fingrid.Messaging.Data;

namespace Fingrid.Messaging.Processor.Implementation
{
    public class SmppSmsSettings
    {
        public SmppSmsSettings(int timerInt, string ipAddress, int port, string systemID, string password)
        {
            this.TimerInterval = timerInt;
            this.IpAddress = ipAddress;
            this.Port = port;
            this.SystemID = systemID;
            this.Password = password;
        }

        public int TimerInterval { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string SystemID { get; set; }
        public string Password { get; set; }
    }

    public class DiamondIpIntegratedSmppSmsService : BaseSmppSmsService
    {

        public DiamondIpIntegratedSmppSmsService(SmppSmsSettings smppSmsSettings): base(smppSmsSettings)
        {
        }

        public override SmsServiceType SmsServiceType { get { return SmsServiceType.SmppIpIntegrated; } }
    }

    public abstract class BaseSmppSmsService : ISmsService, IDisposable
    {
        public abstract SmsServiceType SmsServiceType { get; }
        public Func<IMessageDeliveredEventArgs, Task<bool>> OnMessageDelivered{ get; set; }

        private readonly smscc.SMPP.SMSCclientSMPP smppClientConnection;

        private readonly SmppSmsSettings smppSmsSettings;
        //private readonly ISmsDao smsDao;
        private System.Timers.Timer TimerReconnect;
        private bool bServiceReady = false;

        public BaseSmppSmsService(SmppSmsSettings smppSmsSettings)//, ISmsDao smsDao)
        {
            this.smppSmsSettings = smppSmsSettings;
            //this.smsDao = smsDao;

            this.TimerReconnect = new System.Timers.Timer();

            // Create 1st instance of the SMPP component
            smppClientConnection = new smscc.SMPP.SMSCclientSMPP();
            ((System.ComponentModel.ISupportInitialize)(this.TimerReconnect)).BeginInit();
        
            // 
            // TimerReconnect
            // 
            this.TimerReconnect.AutoReset = false;
            this.TimerReconnect.Interval = this.smppSmsSettings.TimerInterval; //10000
            this.TimerReconnect.Elapsed += new System.Timers.ElapsedEventHandler(this.TimerReconnect_Elapsed);
            // 
            // smsccService
            //
            ((System.ComponentModel.ISupportInitialize)(this.TimerReconnect)).EndInit();



            // Just to be sure
            TimerReconnect.Enabled = false;

            // Set properties required for communication
            smppClientConnection.KeepAliveInterval = 10;
            
            smppClientConnection.OnTcpDisconnected += SMSCclientSMPP_OnTcpDisconnected;
            //smppClientConnection.OnSmppMessageReceived += SMSCclientSMPP_OnSmppMessageReceived;
            //smppClientConnection.OnSmppStatusReportReceived += SMSCclientSMPP_OnSmppStatusReportReceived;
            smppClientConnection.OnSmppSubmitResponseAsyncReceived += SMSCclientSMPP_OnSmppSubmitResponseAsyncReceived;
            smppClientConnection.ThrottleRate = 100;

            // Signal for automatic reconnecting that 
            // service initialization is finished
            bServiceReady = true;

            // Start timer for first lap
            TimerReconnect.Enabled = true;
        }


        public async Task<bool> SendSms(Sms sms)
        {
            int options = (int)SubmitOptionEnum.soRequestStatusReport;
            uint sequenceNumber = 0;
            int result = smppClientConnection.smppSubmitMessageAsync(
                                                    sms.To, 1, 1, sms.InstitutionName, 1, 1,
                                                    sms.Body, smscc.EncodingEnum.et7BitText, "", options, DateTime.Now, DateTime.Now.AddHours(1),
                                                    "", 0, $"TransactionNumber={sms.UniqueId}", out sequenceNumber);

            sms.SequenceNumber = Int32.Parse(sequenceNumber.ToString());
            return await Task.FromResult(true);
        }

        // Disconnected from SMSC
        private void SMSCclientSMPP_OnTcpDisconnected(object sender,
          smscc.tcpDisconnectedEventArgs e)
        {
            //iConnected = (iConnected > 0 ? iConnected - 1 : 0);

            //TimerMain.Enabled = false;

            //Logger.Log("[smscc] DisconnectedEvent", 1);
            //Send a message
        }

        // Status Report (SR) received from SMSC
        private void SMSCclientSMPP_OnSmppSubmitResponseAsyncReceived(object sender, smscc.SMPP.smppSubmitResponseAsyncReceivedEventArgs e)
        {
            //Logger.Log("[smscc] SubmitResponseAsyncReceivedEvent", 1);
            //Logger.Log($"[smscc] e.MessageID {e.MessageID}, sequenceNumber - {e.SequenceNumber}", 1);
            MessageDeliveredEventArgs entity = new MessageDeliveredEventArgs()
            {
                Status = MsgStatus.Submitted,
                SMSCMSGID = e.MessageID,
                SequenceNumber = Int32.Parse(e.SequenceNumber.ToString())
            };        
        
            if (this.OnMessageDelivered != null) this.OnMessageDelivered(entity);
        }
        //
        // This timer kicks once a while to see if we are still connected. 
        // If not then connect procedure is performed leaving application 
        // in either succesfully connected state (bConnected == true) or 
        // not connected (bConnected == false). 
        //
        // If still not connected timer will retry in few seconds.
        // 
        // In this approach it is not necessary to do Connect & Initialize sequence 
        // anywhere else. This event is responsible for both connecting and reconnecting
        // in an event connection is dropped.
        //
        // There are two flags which control reconnecting process:
        //
        // bConnected is set to false if service registers OnDisconnected event.
        // bServiceReady is important to avoid connection retries on shutdown.
        //
        private void TimerReconnect_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (bServiceReady)
            {

                if (!this.smppClientConnection.Connected)
                {
                    int result;

                    // Connect to the SMSC
                    result = this.smppClientConnection.tcpConnect(this.smppSmsSettings.IpAddress, this.smppSmsSettings.Port, "");

                    if (result != 0)
                    {
                        //Logger.Log("[smscc] Failed to connect with the SMSC", 1);
                    }
                    else
                    {
                        // Initialize session
                        //Get from DB
                        result = this.smppClientConnection.smppInitializeSession(this.smppSmsSettings.SystemID, this.smppSmsSettings.Password, 1, 1, "123456");

                        //if (result != 0)
                        //{
                        //    //Logger.Log("[smscc] Failed to initialize session with SMSC", 1);
                        //}
                        //else
                        //{
                        //    //Logger.Log("[smscc] Connected & initialized properly.", 1);

                        //    TimerMain.Enabled = true;
                        //}
                    }
                }
            }

            // Start timer for another round - notice that Timer 
            // component has AutoReset property set to false so 
            // that we have retrigger the timer for another lapse. 
            // This is done to avoid having another event triggered 
            // when lenghty reconnect is still taking place.
            TimerReconnect.Enabled = true;
        }

        public void Dispose()
        {
            // Signal for automatic reconnecting that 
            // service shutdown has been started
            bServiceReady = false;

            if (TimerReconnect != null)
            {
                // Stop reconnecting
                TimerReconnect.Enabled = false;
            }

            if (smppClientConnection != null && smppClientConnection.Connected)
            {
                // Disconnect from the SMSC
                smppClientConnection.tcpDisconnect();
            }
            //smppClientConnection = null;
        }
    }
    
}
