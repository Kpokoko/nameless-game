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
        public List<IEntity> Entities;
        public Storage Storage;
        //public bool ConstructorMode { get; private set; } = true;

        public Scene(string sceneName, string dir)
        {
            Entities = SceneLoader.LoadScene(sceneName, dir);
            Storage = new Storage(Entities);
            Globals.CurrentScene = this;
        }

        private bool _prevKeyPressed;

        public void Update(GameTime gameTime)
        {
            if (Globals.IsConstructorModeEnabled)
                Globals.Constructor.Update(gameTime);

            for (var i = 0; i < Entities.Count;i++)
            {
                var entity = Entities[i];

                switch (entity)
                {
                    case PlayerModel:
                        if (!Globals.IsConstructorModeEnabled)
                            entity.Update(gameTime);
                        else if (!_prevKeyPressed && Keyboard.GetState().IsKeyDown(Keys.E))
                            Globals.IsConstructorModeEnabled = false;
                        _prevKeyPressed = Keyboard.GetState().IsKeyDown(Keys.E);
                        continue;
                }
                //Обновляем тут движущиеся объекты на сцене
            }

            Storage = new Storage(Entities);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in Entities)
                switch (entity)
                {
                    case PlayerModel:
                        entity.Draw(spriteBatch);
                        return;
                }
        }
    }
}
