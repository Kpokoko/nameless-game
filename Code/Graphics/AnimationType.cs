using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Graphics;

public enum AnimationType
{
    None,

    Idle,
    IdleLeft,
    IdleRight,

    Move,
    MoveLeft,
    MoveRight,

    Jump,
    JumpRight,
    JumpLeft,

    Falling,
    FallingLeft,
    FallingRight,

    Landing,
    LandingLeft,
    LandingRight,

    Hit,
    HitLeft,
    HitRight,

    Up,
    Down,
}
