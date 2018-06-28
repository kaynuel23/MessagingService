using Fingrid.Messaging.Processor.Implementation;
using Fingrid.Messaging.Processor.Interfaces;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Fingrid.Messaging.WindowsService.Utility
{
    public static class SimpleInjectorContainerExtensions
    {
        public static void RegisterAllDependencies(this Container container, string connectionString)
        {
            //container.Register<IUnitOfWork, MyUnitOfWork>(Lifestyle.Scoped);
            //container.Register<ILogger, TraceLogger>(Lifestyle.Singleton);
            //container.Register<IDataFactory>(() => new SqlDataFactory(connectionString));
            string eventSourceConnectionString = System.Configuration.ConfigurationManager.AppSettings["Fingrid.Messaging.EventSource.ConnectionString"];
            string eventSourceUsername = System.Configuration.ConfigurationManager.AppSettings["Fingrid.Messaging.EventSource.Username"];
            string eventSourcePassword = System.Configuration.ConfigurationManager.AppSettings["Fingrid.Messaging.EventSource.Password"];

            container.Register<Data.EventStorage.IEventStoreDataFactory>(() => new Data.EventStorage.EventStoreDataFactory(eventSourceConnectionString, eventSourceUsername, eventSourcePassword), (Lifestyle.Singleton));


            //Register all DAOs. 
            container.RegisterSpecial(typeof(Data.Dapper.SmsDapperDao).Assembly, "dao");
            container.RegisterSpecial(typeof(Data.EventStorage.SmsRequestEventStoreDao).Assembly, "dao");

            //Register the caches. They'll use decorator pattern to initially get the real data.
            //container.Register<ICacheStorage, SystemRuntimeCacheStorage>();

            //container.Register<StackExchange.Redis.IConnectionMultiplexer>(() => StackExchange.Redis.ConnectionMultiplexer.Connect("localhost:6379"), (Lifestyle.Singleton));
            //container.Register<ICacheStorage, Fingrid.Infrastructure.Common.Caching.Redis.RedisCacheStorage>();

            //container.Register<ISerializer, StackExchange.Redis.Extensions.Jil.JilSerializer>(Lifestyle.Singleton);
            //container.Register<ICacheClient, StackExchangeRedisCacheClient>(Lifestyle.Singleton);


            //container.Register<ISerializer>(() => new StackExchange.Redis.Extensions.Jil.JilSerializer(), (Lifestyle.Singleton));
            //container.Register<ICacheClient>(() => new StackExchangeRedisCacheClient(container.GetInstance<ISerializer>()), (Lifestyle.Singleton));
            //container.Register<ICacheStorage, Fingrid.Infrastructure.Common.Caching.Redis.RedisExtendedCacheStorage>();

            //Register all MemoryStore for Redis/In-Memory. 
            //container.RegisterSpecialDecorator(typeof(Data.MemoryStore.InstitutionCacheDao).Assembly, "dao");

            //Register all Services. 
            container.RegisterSpecial(typeof(Services.Implementation.SmsService).Assembly, "service");
            container.Register<Processor.SmsRequestProcessor>();

            container.Register(() => new SmppSmsSettings(1, "159.253.213.194", 2345, "appzacb", "acb123"), (Lifestyle.Singleton));
            //TODO: Override Default Collection Behaviour
            //container.Collection.Register(typeof(ISmsService), new[] { typeof(ISmsService).Assembly });
            var smsServiceTypes = container.GetTypesToRegister(typeof(ISmsService), new[] { typeof(ISmsService).Assembly });
            var smsServiceRegistrations = (from type in smsServiceTypes
                select Lifestyle.Singleton.CreateRegistration(type, container)).ToArray(); // This call is needed to prevent double enumeration!!
            container.Collection.Register<ISmsService>(smsServiceRegistrations);
            container.Register<ISmsServiceFactory, DefaultSmsServiceFactory>();

            //Register all Processor. 
            //container.RegisterSpecialProcessor(typeof(Processor.Processors.LoanCreditor).Assembly);

            //Register from Recover.Processor
            //container.RegisterCollection<IThirdPartyEnquiryAgent>(new[] { new Processor.BankEnquiryAgents.NibssEnquiryAgent(), new Processor.BankEnquiryAgents.NibssEnquiryAgent() });
            //container.RegisterCollection(typeof(IBankEnquiryAgent), new[] { typeof(IBankEnquiryAgent).Assembly });
            //container.RegisterCollection(typeof(ILoanEnquiryAgent), new[] { typeof(ILoanEnquiryAgent).Assembly });

            //container.Register<IBankEnquiryProvider, Processor.Implementation.DefaultBankEnquiryProvider>();
            //container.Register<Processor.Interfaces.ILoanEnquiryProvider, Processor.Implementation.DefaultLoanEnquiryProvider>();

            //container.Register<BlacklistController>();
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration, System.Reflection.Assembly.GetExecutingAssembly());
        }

        //public static void RegisterSpecialProcessor(this Container container, System.Reflection.Assembly repositoryAssembly)
        //{
        //    //var repositoryAssembly = type.Assembly;
        //    var registrations =
        //        from type in repositoryAssembly.GetExportedTypes()
        //            //where type.Namespace == "MyComp.MyProd.BL.SqlRepositories"
        //            //where type.Name.ToLower().EndsWith(endString.ToLower())
        //        where type.GetInterfaces().Any()
        //        select new { Service = type.GetInterfaces().Single(), Implementation = type };
        //    foreach (var reg in registrations)
        //    {
        //        container.Register(reg.Service, reg.Implementation, Lifestyle.Transient);
        //    }
        //}


        public static void RegisterSpecial(this Container container, System.Reflection.Assembly repositoryAssembly, string endString)
        {
            //var repositoryAssembly = type.Assembly;
            //try
            //{
                var registrations =
                        from type in repositoryAssembly.GetExportedTypes()
                            //where type.Namespace == "MyComp.MyProd.BL.SqlRepositories"
                where type.Name.ToLower().EndsWith(endString.ToLower())
                        where type.GetInterfaces().Any()
                        select new { Service = type.GetInterfaces().Where(i => i.Name.ToLower().EndsWith(endString)).FirstOrDefault(), Implementation = type };
                foreach (var reg in registrations)
                {
                    container.Register(reg.Service, reg.Implementation, Lifestyle.Transient);
                }
            //}
            //catch (Exception ex)
            //{

            //    System.IO.File.WriteAllText(@"C:\Logs\Fingrid.Messaging\ErrorFile.txt", $"StartUp Error repositoryAssembly- {repositoryAssembly} - {ex.Message}. {ex.StackTrace}");
            //}
        }

        public static void RegisterSpecialDecorator(this Container container, System.Reflection.Assembly repositoryAssembly, string endString)
        {
            //var repositoryAssembly = type.Assembly;
            var registrations =
                from type in repositoryAssembly.GetExportedTypes()
                    //where type.Namespace == "MyComp.MyProd.BL.SqlRepositories"
                where type.Name.ToLower().EndsWith(endString.ToLower())
                where type.GetInterfaces().Any()
                select new { Service = type.GetInterfaces().Single(), Implementation = type };
            foreach (var reg in registrations)
            {
                container.RegisterDecorator(reg.Service, reg.Implementation);
            }
        }
    }
}
