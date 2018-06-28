using Fingrid.Messaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.WindowsService.Models
{
    public class SmsModel
    {
        public string PhoneNo { get; set; }
        public string Body { get; set; }
        public string UniqueId { get; set; }
        public string AccountNo { get; set; }
        public string InstitutionName { get; set; }
        public string APPName { get; set; }

        public SmsRequest ConvertToSmsObject()
        {
            string uniqueID = $"{this.APPName}_{ this.UniqueId }";
            if (string.IsNullOrEmpty(uniqueID))
            {
                uniqueID = Guid.NewGuid().ToString();
            }
            return new SmsRequest {
                Body = this.Body,
                To = this.PhoneNo,
                UniqueId = uniqueID,
                AccountNo = this.AccountNo,
                InstitutionName = this.InstitutionName
            };
        }
    }
}
