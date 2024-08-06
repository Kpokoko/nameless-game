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

public class MyTuple<T1, T2>
{
    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }
    public MyTuple() { }
    public MyTuple(T1 item1, T2 item2)
    {
        Item1 = item1;
        Item2 = item2;
    }
}