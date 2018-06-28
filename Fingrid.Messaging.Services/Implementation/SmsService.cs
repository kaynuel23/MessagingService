using Fingrid.Messaging.Data;
using Fingrid.Messaging.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fingrid.Messaging.Core;

namespace Fingrid.Messaging.Services.Implementation
{
    public class SmsService : ISmsService
    {
        private readonly ISmsRequestDao smsRequestDao;

        public SmsService(ISmsRequestDao smsRequestDao)
        {
            if (smsRequestDao == null)
                throw new ArgumentNullException("smsRequestDao");

            this.smsRequestDao = smsRequestDao;
        }

        public async Task<bool> LogMessage(SmsRequest sms)
        {
            //TODO: Validations
            //check if sms is disabled
            //check if the sms is a postdate
            //check if account no is null or empty
            //get countryThreeDigitPrefix
            string countryThreeDigitPrefix = "";
            if (!String.IsNullOrEmpty(sms.To) && !String.IsNullOrEmpty(sms.To.Trim())
                            && sms.To.Trim() != "0" && sms.To.Trim() != countryThreeDigitPrefix
                            && !System.Text.RegularExpressions.Regex.IsMatch(sms.To, "^[a-zA-Z]*$"))
            {
                if (string.IsNullOrEmpty(sms.AccountNo)) sms.AccountNo = "";
                if (string.IsNullOrEmpty(sms.To)) return false;

                if (sms.AccountNo.ToLower().Trim().StartsWith("appzone") || sms.AccountNo.ToLower().Trim().StartsWith("b-day")
                     || sms.AccountNo.ToLower().Trim().StartsWith("broadcast"))
                {
                    sms.ChargeType = Core.Enums.SMSChargeType.NonChargable;
                }           

                sms.Body = sms.Body.Replace("Deposit", "Dep").Replace("Withdrawals", "Wth")
                                    .Replace("Withdrawal", "Wth").Replace("Instruments", "Insts.")
                                    .Replace("Instrument", "Inst.");
                sms.NoOfAttempts = 0;
                sms.StatusMsg = "";
                sms.Status = Core.Enums.MsgStatus.Pending;

                sms.To = sms.To.Trim();

                sms.To = sms.To.Replace('o', '0'); //Replace any small letter 'o' with zero
                sms.To = sms.To.Replace('O', '0');//Replace any capital letter 'O' with zero
                sms.To = sms.To.Replace("-", ""); //Remove any dashes
                if (sms.To.StartsWith("0"))
                {
                    sms.To = sms.To.Remove(0, 1);
                }
                if (!sms.To.StartsWith(countryThreeDigitPrefix))
                {
                    sms.To = string.Format("{0}{1}", countryThreeDigitPrefix, sms.To);
                }                
            }
                return await this.smsRequestDao.LogMessage(sms);
        }
    }
}
