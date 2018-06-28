using Fingrid.Messaging.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Core
{
    public class SmsRequest : Sms
    {
        public override MessageType Type => MessageType.SMS;
    }
}
