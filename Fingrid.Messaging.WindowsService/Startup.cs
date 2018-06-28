using Fingrid.Messaging.WindowsService.Utility;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Fingrid.Messaging.WindowsService
{
    class Startup
    {
        public static SimpleInjector.Container Container; 
        //  Hack from http://stackoverflow.com/a/17227764/19020 to load controllers in 
        //  another assembly.  Another way to do this is to create a custom assembly resolver
        Type valuesControllerType = typeof(Fingrid.Messaging.WindowsService.Controllers.SmsController);

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            //System.Threading.Thread.Sleep(70000);//.
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Fingrid.Messaging"].ConnectionString;

            Container = new SimpleInjector.Container();
            //Container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            Container.Options.DefaultScopedLifestyle = new SimpleInjector.Lifestyles.AsyncScopedLifestyle();
            Container.RegisterAllDependencies(connectionString);

            Container.Verify();


            try
            {

                //GlobalConfiguration.Configuration.DependencyResolver =
                //    new SimpleInjector.Integration.WebApi.SimpleInjectorWebApiDependencyResolver(Container);
                //Fingrid.Infrastructure.Common.Logging.ILogger logger = Container.GetInstance<Fingrid.Infrastructure.Common.Logging.ILogger>();

                // Configure Web API for self-host. 
                HttpConfiguration config = new HttpConfiguration()
                {
                    DependencyResolver = new SimpleInjector.Integration.WebApi.SimpleInjectorWebApiDependencyResolver(Container)
                };

                //config.EnableCors();

                //  Enable attribute based routing
                //  http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
                config.MapHttpAttributeRoutes();

                //config.Routes.MapHttpRoute(
                //    name: "DefaultApi",
                //    routeTemplate: "api/{controller}/{id}",
                //    defaults: new { id = RouteParameter.Optional }
                //);

                appBuilder.UseWebApi(config);
            }
            catch (Exception)
            {
                //logger.LogError(ex);
                throw;
            }

        }
    }
}
