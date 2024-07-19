using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Interfaces;

public interface IUI
{
    public Rectangle Bounds { get; set; }
    public void Remove();
}
