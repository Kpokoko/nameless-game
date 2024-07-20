using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using nameless.Code.SceneManager;
using nameless.Controls;
using nameless.Entity;
using nameless.Interfaces;
using nameless.UI.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.Constructors
{
    public class DeveloperConstructor : Constructor
    {
        protected override void SpawnBlock(Vector2 mouseTilePos)
        {
            base.SpawnBlock(mouseTilePos);
            switch (SelectedEntity)
            {
                case EntityType.EditorBlock:
                    _entities.Add(new EditorBlock((int)mouseTilePos.X, (int)mouseTilePos.Y));
                    break;
                case EntityType.Block:
                    _entities.Add(new Block((int)mouseTilePos.X, (int)mouseTilePos.Y));
                    break;
                default:
                    break;
            }
        }
    }
}
