﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using nameless.Entity;
using nameless.Graphics;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI.Scenes;

public class ConstructorScene : UIScene
{
    private static Vector2 _spawnContainerPos = new Vector2(1200, 300);
    private static Vector2 _hitboxContainerPos = new Vector2(1200, 700);

    private Container _hitboxContainer = null;


    public ConstructorScene()
    {
        var zoom = Globals.Camera.Zoom;
        Name = UIScenes.ConstructorScene;

        var spawnContainer = new Container(_spawnContainerPos, (300 ), (600 ), FlexDirection.Vertical, Vector2.Zero);
        spawnContainer.OnDrag += () => _spawnContainerPos = spawnContainer.RelativePosition;

        var button1 = new ConstructorButton(Vector2.Zero, 280, 50, EntityTypeEnum.InventoryBlock);
        //button1.SetSprite(new SpriteBox(Vector2.Zero,new Sprite(Globals.SpriteSheet, 0, 0, 60, 60),60,60));
        //button1.Elements[1].Offset += new Vector2(0,50);
        var button2 = new ConstructorButton(Vector2.Zero, 280, 50, EntityTypeEnum.EditorBlock);
        var button3 = new ConstructorButton(Vector2.Zero, 280, 50, EntityTypeEnum.Block);
        var button4 = new ConstructorButton(Vector2.Zero, 280, 50, EntityTypeEnum.Platform);
        var button5 = new ConstructorButton(Vector2.Zero, 280, 50, EntityTypeEnum.MovingPlatform);
        var button6 = new ConstructorButton(Vector2.Zero, 280, 50, EntityTypeEnum.Pivot);
        var button7 = new ConstructorButton(Vector2.Zero, 280, 50, EntityTypeEnum.StickyBlock);
        var button8 = new ConstructorButton(Vector2.Zero, 280, 50, EntityTypeEnum.FragileBlock);
        var button9 = new ConstructorButton(Vector2.Zero, 280, 50, EntityTypeEnum.TemporaryBlock);
        spawnContainer.AddElements(button1,button2,button3,button4,button5,button6,button7,button8,button9);
        AddElements(spawnContainer);



        foreach (var b in spawnContainer.Elements)
        {
            var button = (ConstructorButton)b;
            if (button.EntityTypeSelect != EntityTypeEnum.Pivot)
                button.OnClickEvent += () => { spawnContainer.SwitchButtons(button); DespawnHitboxContainer(); };
        }
        button6.OnClickEvent += () => { spawnContainer.SwitchButtons(button6); SpawnHitboxContainer(); };


        button1.SetKeyboardKey(Keys.D1);
        button2.SetKeyboardKey(Keys.D2);
        button3.SetKeyboardKey(Keys.D3);
        button4.SetKeyboardKey(Keys.D4);
        button5.SetKeyboardKey(Keys.D5);
        button6.SetKeyboardKey(Keys.D6);
        button7.SetKeyboardKey(Keys.D7);
        button8.SetKeyboardKey(Keys.D8);
        button9.SetKeyboardKey(Keys.D9);

    }

    private void SpawnHitboxContainer()
    {
        _hitboxContainer = new Container(_hitboxContainerPos, (int)(300), (int)(240), FlexDirection.Vertical, Vector2.Zero);
        _hitboxContainer.OnDrag += () => _hitboxContainerPos = _hitboxContainer.RelativePosition;

        var button5 = new Button(Vector2.Zero, 240, 40, "SwitchScene", ButtonActivationProperty.Switch);
        var button6 = new Button(Vector2.Zero, 240, 40, "DamagePlayer", ButtonActivationProperty.Switch);
        var button7 = new Button(Vector2.Zero, 240, 40, "Disposable", ButtonActivationProperty.Switch);
        var button8 = new Button(Vector2.Zero, 240, 40, "Saver", ButtonActivationProperty.Switch);

<<<<<<< HEAD
        _hitboxContainer.AddElements(button5, button6, button7);
=======
        hitboxContainer.AddElements(button5, button6, button7, button8);
>>>>>>> ебаная-хуйня

        AddElements(_hitboxContainer);

        foreach (var b in _hitboxContainer.Elements)
        {
            var button = (Button)b;
            button.OnClickEvent += () => { _hitboxContainer.SwitchButtons(button); };
        }

        button5.OnClickEvent += () => Globals.Constructor.SelectedEntityProperty = TriggerType.SwitchScene;
        button6.OnClickEvent += () => Globals.Constructor.SelectedEntityProperty = TriggerType.DamagePlayer;
        button7.OnClickEvent += () => Globals.Constructor.SelectedEntityProperty = TriggerType.Disposable;
        button8.OnClickEvent += () => Globals.Constructor.SelectedEntityProperty = TriggerType.Saver;
    }

    private void DespawnHitboxContainer()
    {
        RemoveElements(_hitboxContainer);
    }
}