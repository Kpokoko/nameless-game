using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public class StickyBlock : Block, IConstructable
{
    public StickyBlock(int x, int y) : base(x, y)
    { Colliders[0].Color = Color.LimeGreen; IsEnableToPlayer = true; }
}
