using Fingrid.Messaging.Core;
using Fingrid.Messaging.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Processor.Interfaces
{
    public interface ISmsService
    {
        SmsServiceType SmsServiceType { get; }
        Func<IMessageDeliveredEventArgs, Task<bool>> OnMessageDelivered { get; set; }
        Task<bool> SendSms(Sms sms);
    }


    public interface IMessageDeliveredEventArgs
    {
        MsgStatus Status { get; set; }
        string SMSCMSGID { get; set; }
        int? SequenceNumber { get; set; }
        string DeliveryStatus { get; set; }
        string MessageID { get; set; }
        string UniqueID { get; set; }
        bool HasDeliveryReport { get; set; }
    }
    public class MessageDeliveredEventArgs : IMessageDeliveredEventArgs
    {
        public MsgStatus Status { get; set; }
        public string SMSCMSGID { get; set; }
        public int? SequenceNumber { get; set; }
        public string DeliveryStatus { get; set; }
        public string MessageID { get; set; }
        public string UniqueID { get; set; }
        public bool HasDeliveryReport { get; set; }
    }
}
