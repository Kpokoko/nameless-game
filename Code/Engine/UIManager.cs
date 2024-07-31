using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
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
    public List<CircleController> CircleControllers = new List<CircleController>();
    public Dictionary<Keys, Button> KeyboardButtons = new();
    public Dictionary<UIScenes, UIScene> CurrentUIScenes = new();
    public SpriteFont Font;
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
        for (var i = 0; i < CircleControllers.Count; i++)
            CircleControllers[i].Update();
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
        for (var i = 0; i < CircleControllers.Count; i++)
            CircleControllers[i].Draw(spriteBatch);
    }

    public void SetScene(UIScenes scene)
    {
        switch (scene)
        {
            case UIScenes.ConstructorScene:
                CurrentUIScenes[scene] = new ConstructorScene();
                break;
            default:
                throw new ArgumentException("Scene not implemented");
        }
    }
    public void RemoveScene(UIScenes scene)
    {
        CurrentUIScenes[scene].Clear();
        CurrentUIScenes.Remove(scene);
    }

    public void Clear()
    {
        Buttons.Clear();
        Labels.Clear();
        KeyboardButtons.Clear();
    }

    public void ShowMap() => _mapIsActive = true;

    public void HideMap() => _mapIsActive = false;
}