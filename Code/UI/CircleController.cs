using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class CircleController : UIElement
{
    new private CircleF Bounds {  get; set; }
    override public Vector2 Position { get; set; }
    new public bool Hovered { get { return MouseInputController.MouseBounds.Intersects(Bounds); } }
    public Vector2 Direction 
    {
        get => _direction;
        private set 
        { 
            _direction = value; 
            if (OnDirectionSet != null)
                OnDirectionSet(); 
        } 
    }
    private Vector2 _direction = Vector2.Zero;
    public float Length { get { return Direction.Length(); } }

    public event Action OnDirectionSet;
    public CircleController(Vector2 position, int width = 60) : base(position, width, width)
    {
        Bounds = new CircleF(position.ToPoint(), width);
        Globals.UIManager.CircleControllers.Add(this);
    }

    public override void Remove()
    {
        if (!Globals.UIManager.ToRemove.Contains(this))
            Globals.UIManager.ToRemove.Add(this);
        else
            Globals.UIManager.CircleControllers.Remove(this);
    }

    public void Update()
    {
        if (!Hovered)
        {
            if (MouseInputController.IsJustPressed)
                Remove();
            return;
        }

        MouseInputController.SetOnUIState();

        if (MouseInputController.RightButton.IsJustReleased)
        {
            Remove();
            return;
        }


        if (MouseInputController.MiddleButton.IsScrolled)
            ChangeSelectedLength();

        if (!MouseInputController.LeftButton.IsPressed) return;

        var dir = MouseInputController.MousePos - Position;
        dir.Normalize();
        if (Globals.KeyboardInputController.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            dir = AllignToAxis(dir);
        Direction = dir;
    }

    private void ChangeSelectedLength()
    {
        if (MouseInputController.MiddleButton.IsScrolledDown)
            if (Direction.Length() < 0.1f)
                Direction = Vector2.Zero;
            else
                Direction = Direction - Direction.NormalizedCopy() / 10;
        if (MouseInputController.MiddleButton.IsScrolledUp)
            if (Direction.Length() != 0)
                Direction = Direction + Direction.NormalizedCopy() / 10;
    }

    private Vector2 AllignToAxis(Vector2 dir)
    {
        var axisVectors = new List<Vector2>();
        for (int i = -1;i<2;i++) 
            for (int j = -1;j<2;j++)
                if (!(i==0 && j==0))
                    axisVectors.Add(new Vector2(i,j).NormalizedCopy());
        var newDir = axisVectors.MinBy(v => (dir - v).Length());
        return newDir;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawCircle(Bounds, 20, Globals.PrimaryColor, 3,0.9f);
        spriteBatch.DrawPoint(Position + Direction * Bounds.Radius, Color.Red, 10, 0.92f);
    }
}
