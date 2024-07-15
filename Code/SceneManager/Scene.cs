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
using Microsoft.Xna.Framework.Input;
using nameless.Controls;

namespace nameless.Code.SceneManager
{
    public class Scene
    {
        public List<IEntity> _entities;
        public Storage Storage;
        //public bool ConstructorMode { get; private set; } = true;

        public Scene(string sceneName, string dir)
        {
            _entities = SceneLoader.LoadScene(sceneName, dir);
            Storage = new Storage(_entities);
        }

        private bool _prevKeyPressed;

        public void Update(GameTime gameTime)
        {
            foreach (var entity in _entities)
            {
                //Constructor mode update
                if (Globals.IsConstructorModeEnabled && entity is IConstructable)
                {
                    var pos = MouseInputController.MouseTilePos;
                    if (Storage.Entities[(int)pos.X, (int)pos.Y] == null)
                        (entity as IConstructable).UpdateConstructor(gameTime);
                }

                switch (entity)
                {
                    case PlayerModel:
                        if (!Globals.IsConstructorModeEnabled)
                            entity.Update(gameTime);
                        else if (!_prevKeyPressed && Keyboard.GetState().IsKeyDown(Keys.E))
                            Globals.IsConstructorModeEnabled = false;
                        _prevKeyPressed = Keyboard.GetState().IsKeyDown(Keys.E);
                        continue;
                    //case InventoryBlock:
                    //    if (Globals.IsConstructorModeEnabled)
                    //        entity.Update(gameTime);
                    //    continue;
                }
                //_prevKeyPressed = Keyboard.GetState().IsKeyDown(Keys.E);
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
