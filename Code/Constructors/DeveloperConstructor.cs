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
using nameless.Collisions;
using nameless.Entitiy;

namespace nameless.Code.Constructors
{
    public class DeveloperConstructor : Constructor
    {
        protected override void SpawnBlock(Vector2 mouseTilePos)
        {
            base.SpawnBlock(mouseTilePos);
            switch (SelectedEntity)
            {
                case EntityTypeEnum.EditorBlock:
                    _entities.Add(new EditorBlock((int)mouseTilePos.X, (int)mouseTilePos.Y));
                    break;
                case EntityTypeEnum.Block:
                    _entities.Add(new Block((int)mouseTilePos.X, (int)mouseTilePos.Y));
                    break;
                case EntityTypeEnum.HitboxTrigger:
                    var pivot = new Pivot((int)mouseTilePos.X, (int)mouseTilePos.Y);
                    var b = HitboxTrigger.CreateHitboxTrigger(TriggerType.SwitchScene, pivot);
                    _entities.Add(pivot);
                    break;
                default:
                    break;
            }
        }
    }
}
