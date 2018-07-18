
using Fingrid.Messaging.Core.Enums;
using Fingrid.Messaging.Processor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Processor.Implementation
{
    public class DefaultSmsServiceProvider : ISmsServiceProvider
    {
        private readonly Dictionary<SmsServiceType, ISmsService> smsServiceDict;
        public DefaultSmsServiceProvider(IEnumerable<ISmsService> smsServices)
        {
            if(smsServices == null || smsServices.Count() == 0)
            {
                throw new ArgumentNullException("smsServices");
            }

            this.smsServiceDict = new Dictionary<SmsServiceType, ISmsService>();
            foreach(var item in smsServices)
            {
                if(this.smsServiceDict.ContainsKey(item.SmsServiceType))
                {
                    throw new Exception($"SmsServiceType '{item.SmsServiceType}' has already been implemented.");
                }

                this.smsServiceDict.Add(item.SmsServiceType, item);
            }
        }

        public ISmsService GetService(string institutionCode)
        {
            SmsServiceType serviceType = SmsServiceType.AccessIPIntegrated;
            if(institutionCode == "0383833")  //TODO: International instituion
            {
                serviceType = SmsServiceType.InfoBip;
            }

            ISmsService result = null;
            if (!this.smsServiceDict.TryGetValue(serviceType, out result)) throw new NotImplementedException($"{serviceType} has not been implemented.");

            return result;
        }
    }
}
