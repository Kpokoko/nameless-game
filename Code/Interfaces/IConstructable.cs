using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Interfaces;

public interface IConstructable
{
    public void UpdateConstructor();
    public bool IsHolding { get; set; }
    public bool IsEnableToPlayer { get; set; }

}
