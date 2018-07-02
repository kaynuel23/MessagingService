using Fingrid.Messaging.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Core
{
    public abstract class Sms : Entity
    {
        public virtual SmsServiceType SmsServiceType { get; set; }
        public virtual string InstitutionCode { get; set; }
        public virtual int? SequenceNumber { get; set; }
        public virtual string SMSCMSGID { get; set; }
        public virtual string DeliveryStatus { get; set; }
        public virtual string MessageID { get; set; }
        /// <summary>
        /// Represents the institution name
        /// would be the source address that the customer would see
        /// </summary>        
        public virtual string InstitutionName { get; set; }

        public virtual string UniqueId { get; set; }
        /// <summary>
        /// Represents the body of the SMS
        /// </summary>
        public virtual string Body { get; set; }

        private string _aa;
        public virtual int NoOfParts { get; set; }

        public virtual string MaskedBody
        {
            get
            {
                if (String.IsNullOrEmpty(this.Body))
                {
                    return "";
                }

                string hashString = "";
                int ind1 = Body.IndexOf('[');
                if (ind1 < 1)
                    return Body;

                int ind2 = Body.IndexOf(']');
                if (ind2 < 1)
                    return Body;

                string sub = Body.Substring(ind1 + 1, ind2 - ind1 - 1);
                return Body.Replace(sub, hashString.PadRight(sub.Length, 'x'));
            }
            set { this._aa = value; }
        }

        
        /// <summary>
        /// Represents the ID of the mfb 
        /// </summary>
        public virtual long MfbID { get; set; }
        /// <summary>
        /// Represents the number of the customer who would recieve 
        /// the text message
        /// </summary>

        
        public virtual DateTime DateCreated { get; set; }


        
        /// <summary>
        /// Represents the date/time the sms was logged in the database
        /// </summary>
        public virtual DateTime? DateSent { get; set; }

        
        /// <summary>
        /// The Destination
        /// </summary>

        public virtual string To { get; set; }

        
        /// <summary>
        /// Represents no of times attempted to send a failed message
        /// </summary>
        public virtual int NoOfAttempts { get; set; }

        
        /// <summary>
        /// The account No. of the Customer...
        /// </summary>
        public virtual string AccountNo { get; set; }

        
        public virtual long AccountID { get; set; }

        
        /// <summary>
        /// Represents the status of the message i.e pending,successful
        /// </summary>
        //[EnumMember]
        public virtual MsgStatus Status { get; set; }

        public virtual SMSChargeType ChargeType { get; set; }


        /// <summary>
        /// Gives more information about the send/delivery status of message sent.
        /// i.e successful,pending,
        /// </summary>
        public virtual string StatusMsg { get; set; }

        
        /// <summary>
        /// Represents the ref no of the transaction being logged
        /// </summary>
        public virtual string ReferenceNo { get; set; }

        /// <summary>
        /// Represents the type of message to be logged
        /// </summary>
        //[EnumMember]
        public abstract MessageType Type { get; }
        
        public virtual bool IgnoreChargeOnInstitution { get; set; }
    }
}
