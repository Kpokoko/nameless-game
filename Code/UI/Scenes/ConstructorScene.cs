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

public class ConstructorScene : UIScene
{
    public ConstructorScene()
    {
        Name = UIScenes.ConstructorScene;

        var container = new Container(new Vector2(1700, 300), 300, 300, Alignment.Center, FlexDirection.Vertical, Vector2.Zero);

        var button1 = new Button(Vector2.Zero, 280, 50, "InventoryBlock", ButtonActivationProperty.Switch);
        var button2 = new Button(Vector2.Zero, 280, 50, "EditorBlock", ButtonActivationProperty.Switch);
        var button3 = new Button(Vector2.Zero, 280, 50, "Block", ButtonActivationProperty.Switch);

        container.AddElements(button1,button2,button3);
        AddElements(container);

        //var button1 = new Button(new Vector2(1700, 150), 280, 50, "InventoryBlock", ButtonActivationProperty.Switch);
        //var button2 = new Button(new Vector2(1700, 250), 280, 50, "EditorBlock", ButtonActivationProperty.Switch);
        //var button3 = new Button(new Vector2(1700, 350), 280, 50, "Block", ButtonActivationProperty.Switch);

        button1.OnClickEvent += () => { button2.Deactivate(); button3.Deactivate(); };
        button2.OnClickEvent += () => { button1.Deactivate(); button3.Deactivate(); };
        button3.OnClickEvent += () => { button2.Deactivate(); button1.Deactivate(); };

        button1.OnClickEvent += () => Globals.Constructor.SelectedEntity = EntityType.InventoryBlock;
        button2.OnClickEvent += () => Globals.Constructor.SelectedEntity = EntityType.EditorBlock;
        button3.OnClickEvent += () => Globals.Constructor.SelectedEntity = EntityType.Block;

        button1.SetKeyboardKey(Keys.D1);
        button2.SetKeyboardKey(Keys.D2);
        button3.SetKeyboardKey(Keys.D3);
    }
}