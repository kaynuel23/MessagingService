using Fingrid.Messaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Services.Interfaces
{
    public interface ISmsService
    {
        Task<bool> LogMessage(SmsRequest sms);
    }
}
