using Fingrid.Messaging.Core.Enums;
using Fingrid.Messaging.Processor.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Processor.Interfaces
{

    public interface ISmppSmsSettingsProvider
    {
        SmppSmsSettings GetSetting(SmsServiceType smsServiceType);
    }

}
