using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Timers;
using nameless.Entity;
using nameless.Interfaces;
using nameless.Entities.Blocks;
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
        public bool ConstructorMode {  get; set; } = true;

        public Scene(string sceneName, string dir)
        {
            _entities = SceneLoader<IEntity>.LoadScene(sceneName, dir);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in _entities)
            {
                //Constructor mode update
                if (ConstructorMode && entity is IConstructable)
                    (entity as IConstructable).UpdateConstructor(gameTime);

                switch (entity)
                {
                    case PlayerModel:
                        entity.Update(gameTime);
                        continue;
                    case InventoryBlock:
                        entity.Update(gameTime);
                        continue;
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
