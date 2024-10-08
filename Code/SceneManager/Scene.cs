﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Timers;
using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using nameless.Controls;
using Microsoft.Xna.Framework.Content;
using nameless.Collisions;
using System.IO;

namespace nameless.Code.SceneManager
{
    public class Scene
    {
        public List<IEntity> Entities;
        public Storage Storage;

        public List<IEntity> ToRemove = new();

        public string Name { get; }
        private ContentManager _content;

        public Scene(string sceneName)
        {
            Entities = SceneLoader.LoadScene(sceneName);
            //SceneLoader.SaveScene();
            Storage = new(Entities);
            Name = sceneName;
        }

        public static Storage GetSceneStorage(string sceneName)
        {
            var entities = SceneLoader.LoadScene(sceneName, true);
            return new Storage(entities);
        }

        public void Update(GameTime gameTime)
        {
            if (Globals.KeyboardInputController.IsJustPressed(Keys.C) && Globals.IsDeveloperModeEnabled)
            {
                Globals.CopyFiles(Path.Combine("Layout","Levels", Name + ".xml"), Path.Combine("BaseLayout","Levels", Name + ".xml"), false);
                Globals.UIManager.PopupMessage("Initial level state updated");
            
            }
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
                        continue;
                    case MovingBlock:
                        if (!Globals.IsConstructorModeEnabled)
                            entity.Update(gameTime);
                        continue;
                    case RayCaster:
                        if (!Globals.IsConstructorModeEnabled)
                            entity.Update(gameTime);
                        continue;
                }
                //Обновляем тут движущиеся объекты на сцене
            }

            foreach (var entity in ToRemove)
            {
                entity.Remove();
                Entities.Remove(entity);
            }
            ToRemove.Clear();

            //Storage = new Storage(Entities);
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
