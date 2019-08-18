# MessagingService
A service that accepts SMS and sends to SMS Engine using SMPP library and Web service calls

A self-hosted WEB API Windows Service that accepts SMS requests and logs to an event store.
A subscribed service now picks up the event and sends the SMS to the designated address.

It was built using C# .NET.
