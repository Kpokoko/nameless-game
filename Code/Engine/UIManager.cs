using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using nameless.Entity;
using nameless.GameObjects;
using nameless.UI;
using nameless.UI.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace nameless.Engine;
public class UIManager
{
    private bool _mapIsActive = false;

    public List<Button> Buttons = new List<Button>();
    public List<Label> Labels = new List<Label>();
    public List<Container> Containers = new List<Container>();
    public List<Minimap> Minimaps = new List<Minimap>();
    public List<Controller> Controllers = new List<Controller>();
    public List<BarController> BarControllers = new List<BarController>();
    public List<CircleController> CircleControllers = new List<CircleController>();
    public List<SpriteBox> SpriteBoxes = new List<SpriteBox>();

    public SpriteFont Font = ResourceManager.Font;

    public List<Container> Popups = new List<Container>();

    public List<Block> Selected = new();
    public Rectangle SelectionArea = new Rectangle();
    public CRectangle PlaceArea = new CRectangle();

    public List<UIElement> ToRemove = new List<UIElement>();

    public Dictionary<Keys, Button> KeyboardButtons = new();

    public Dictionary<UIScenes, UIScene> CurrentUIScenes = new();

    public void Update(GameTime gameTime)
    {
        for (var i = 0; i < Buttons.Count; i++)
            Buttons[i].Update(gameTime);
        for (var i = 0; i < KeyboardButtons.Count; i++)
        {
            if (Keyboard.GetState().IsKeyDown(KeyboardButtons.Keys.ToArray()[i]))
                KeyboardButtons.Values.ToArray()[i].Activate();
        }
        for (var i = 0; i < Containers.Count; i++)
            Containers[i].Update(gameTime);
        //for (var i = 0; i < CircleControllers.Count; i++)
        //    CircleControllers[i].Update();
        for (var i = 0; i < Controllers.Count; i++)
            Controllers[i].Update();
        if (_mapIsActive)
        {
            for (var i = 0; i < Minimaps.Count; i++)
            {
                Minimaps[i].Update();
                if (Minimaps[i].Offset != Globals.SceneManager.MinimapOffset)
                    Minimaps[i].Offset = Globals.SceneManager.MinimapOffset;
            }
        }

        for (var i = 0;i< ToRemove.Count; i++)
            ToRemove[i].Remove();
        ToRemove.Clear();
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < Containers.Count; i++)
            Containers[i].Draw(spriteBatch);
        if (_mapIsActive)
            for (var i = 0; i < Minimaps.Count; i++)
                Minimaps[i].Draw(spriteBatch);
        for (var i = 0; i < Buttons.Count; i++)
            Buttons[i].Draw(spriteBatch);
        for (var i = 0; i < Labels.Count; i++)
            Labels[i].Draw(spriteBatch);
        //for (var i = 0; i < CircleControllers.Count; i++)
        //    CircleControllers[i].Draw(spriteBatch);
        for (var i = 0; i < Controllers.Count; i++)
            Controllers[i].Draw(spriteBatch);
        for (var i = 0; i < SpriteBoxes.Count; i++)
            SpriteBoxes[i].Draw(spriteBatch);

        for (var i = 0; i < Selected.Count; i++)
        {
            if (Selected[i].Colliders.colliders.Any())
                spriteBatch.FillRectangle((RectangleF)Selected[i].Colliders[0].Bounds, Color.DeepSkyBlue * 0.5f);
        }
        spriteBatch.FillRectangle(SelectionArea, Color.DeepSkyBlue * 0.3f);
        spriteBatch.FillRectangle(PlaceArea.RectangleF, Color.Lime * 0.3f);
    }

    public void SetScene(UIScenes scene)
    {
        switch (scene)
        {
            case UIScenes.ConstructorScene:
                CurrentUIScenes[scene] = new ConstructorScene();
                break;
             case UIScenes.InventoryScene:
                CurrentUIScenes[scene] = new InventoryScene();
                break;
           default:
                throw new ArgumentException("Scene not implemented");
        }
    }
    public void RemoveScene(UIScenes scene)
    {
        if (CurrentUIScenes.ContainsKey(scene))
        {
            CurrentUIScenes[scene].Clear();
            CurrentUIScenes.Remove(scene);
        }
    }

    public void PopupMessage(string message)
    {
        foreach (var c in Popups)
            c.Remove();
        Popups.Clear();
        var label = new Label(new Vector2(), message);
        var container = new Container(new Vector2(100,40), (int)label.Bounds.Width,(int)label.Bounds.Height,FlexDirection.Horizontal, new Vector2(10,10));
        container.Alignment = Alignment.CenterLeft;
        container.AddElements(label);
        Popups.Add(container);
        TimerTrigger.DelayEvent(3000, () => 
        { 
            container.Remove(); 
        });
    }

    public void Clear()
    {
        Buttons.Clear();
        Labels.Clear();
        KeyboardButtons.Clear();
        Containers.Clear();
    }
    
    public void SwitchMinimap()
    {
        if (_mapIsActive)
        {
            foreach (var map in Minimaps)
                map.HideLabel();

            HideMap();
        }
        else
            ShowMap();
    }

    public void ShowMap() => _mapIsActive = true;

    public void HideMap() => _mapIsActive = false;
}