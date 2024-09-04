using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Controls;
using nameless.Engine;
using nameless.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class BarController : Controller
{
    public BarController(Vector2 position, int width = 60) : base(position, width, width)
    {
        Bounds = new CRectangle(position.ToPoint(), new Point(width, 15));
        Globals.UIManager.BarControllers.Add(this);
    }

    //public override void Remove()
    //{
    //    Globals.UIManager.BarControllers.Remove(this);
    //}


    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawRectangle(Bounds.RectangleF, Globals.PrimaryColor, 3, 0.04f);
        spriteBatch.DrawPoint(RelativePosition.SetX(5 + RelativePosition.X - Bounds.Width / 2 + CurrentLengthIndex * (Bounds.Width / _lenghtValues.Length)), Color.Red, 10, 0.04f);
        spriteBatch.DrawString(ResourceManager.Font, Length.ToString(), RelativePosition + new Vector2(0, 15), Color.Black);
    }
}
