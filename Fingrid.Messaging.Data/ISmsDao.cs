using Fingrid.Messaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Data
{
    public interface ISmsDao
    {
        Task<bool> LogMessage(Sms sms);
        Task<bool> UpdateSmsStatus(Sms sms);
        Task<Sms> GetBySequenceNumber(string sequenceNumber);
        Task<int> UniqueIDExists(string uniqueID);
        Task<Sms> GetByUniqueID(string uniqueID);
        Task<Sms> GetByMessageID(string messageID);
    }
}
