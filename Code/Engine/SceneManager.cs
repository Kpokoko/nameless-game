using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nameless.Code.SceneManager;
using nameless.Collisions;
using nameless.Engine;
using nameless.Entity;
using nameless.GameObjects;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Engine;

public class SceneManager
{
    private Scene _currentScene;

    public void LoadScene(string sceneName)
    {
        _currentScene = new Scene(sceneName);
        var trigger = new HitboxTrigger(new Pivot(17, 12), 80, 80, ReactOnProperty.ReactOnEntityType, Collisions.SignalProperty.OnceOnEveryContact);
        trigger.SetTriggerEntityTypes(typeof(PlayerModel));
        trigger.Color = Color.SkyBlue;
        trigger.OnCollisionEvent += () => GetPlayer().Position = new Vector2(GetPlayer().Position.X, GetPlayer().Position.Y - 60);
        var entities = _currentScene.Entities;
        foreach (var block in entities.Where(item => item is Block))
        {
            //if (block is EditorBlock) continue;
            var colliderBlock = block as ICollider;
            var trigger2 = new HitboxTrigger(colliderBlock, 70, 70, ReactOnProperty.ReactOnEntityType, Collisions.SignalProperty.OnceOnEveryContact);
            trigger2.SetTriggerEntityTypes(typeof(PlayerModel));
            colliderBlock.colliders.Add(trigger2);
            var trueColor = colliderBlock.colliders[0].Color;
            trigger2.OnCollisionEvent += () =>
            {
                colliderBlock.colliders[0].Color = Color.Blue;
            };
            trigger2.OnCollisionExitEvent += () =>
            {
                TimerTrigger.DelayEvent(500, () => { if (!trigger2.isActivated) colliderBlock.colliders[0].Color = trueColor; });
            };
        }
    }

    public void ReloadScene()
    {
        var sceneName = _currentScene.Name;
        _currentScene = null;
        LoadScene(sceneName);
    }

    public PlayerModel GetPlayer() => _currentScene.Entities.Where(item => item is PlayerModel).First() as PlayerModel;

    public Storage GetStorage() => _currentScene.Storage;

    public List<IEntity> GetEntities() => _currentScene.Entities;

    public string GetName() => _currentScene.Name;

    public void Update(GameTime gameTime) => _currentScene.Update(gameTime);

    public void Draw(SpriteBatch spriteBatch) => _currentScene.Draw(spriteBatch);
}
