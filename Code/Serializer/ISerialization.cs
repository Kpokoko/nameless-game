using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Serialize
{
    public interface ISerialization
    {
        public SerializationInfo Info { get; set; }
    }
}
