using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fingrid.Messaging.Core;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Fingrid.Messaging.Data.EventStorage
{
    public class SmsRequestEventStoreDao : ISmsRequestDao
    {
        private readonly IEventStoreDataFactory dataFactory;
        //private readonly string groupName;
        public SmsRequestEventStoreDao(IEventStoreDataFactory dataFactory)
        {
            if (dataFactory == null)
            {
                throw new ArgumentNullException("dataFactory");
            }

            this.dataFactory = dataFactory;
        }


        public async Task<bool> LogMessage(SmsRequest sms)
        {
            EventData eventData = new EventData(Guid.NewGuid(), "smsInfo", true,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sms)), null);

            await this.dataFactory.Connection.AppendToStreamAsync("fingrid.messaging.smsrequest", ExpectedVersion.Any, eventData);
            return true;
        }

    }
}
