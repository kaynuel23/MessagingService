using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Processor.Model
{
    public class InfoBipSingleSmsRequest
    {
        public string bulkId { get; set; }
        public List<Message> messages { get; set; }
    }

    public class Destination
    {
        public string to { get; set; }
        public string messageId { get; set; }
    }

    public class Message
    {
        public string from { get; set; }
        public List<Destination> destinations { get; set; }
        public string text { get; set; }
        public string notifyUrl { get; set; }
        public string notifyContentType { get; set; }
        public string callbackData { get; set; }
    }
}
