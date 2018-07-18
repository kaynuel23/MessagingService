using Fingrid.Messaging.Core.Enums;
using Fingrid.Messaging.Processor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Processor.Implementation
{
    //public class IpIntegratedSmppSmsService : BaseSmppSmsService
    //{

    //    public IpIntegratedSmppSmsService(SmppSmsSettingsProvider smppSmsSettingsProvider) : base(smppSmsSettingsProvider)
    //    {
    //    }


    //    public override SmsServiceType SmsServiceType { get { return SmsServiceType.; } }
    //}
    public class SterlingIpIntegratedSmppSmsService : BaseSmppSmsService
    {
        public SterlingIpIntegratedSmppSmsService(ISmppSmsSettingsProvider smppSmsSettingsProvider) : base(smppSmsSettingsProvider)
        {
        }

        public override SmsServiceType SmsServiceType { get { return SmsServiceType.SterlingIPIntegrated; } }
    }
    
    public class DiamondIpIntegratedSmppSmsService : BaseSmppSmsService
    {
        public DiamondIpIntegratedSmppSmsService(ISmppSmsSettingsProvider smppSmsSettingsProvider) : base(smppSmsSettingsProvider)
        {
        }

        public override SmsServiceType SmsServiceType { get { return SmsServiceType.DiamondIPIntegrated; } }
    }

    public class AccessIpIntegratedSmppSmsService : BaseSmppSmsService
    {
        public AccessIpIntegratedSmppSmsService(ISmppSmsSettingsProvider smppSmsSettingsProvider) : base(smppSmsSettingsProvider)
        {
        }

        public override SmsServiceType SmsServiceType { get { return SmsServiceType.AccessIPIntegrated; } }
    }
    public class BOIIpIntegratedSmppSmsService : BaseSmppSmsService
    {
        public BOIIpIntegratedSmppSmsService(ISmppSmsSettingsProvider smppSmsSettingsProvider) : base(smppSmsSettingsProvider)
        {
        }

        public override SmsServiceType SmsServiceType { get { return SmsServiceType.BOIIPIntegrated; } }
    }
}
