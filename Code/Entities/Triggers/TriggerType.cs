using Microsoft.Xna.Framework;
using nameless.Collisions;
using nameless.Entity;
using nameless.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public enum TriggerType
{
    None,
    SwitchScene,
    DamagePlayer,
    Disposable,
    Saver,
}
