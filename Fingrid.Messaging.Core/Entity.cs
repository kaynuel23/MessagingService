using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fingrid.Messaging.Core
{
    public class Entity : IEntity
    {
        public virtual long ID { get; set; }
    }
}
