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
    override public Vector2 RelativePosition { get; set; }
    new public bool Hovered { get { return MouseInputController.MouseBounds.Intersects(Bounds); } }

    private Vector2 _direction = Vector2.Zero;
    private Vector2 Direction 
    {
        get => _direction;
        set 
        { 
            _direction = value; 
            if (OnDirectionSet != null)
                OnDirectionSet(); 
        } 
    }
    private float _lenght = 1;
    private float Length
    {
        get => _lenght;
        set
        {
            _lenght = value;
            if (OnDirectionSet != null)
                OnDirectionSet();
        }
    }
    public Vector2 Vector { get => Direction * Length; }
    private float[] _lenghtValues { get; set; }
    private int _currentLength = 0;

    public event Action OnDirectionSet;

    public CircleController(Vector2 position, int width = 60) : base(position, width, width)
    {
        Bounds = new CircleF(position.ToPoint(), width);
        Globals.UIManager.CircleControllers.Add(this);
    }

    public void SetPossibleValues(params float[] values)
    {
        _lenghtValues = values;
        Length = _lenghtValues[0];
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

        MouseInputController.SetOnUIState(this);

        if (MouseInputController.RightButton.IsJustReleased)
        {
            Remove();
            return;
        }


        if (MouseInputController.MiddleButton.IsScrolled)
            ChangeSelectedLength();

        if (!MouseInputController.LeftButton.IsPressed) return;

        var dir = MouseInputController.MousePos - RelativePosition;
        dir.Normalize();
        if (Globals.KeyboardInputController.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            dir = AllignToAxis(dir);

        Direction = dir;
    }

    private void ChangeSelectedLength()
    {
        if (_lenghtValues != null)
        {
            if (MouseInputController.MiddleButton.IsScrolledDown)
            {
                if (_currentLength > 0)
                    _currentLength--;
            }
            else if (MouseInputController.MiddleButton.IsScrolledUp)
            {
                if (_currentLength < _lenghtValues.Length - 1)
                    _currentLength++;
            }

            Length = _lenghtValues[_currentLength];
        }
        else if (MouseInputController.MiddleButton.IsScrolledDown)
        {
            if (Direction.Length() < 0.1f)
                Length = 0;
            else
                Length -= 0.1f;
        }
        else if (MouseInputController.MiddleButton.IsScrolledUp)
            Length += 0.1f;
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
        spriteBatch.DrawCircle(Bounds, 20, Globals.PrimaryColor, 3,0.04f);
        spriteBatch.DrawPoint(RelativePosition + Vector * Bounds.Radius, Color.Red, 10, 0.04f);
    }
}
