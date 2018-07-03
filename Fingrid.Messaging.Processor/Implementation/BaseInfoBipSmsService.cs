using Fingrid.Messaging.Processor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fingrid.Messaging.Core;
using Fingrid.Messaging.Core.Enums;
using Fingrid.Messaging.Processor.Model;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace Fingrid.Messaging.Processor.Implementation
{
    public abstract class BaseInfoBipSmsService : ISmsService
    {
        public SmsServiceType SmsServiceType => SmsServiceType.SmppInfobip;

        public Func<IMessageDeliveredEventArgs, Task<bool>> OnMessageDelivered { get; set; }
        private readonly SmppSmsSettings smppSmsSettings;
        public BaseInfoBipSmsService(SmppSmsSettings smppSmsSettings)
        {
            this.smppSmsSettings = smppSmsSettings;
        }
        public async Task<bool> SendSms(Sms sms)
        {
            List<Message> listMsgs = new List<Message>();
            Message msg = new Message();
            msg.destinations = new List<Destination>();
            msg.destinations.Add(new Destination() { to = sms.To, messageId = "InfoMSG-" + sms.ID });
            msg.text = sms.Body;
            msg.notifyUrl = this.smppSmsSettings.NotifyUrl;
            msg.notifyContentType = "application/json";
            msg.from = sms.InstitutionName;

            listMsgs.Add(msg);

            InfoBipSingleSmsRequest smsToSend = new InfoBipSingleSmsRequest()
            {
                bulkId = string.Empty,
                messages = listMsgs
            };

            var result = MakeRequest(this.smppSmsSettings.IpAddress, smsToSend, "POST", "application/json");
            if (result)
            {
                sms.Status = MsgStatus.Successful;
            }
            else
            {
                sms.Status = MsgStatus.Failed;
            }            
            return await Task.FromResult(result);
        }
        public static bool MakeRequest(string requestUrl, object JSONRequest, string JSONmethod, string JSONContentType, Type JSONResponseType = null)
        {
            try
            {
                string InfoBipPasswordHash = System.Configuration.ConfigurationManager.AppSettings["InfoBipPasswordHash"];
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                string sb = JsonConvert.SerializeObject(JSONRequest);
                request.Method = JSONmethod;// "POST";
                request.ContentType = JSONContentType; // "application/json";
                request.Headers.Add("authorization", $"Basic {InfoBipPasswordHash}"); // Appzone user name and password with basic authentication 
                Byte[] bt = Encoding.UTF8.GetBytes(sb);
                Stream st = request.GetRequestStream();
                st.Write(bt, 0, bt.Length);
                st.Close();
                var response = request.GetResponse() as HttpWebResponse;

                Stream stream1 = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream1);
                string strsb = sr.ReadToEnd();
                object objResponse = JsonConvert.DeserializeObject(strsb, JSONResponseType);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}

