using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Data.EventStorage
{
    public class EventStoreDataFactory : IEventStoreDataFactory
    {
        private readonly IEventStoreConnection connection;
        public EventStoreDataFactory(string connectionString, string username, string password)
        {
            this.Username = username;
            this.Password = password;
            connection = EventStoreConnection.Create(connectionString);
            //EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));

            // Don't forget to tell the connection to connect!
            connection.ConnectAsync().Wait();

            //Should be run ONLY once actually.
            try
            {
                EventStore.ClientAPI.SystemData.UserCredentials credentials = new EventStore.ClientAPI.SystemData.UserCredentials(username, password);
                PersistentSubscriptionSettings settings = PersistentSubscriptionSettings.Create().DoNotResolveLinkTos().StartFromCurrent();
                connection.CreatePersistentSubscriptionAsync("fingrid.messaging.smsrequest", "fingrid.messaging.smsrequest.group", settings, credentials).Wait();
            }
            catch { }
        }

        public IEventStoreConnection Connection { get { return this.connection; } }

        public string Password { get; private set; }

        public string Username { get; private set; }
    }
}
