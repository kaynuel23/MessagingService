using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.WindowsService
{
    public partial class SmsService : ServiceBase
    {

        private IDisposable _server = null;
        public SmsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _server = WebApp.Start<Startup>(url: System.Configuration.ConfigurationManager.AppSettings["Fingrid.Messaging.Api.Url"]);
            try
            {
                Startup.Container.GetInstance<Processor.SmsRequestProcessor>().Start();
            }
            catch (Exception ex)
            {

                System.IO.File.WriteAllText(@"C:\Logs\Fingrid.Messaging\ErrorFile.txt", $"StartUp Error - {ex.Message}. {ex.StackTrace}");
                
            }
        }

        protected override void OnStop()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
        }
    }
}
