using Fingrid.Messaging.Processor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fingrid.Messaging.Core;
using Fingrid.Messaging.Core.Enums;
using Vanso.SXMP;

namespace Fingrid.Messaging.Processor.Implementation
{
    public class MainstreetVansoSmsService : BaseSmppSmsService
    {

        public MainstreetVansoSmsService(SmppSmsSettings smppSmsSettings) : base(smppSmsSettings)
        {
        }

        public override SmsServiceType SmsServiceType { get { return SmsServiceType.SmppVanso; } }
    }
    public abstract class BaseVansoSmsService : ISmsService
    {
        public abstract SmsServiceType SmsServiceType { get; }
        public Func<IMessageDeliveredEventArgs, Task<bool>> OnMessageDelivered { get; set; }
        private readonly SmppSmsSettings smppSmsSettings;
        public BaseVansoSmsService(SmppSmsSettings smppSmsSettings)
        {
            this.smppSmsSettings = smppSmsSettings;
        }

        public async Task<bool> SendSms(Sms sms)
        {
            try
            {
                var request = new SubmitRequest();
                request.DeliveryReport = false; // Delivery reports will be posted to a customer specified URL. If no URL available, set to false
                request.account = new Account(this.smppSmsSettings.SystemID, this.smppSmsSettings.Password);
                request.Text = sms.Body;
                request.SourceAddress = new MobileAddress(MobileAddress.Type.alphanumeric, sms.InstitutionName);
                request.DestinationAddress = new MobileAddress(MobileAddress.Type.international, "+" + sms.To); // Number in International format - + sign, country code without leading 0
                var sender = new SXMPSender(@"http://sxmp.gw1.vanso.com", 80);
                //request.ReferenceID = "111";
                var resp = sender.Submit(request);
                if (resp.ErrorCode == (int)SXMPErrorCode.OK)
                {
                    sms.Status = MsgStatus.Successful;
                    sms.DeliveryStatus = ((SubmitResponse)resp).TicketID;
                }
                else
                {
                    sms.Status = MsgStatus.Failed;
                    sms.DeliveryStatus = string.Format("Sending failed: Error code = {0}, Error Description = {1}", resp.ErrorCode, resp.ErrorMessage);
                }
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                sms.Status = MsgStatus.Failed;
                sms.DeliveryStatus = string.Format("Sending failed: Error code = {0}, Error Description = {1}", "06", ex.Message);
                return await Task.FromResult(true);
            }
            
        }
    }
}
