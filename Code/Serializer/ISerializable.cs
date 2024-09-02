using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Serialize
{
    public interface ISerializable
    {
        public SerializationInfo Info { get; set; }

        public bool AllowSerialization { get; set; }

        void PrepareSerializationInfo();
    }
}
