using Fingrid.Messaging.Services.Interfaces;
using Fingrid.Messaging.WindowsService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Fingrid.Messaging.WindowsService.Controllers
{
    [RoutePrefix("apis/messaging/sms")]
    public class SmsController : ApiController
    {

        private readonly ISmsService smsService;

        public SmsController(ISmsService smsService)
        {
            if (smsService == null)
                throw new ArgumentNullException("smsService");

            this.smsService = smsService;
        }


        [HttpPost]
        [Route("LogSms")]
        public async Task<bool> LogSms(SmsModel sms)
        {
            return await this.smsService.LogMessage(sms.ConvertToSmsObject());
        }
    }
}