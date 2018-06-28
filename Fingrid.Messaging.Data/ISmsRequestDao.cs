using Fingrid.Messaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Data
{
    public interface ISmsRequestDao
    {
        Task<bool> LogMessage(SmsRequest sms);
    }
}
