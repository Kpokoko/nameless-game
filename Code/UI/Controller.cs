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
            if (_lenghtValues != null)
            {
                CurrentLengthIndex = Array.IndexOf(_lenghtValues, _lenghtValues.MinBy(l => Math.Abs(l - value)));
                _lenght = _lenghtValues[CurrentLengthIndex];
            }
            else
                _lenght = value;
            InvokeEvent();
        }
    }
    protected float[] _lenghtValues { get; set; }
    public int CurrentLengthIndex = 0;

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
                if (CurrentLengthIndex > 0)
                    CurrentLengthIndex--;
            }
            else if (MouseInputController.MiddleButton.IsScrolledUp)
            {
                if (CurrentLengthIndex < _lenghtValues.Length - 1)
                    CurrentLengthIndex++;
            }

            Length = _lenghtValues[CurrentLengthIndex];
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
