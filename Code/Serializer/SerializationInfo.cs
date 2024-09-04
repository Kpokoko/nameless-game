using Microsoft.Xna.Framework;
using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace nameless.Serialize
{
    [XmlType(TypeName = "I")]
    public class SerializationInfo
    {
        [XmlElement("Type")]
        public string TypeOfElement { get; set; }
        public Vector2 TilePos { get; set; }
        public TriggerType TriggerType { get; set; } = TriggerType.None;
        public bool TriggerTypeSpecified => TriggerType != TriggerType.None;
        public Vector2 Direction { get; set; } = Vector2.Zero;
        public bool DirectionSpecified => Direction != Vector2.Zero;
        public float Speed { get; set; } = 0;
        public bool SpeedSpecified => Speed != 0;

        public SerializationInfo Clone()
        {
            SerializationInfo info = new SerializationInfo()
            {
                TypeOfElement = TypeOfElement,
                TilePos = TilePos,
                Direction = Direction,
                Speed = Speed,
                TriggerType = TriggerType,
            };
            return info;
        }
    }
}
