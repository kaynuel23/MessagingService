using Fingrid.Messaging.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Processor.Implementation
{
    public class SmppSmsSettings
    {
        //public SmppSmsSettings(int timerInt, string ipAddress, int port, string systemID, string password, string notifyUrl)
        //{
        //    this.TimerInterval = timerInt;
        //    this.IpAddress = ipAddress;
        //    this.Port = port;
        //    this.SystemID = systemID;
        //    this.Password = password;
        //    this.NotifyUrl = notifyUrl;
        //}

        //public SmppSmsSettings(SmppSmsSettings settings)
        //{
        //    this.TimerInterval = settings.TimerInterval;
        //    this.IpAddress = settings.IpAddress;
        //    this.Port = settings.Port;
        //    this.SystemID = settings.SystemID;
        //    this.Password = settings.Password;
        //    this.NotifyUrl = settings.NotifyUrl;
        //    this.Name = settings.Name;
        //}

        public int TimerInterval { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string SystemID { get; set; }
        public string Password { get; set; }
        public string NotifyUrl { get; set; }
        public SmsServiceType Name { get; set; }
    }

}
