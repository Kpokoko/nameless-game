using nameless.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.Entity;

namespace nameless.Entity;

public partial class Block : ISerializable
{
    public SerializationInfo Info { get; set; } = new();

    public virtual void PrepareSerializationInfo()
    {
        Info.TilePos = TilePosition;
        Info.TypeOfElement = this.GetType().Name;
    }
}
