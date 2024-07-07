using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Timers;
using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.SceneManager
{
    public class Scene
    {
        public List<IEntity> _entities;

        public Scene(string sceneName, string dir)
        {
            _entities = SceneLoader<IEntity>.LoadScene(sceneName, dir);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in _entities)
            {
                switch (entity)
                {
                    case PlayerModel:
                        entity.Update(gameTime);
                        return;
                }
                //Обновляем тут движущиеся объекты на сцене
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var entity in _entities)
                switch (entity)
                {
                    case PlayerModel:
                        entity.Draw(spriteBatch, gameTime);
                        return;
                }
        }
    }
}
