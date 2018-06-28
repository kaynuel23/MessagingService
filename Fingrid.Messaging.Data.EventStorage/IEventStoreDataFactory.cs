using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Data.EventStorage
{
    public interface IEventStoreDataFactory
    {
        IEventStoreConnection Connection { get; }

        string Username { get; }
        string Password { get; }
    }
}
