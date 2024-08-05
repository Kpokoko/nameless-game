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
    private Container hitboxContainer = null;
    public ConstructorScene()
    {
        var zoom = Globals.Camera.Zoom;
        Name = UIScenes.ConstructorScene;

        var spawnContainer = new Container(new Vector2(1200, 300), (int)(300 ), (int)(400 ), Alignment.Center, FlexDirection.Vertical, Vector2.Zero);

        var button1 = new Button(Vector2.Zero, 280 , 50 , "InventoryBlock", ButtonActivationProperty.Switch);
        var button2 = new Button(Vector2.Zero, 280 , 50 , "EditorBlock", ButtonActivationProperty.Switch);
        var button3 = new Button(Vector2.Zero, 280 , 50 , "Block", ButtonActivationProperty.Switch);
        var button4 = new Button(Vector2.Zero, 280 , 50 , "Platform", ButtonActivationProperty.Switch);
        var button5 = new Button(Vector2.Zero, 280 , 50 , "MovingPlatform", ButtonActivationProperty.Switch);
        var button6 = new Button(Vector2.Zero, 280 , 50 , "Hitbox", ButtonActivationProperty.Switch);

        spawnContainer.AddElements(button1,button2,button3,button4,button5,button6);
        AddElements(spawnContainer);


        //var button1 = new Button(new Vector2(1700, 150), 280, 50, "InventoryBlock", ButtonActivationProperty.Switch);
        //var button2 = new Button(new Vector2(1700, 250), 280, 50, "EditorBlock", ButtonActivationProperty.Switch);
        //var button3 = new Button(new Vector2(1700, 350), 280, 50, "Block", ButtonActivationProperty.Switch);

        button1.OnClickEvent += () => { spawnContainer.SwitchButtons(button1); DespawnHitboxContainer(); };
        button2.OnClickEvent += () => { spawnContainer.SwitchButtons(button2); DespawnHitboxContainer(); };
        button3.OnClickEvent += () => { spawnContainer.SwitchButtons(button3); DespawnHitboxContainer(); };
        button4.OnClickEvent += () => { spawnContainer.SwitchButtons(button4); DespawnHitboxContainer(); };
        button5.OnClickEvent += () => { spawnContainer.SwitchButtons(button5); DespawnHitboxContainer(); };
        button6.OnClickEvent += () => { spawnContainer.SwitchButtons(button6); SpawnHitboxContainer(); };

        button1.OnClickEvent += () => Globals.Constructor.SelectedEntity = EntityTypeEnum.InventoryBlock;
        button2.OnClickEvent += () => Globals.Constructor.SelectedEntity = EntityTypeEnum.EditorBlock;
        button3.OnClickEvent += () => Globals.Constructor.SelectedEntity = EntityTypeEnum.Block;
        button4.OnClickEvent += () => Globals.Constructor.SelectedEntity = EntityTypeEnum.Platform;
        button5.OnClickEvent += () => Globals.Constructor.SelectedEntity = EntityTypeEnum.MovingPlatform;
        button6.OnClickEvent += () => Globals.Constructor.SelectedEntity = EntityTypeEnum.HitboxTrigger;

        button1.SetKeyboardKey(Keys.D1);
        button2.SetKeyboardKey(Keys.D2);
        button3.SetKeyboardKey(Keys.D3);
        button4.SetKeyboardKey(Keys.D4);
        button5.SetKeyboardKey(Keys.D5);
        button6.SetKeyboardKey(Keys.D6);

    }

    private void SpawnHitboxContainer()
    {
        hitboxContainer = new Container(new Vector2(1200, 650), (int)(300), (int)(240), Alignment.Center, FlexDirection.Vertical, Vector2.Zero);

        var button5 = new Button(Vector2.Zero, 240, 40, "SwitchScene", ButtonActivationProperty.Switch);
        var button6 = new Button(Vector2.Zero, 240, 40, "DamagePlayer", ButtonActivationProperty.Switch);
        var button7 = new Button(Vector2.Zero, 240, 40, "Disposable", ButtonActivationProperty.Switch);

        hitboxContainer.AddElements(button5, button6, button7);

        AddElements(hitboxContainer);

        button5.OnClickEvent += () => { hitboxContainer.SwitchButtons(button5); };
        button6.OnClickEvent += () => { hitboxContainer.SwitchButtons(button6); };
        button7.OnClickEvent += () => { hitboxContainer.SwitchButtons(button7); };

        button5.OnClickEvent += () => Globals.Constructor.SelectedEntityProperty = TriggerType.SwitchScene;
        button6.OnClickEvent += () => Globals.Constructor.SelectedEntityProperty = TriggerType.DamagePlayer;
        button7.OnClickEvent += () => Globals.Constructor.SelectedEntityProperty = TriggerType.Disposable;
    }

    private void DespawnHitboxContainer()
    {
        RemoveElements(hitboxContainer);
    }
}