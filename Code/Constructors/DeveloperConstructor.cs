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
using nameless.UI;
using nameless.Tiles;
namespace nameless.Code.Constructors
{
    public class DeveloperConstructor : Constructor
    {
        public override void SpawnBlock(Vector2 mouseTilePos, bool calledFromHistory = false)
        {
            base.SpawnBlock(mouseTilePos, calledFromHistory);
            switch (SelectedEntity)
            {
                case EntityTypeEnum.EditorBlock:
                    _entities.Add(new EditorBlock((int)mouseTilePos.X, (int)mouseTilePos.Y));
                    break;
                case EntityTypeEnum.Block:
                    _entities.Add(new Block((int)mouseTilePos.X, (int)mouseTilePos.Y));
                    break;
                case EntityTypeEnum.Platform:
                    _entities.Add(new Platform((int)mouseTilePos.X, (int)mouseTilePos.Y));
                    break;
                case EntityTypeEnum.MovingPlatform:
                    var mp = new MovingPlatform((int)mouseTilePos.X, (int)mouseTilePos.Y, Vector2.One, 1);
                    EditMovingPlatform(mp);
                    _entities.Add(mp);
                    break;
                case EntityTypeEnum.Pivot:
                    if (SelectedEntityProperty is not TriggerType) return;
                    var type = (TriggerType)SelectedEntityProperty;
                    if (type is TriggerType.None) return;

                    var pivot = new Pivot((int)mouseTilePos.X, (int)mouseTilePos.Y);
                    var b = HitboxTrigger.CreateHitboxTrigger(type, pivot, Globals.SceneManager.GetEntities());
                    _entities.Add(pivot);
                    break;
                default:
                    break;
            }
        }

        private void EditMovingPlatform(MovingPlatform platform)
        {
            var circle = new CircleController(platform.Position);
            circle.SetPossibleValues(0.25f, 0.5f, 0.75f, 1f, 1.5f, 2f);
            circle.OnDirectionSet += () =>
                platform.SetMovement(circle.Vector, circle.Vector.Length() * 10);
        }
    }
}
