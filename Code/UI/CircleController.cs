using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class CircleController : Controller
{
    new private CircleF Bounds {  get; set; }
    override public bool UnderMouse { get => MouseInputController.MouseBounds.Intersects(Bounds); }

    private Vector2 _direction = Vector2.Zero;
    private Vector2 Direction 
    {
        get => _direction;
        set 
        { 
            _direction = value;
            InvokeEvent();
        } 
    }
    public Vector2 Vector { get => Direction * Length; }

    public CircleController(Vector2 position, int width = 60) : base(position, width, width)
    {
        Bounds = new CircleF(position.ToPoint(), width);
        //Globals.UIManager.CircleControllers.Add(this);
    }

    //public override void Remove()
    //{
    //    Globals.UIManager.CircleControllers.Remove(this);
    //}

    public override void Update()
    {
        base.Update();
        if (Globals.UIManager.ToRemove.Contains(this))
            return;

        if (!Hovered)
            return;

        if (!MouseInputController.LeftButton.IsPressed) return;

        var dir = MouseInputController.MousePos - RelativePosition;
        dir.Normalize();
        if (Globals.KeyboardInputController.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            dir = AllignToAxis(dir);

        Direction = dir;
    }

    private Vector2 AllignToAxis(Vector2 dir)
    {
        var axisVectors = new List<Vector2>();
        for (int i = -1;i<2;i++) 
            for (int j = -1;j<2;j++)
                if (!(i==0 && j==0) && (Math.Abs(i) + Math.Abs(j) == 1))
                    axisVectors.Add(new Vector2(i,j).NormalizedCopy());
        var newDir = axisVectors.MinBy(v => (dir - v).Length());
        return newDir;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawCircle(Bounds, 20, Globals.PrimaryColor, 3,0.04f);
        spriteBatch.DrawPoint(RelativePosition + Vector * Bounds.Radius, Color.Red, 10, 0.04f);
    }
}
