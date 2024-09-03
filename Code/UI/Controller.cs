using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nameless.Controls;
using nameless.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public abstract class Controller : UIElement
{

    private float _lenght = 1;
    public float Length
    {
        get => _lenght;
        set
        {
            _lenght = value;
            InvokeEvent();
        }
    }
    protected float[] _lenghtValues { get; set; }
    protected int _currentLength = 0;

    public event Action OnValueSet;

    public List<Controller> ControllerGroup;
    protected bool GroupUnderMouse { get => ControllerGroup.Any(c => c.UnderMouse); }

    public Controller(Vector2 position, int width = 60, int height = 60) : base(position, width, height)
    {
        Globals.UIManager.Controllers.Add(this);
    }

    public void InvokeEvent()
    {
        if (OnValueSet != null)
            OnValueSet();
    }

    public void SetPossibleValues(params float[] values)
    {
        _lenghtValues = values;
        Length = _lenghtValues[0];
    }

    public override void Remove()
    {
        Globals.UIManager.Controllers.Remove(this);
    }
    public abstract void Draw(SpriteBatch spriteBatch);

    public virtual void Update()
    {
        if (!UnderMouse && (ControllerGroup == null || !GroupUnderMouse))
        {
            if (MouseInputController.IsJustPressed)
                Globals.UIManager.ToRemove.Add(this);
            return;
        }
    
        if (UnderMouse)
            MouseInputController.SetOnUIState(this);

        if (MouseInputController.RightButton.IsJustReleased)
        {
            Globals.UIManager.ToRemove.Add(this);
            return;
        }

        if (!Hovered)
            return;

        if (MouseInputController.MiddleButton.IsScrolled)
            ChangeSelectedLength();
    }

    protected void ChangeSelectedLength()
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
            if (Length < 0.1f)
                Length = 0;
            else
                Length -= 0.1f;
        }
        else if (MouseInputController.MiddleButton.IsScrolledUp)
            Length += 0.1f;
    }
}
