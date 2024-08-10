﻿using Microsoft.Xna.Framework;
using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Serialize
{
    public class SerializationInfo
    {
        public string TypeOfElement { get; set; }
        public Vector2 TilePos { get; set; }
        public TriggerType TriggerType { get; set; }
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }

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
