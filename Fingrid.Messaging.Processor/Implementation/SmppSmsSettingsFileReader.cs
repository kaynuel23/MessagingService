using Fingrid.Messaging.Processor.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Processor.Implementation
{
    public class SmppSmsSettingsFileReader : ISmppSmsSettingsFileReader
    {
        private string fileName;
        public SmppSmsSettingsFileReader(string fileName)
        {
            this.fileName = fileName;
        }

        public IEnumerable<SmppSmsSettings> ReadSettings()
        {
            return JsonConvert.DeserializeObject<List<SmppSmsSettings>>(System.IO.File.ReadAllText(this.fileName));
        }
    }
}
