using EventStore.ClientAPI;
using Fingrid.Messaging.Core;
using Fingrid.Messaging.Data;
using Fingrid.Messaging.Data.EventStorage;
using Fingrid.Messaging.Processor.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Processor
{

    public class SmsRequestProcessor
    {

        private const string STREAM = "fingrid.messaging.smsrequest";
        private const string GROUP = "fingrid.messaging.smsrequest.group";
        private readonly ISmsDao smsDao;
        private readonly ISmsServiceProvider smsServiceFactory;
        private readonly IEventStoreDataFactory dataFactory;
        private readonly EventStore.ClientAPI.SystemData.UserCredentials userCred = null;

        public SmsRequestProcessor(IEventStoreDataFactory dataFactory, ISmsDao smsDao, ISmsServiceProvider smsServiceFactory)
        {
            if (dataFactory == null)
                throw new ArgumentNullException("dataFactory");
            if (smsDao == null)
                throw new ArgumentNullException("smsDao");
            if (smsServiceFactory == null)
                throw new ArgumentNullException("smsServiceFactory");

            this.userCred = new EventStore.ClientAPI.SystemData.UserCredentials(dataFactory.Username, dataFactory.Password);
            this.smsDao = smsDao;
            this.dataFactory = dataFactory;
            this.smsServiceFactory = smsServiceFactory;
        }
        public bool Start()
        {
            ConnectToSubscription();
            return true;
        }

        private void ConnectToSubscription()
        {
            var bufferSize = 50;
            var autoAck = true;

            //_subscription = 
            this.dataFactory.Connection.ConnectToPersistentSubscription(STREAM, GROUP, EventAppeared, SubscriptionDropped,
            this.userCred, bufferSize, autoAck);
        }

        private void SubscriptionDropped(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase,
            SubscriptionDropReason subscriptionDropReason, Exception ex)
        {
            ConnectToSubscription();
        }

        private async Task<bool> EventAppeared(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase,
            ResolvedEvent resolvedEvent)
        {
            bool isSuccessful = true;

            SmsRequest smsRequest = JsonConvert.DeserializeObject<SmsRequest>(Encoding.UTF8.GetString(resolvedEvent.Event.Data));
            if (smsRequest != null)
            {
                Sms sms = smsRequest as Sms;
                var result = await this.smsDao.UniqueIDExists(sms.UniqueId);
                if (result == 0)
                {
                    await this.smsDao.LogMessage(sms);
                    try
                    {
                        ISmsService smsService = this.smsServiceFactory.GetService(sms.InstitutionCode);
                        smsService.OnMessageDelivered += DeliveredMessage;

                        var isSmsSendSuccessful = await smsService.SendSms(sms);
                    }
                    catch (Exception ex)
                    {
                        //throw ex;
                        sms.StatusMsg = ex.Message;
                        sms.Status = Core.Enums.MsgStatus.Failed;
                    }

                    await this.smsDao.UpdateSmsStatus(sms);
                }
                //System.IO.File.WriteAllText(@"C:\Logs\Fingrid.Messaging\TestFile.txt", $"Something was created. {smsRequest.UniqueId}, {smsRequest.To}, {smsRequest.Body}");
            }

            return isSuccessful;
        }

        private async Task<bool> DeliveredMessage(IMessageDeliveredEventArgs messageArgs)
        {
            Sms message = null;
            if (messageArgs.HasDeliveryReport)
            {
                message = await this.smsDao.GetByMessageID(messageArgs.MessageID);//or messageid
            }
            else
            {
                message = await this.smsDao.GetBySequenceNumber(messageArgs.SequenceNumber.ToString());
            }

            if (message != null)
            {
                message.DeliveryStatus = messageArgs.DeliveryStatus;
                message.Status = messageArgs.Status;
                message.SMSCMSGID = messageArgs.SMSCMSGID;
                message.MessageID = messageArgs.MessageID;
                message.SequenceNumber = messageArgs.SequenceNumber.HasValue ? Int32.Parse(messageArgs.SequenceNumber.ToString()) : (int?)null;
                await this.smsDao.UpdateSmsStatus(message);
            }
            return await Task.FromResult(true);
        }

    }
}
