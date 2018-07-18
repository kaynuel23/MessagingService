using Fingrid.Messaging.Core.Enums;
using Fingrid.Messaging.Processor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Processor.Implementation
{
    public class SmppSmsSettingsProvider : ISmppSmsSettingsProvider
    {
        Dictionary<SmsServiceType, SmppSmsSettings> settingsDict = null;
        public SmppSmsSettingsProvider(ISmppSmsSettingsFileReader fileReader)
        {
            this.settingsDict = fileReader.ReadSettings().ToDictionary(a => a.Name);
        }

        public SmppSmsSettings GetSetting(SmsServiceType smsServiceType)
        {
            SmppSmsSettings setting = null;
            if (!this.settingsDict.TryGetValue(smsServiceType, out setting))
            {
                throw new Exception($"Invalid Setting {smsServiceType}");
            }

            return setting;
        }
    }

}
