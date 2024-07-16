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
            var pos = MouseInputController.MouseTilePos;
            if (Globals.IsConstructorModeEnabled) 
            { 
                if (MouseInputController.IsJustClickedLeft && Storage.Entities[(int)pos.X, (int)pos.Y] is IConstructable)
                {
                    (Storage.Entities[(int)pos.X, (int)pos.Y] as IConstructable).IsHolding = true;
                }

                if (MouseInputController.IsJustClickedLeft && Storage.Entities[(int)pos.X, (int)pos.Y] == null)
                {
                    _entities.Add(new InventoryBlock((int)pos.X, (int)pos.Y));
                }

                if (MouseInputController.IsJustClickedRight && Storage.Entities[(int)pos.X, (int)pos.Y] is IConstructable)
                {
                    _entities.Remove(Storage.Entities[(int)pos.X, (int)pos.Y]);
                    (Storage.Entities[(int)pos.X, (int)pos.Y] as Block).colliders.RemoveAll();
                }
            }

            foreach (var entity in _entities)
            {
                //Constructor mode update
                if (Globals.IsConstructorModeEnabled && entity is IConstructable)
                {
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

            Storage = new Storage(_entities);
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
