using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public static class Extensions
{
    public static string ToSimpleString(this Vector2 vector)
    {
        return (vector.X.ToString() + ' ' + vector.Y.ToString());
    }

    public static string ToSimpleString(this Point point)
    {
        return (point.X.ToString() + ' ' + point.Y.ToString());
    }
}
