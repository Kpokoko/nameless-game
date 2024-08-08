using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity
{
    public enum PlayerState
    {
        Jumping,
        Falling,
        Still,
        Static
    }

    public enum MovableState
    {
        Left,
        Right,
        Free
    }
}
