using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI.Scenes;

public class ConstructorScene
{
    private List<IUI> _elements = new();
    public ConstructorScene()
    {
        var button1 = new Button(new Vector2(1700, 150), 280, 50, "Inventory Block",ButtonActivationProperty.Click);
        var button2 = new Button(new Vector2(1700, 250), 280, 50, "EditorBlock", ButtonActivationProperty.Switch);
        var button3 = new Button(new Vector2(1700, 350), 280, 50, "Block", ButtonActivationProperty.Switch);
        _elements = _elements.Concat(new[] { button1, button2, button3 }).ToList();
        button1.OnClickEvent += () => { button2.Deactivate(); button3.Deactivate(); };
        button2.OnClickEvent += () => { button1.Deactivate(); button3.Deactivate(); };
        button3.OnClickEvent += () => { button2.Deactivate(); button1.Deactivate(); };

        button1.OnClickEvent += () => Globals.Constructor.SelectedEntity = "InventoryBlock";
        button2.OnClickEvent += () => Globals.Constructor.SelectedEntity = "EditorBlock";
        button3.OnClickEvent += () => Globals.Constructor.SelectedEntity = "Block";

        button1.SetKeyboardKey(Keys.D1);
        button2.SetKeyboardKey(Keys.D2);
        button3.SetKeyboardKey(Keys.D3);
    }

    public void Clear()
    {
        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].Remove();
        }
        _elements.Clear();
    }
}
