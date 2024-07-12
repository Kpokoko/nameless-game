using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Collisions;

public enum ReactOnProperty
{
    ReactOnEntityType,
    ReactOnColliderId
}

public enum SignalProperty
{
    Once,
    OnceOnEveryContact,
    Continuous
}

public enum ActivateProperty
{
    OnEntry,
    OnExit
}
