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
        private Vector2? _startMouseTilePos;
        private Vector2? _previousDrawTilePos;
        private bool _blocksDeleted = false;
        private bool _fill = false;
        public override void Update(GameTime gameTime)
        {

            var mouseTilePos = Storage.IsInBounds(MouseInputController.MouseTilePos) ? MouseInputController.MouseTilePos : _prevMouseTilePos;
            var entityUnderMouse = _storage[(int)mouseTilePos.X, (int)mouseTilePos.Y, Layer];

            _groupInteraction = IsDeveloperGroupInteraction();
            if (!_groupInteraction)
                ResetDrawing();

            if ((MouseInputController.LeftButton.IsPressed && IsDrawingRectangle()))
                DrawRectangle(mouseTilePos);

            if ((MouseInputController.RightButton.IsPressed && IsDrawingRectangle()))
                DeleteRectangle(mouseTilePos);

            base.Update(gameTime);
        }

        private void ResetDrawing()
        {
            _startMouseTilePos = null;
            _previousDrawTilePos = null;
            _blocksDeleted = false ;
        }

        private void DeleteRectangle(Vector2 mouseTilePos) => DrawRectangle(mouseTilePos, false);

        private void DrawRectangle(Vector2 mouseTilePos, bool spawn = true)
        {
            if (_startMouseTilePos == null)
                _startMouseTilePos = mouseTilePos;
            if (_previousDrawTilePos != null && mouseTilePos == _previousDrawTilePos)
                return;
            if ((spawn && _previousDrawTilePos != mouseTilePos && _previousDrawTilePos != null)
                || (!spawn && _blocksDeleted))
            {
                Undo();
                ClearHistoryStackAction(_redoHistory);
                _blocksDeleted = false;
            }
            _previousDrawTilePos = mouseTilePos;

            var startMousePos = (Vector2)_startMouseTilePos;
            var minX = (int)Math.Min(startMousePos.X, mouseTilePos.X);
            var maxX = (int)Math.Max(startMousePos.X, mouseTilePos.X);
            var minY = (int)Math.Min(startMousePos.Y, mouseTilePos.Y);
            var maxY = (int)Math.Max(startMousePos.Y, mouseTilePos.Y);

            _groupInteraction = true;
            for (var x = minX; x <= maxX; x++)
                for (var y = minY; y <= maxY; y++)
                {
                    if (!(y == minY || y == maxY || x == minX || x == maxX) && !_fill)
                        continue;
                    var tilePos = new Vector2(x, y);
                    var entity = _storage[tilePos, Layer];
                    if (spawn && entity == null)
                    {
                        SpawnBlock(tilePos);
                    }
                    else if (!spawn && entity != null)
                    {
                        DeleteBlock(entity);
                        _blocksDeleted = true;
                    }
                }

        }

        private bool IsDeveloperGroupInteraction()
        {
            if ((IsDrawingRectangle() || IsDrawingLine()) && MouseInputController.IsJustPressed)
            {
                _history.Push(new HistoryEventInfo(null, HistoryEventType.Separator));
            }

            if (Globals.KeyboardInputController.KeyboardState.CapsLock)
                _fill = true;
            else
                _fill = false;

            return (IsDrawingRectangle() || IsDrawingLine()) && MouseInputController.IsPressed;
        }

        private bool IsDrawingRectangle() => Globals.KeyboardInputController.IsPressed(Keys.LeftControl);
        private bool IsDrawingLine() => Globals.KeyboardInputController.IsPressed(Keys.LeftShift);

        public override void SpawnBlock(Vector2 tilePos, bool calledFromHistory = false)
        {
            base.SpawnBlock(tilePos, calledFromHistory);
            switch (SelectedEntity)
            {
                case EntityTypeEnum.EditorBlock:
                    _storage.AddEntity(new EditorBlock(tilePos), tilePos, Layer);
                    break;
                case EntityTypeEnum.Block:
                    _storage.AddEntity(new Block(tilePos), tilePos, Layer);
                    break;
                case EntityTypeEnum.Platform:
                    _storage.AddEntity(new Platform(tilePos), tilePos, Layer);
                    break;
                case EntityTypeEnum.MovingPlatform:
                    var mp = new MovingPlatform((int)tilePos.X, (int)tilePos.Y, Vector2.One, 1);
                    EditMovingPlatform(mp);
                    _storage.AddEntity(mp, tilePos, Layer);
                    break;
                case EntityTypeEnum.Pivot:
                    if (SelectedEntityProperty is not TriggerType) return;
                    var type = (TriggerType)SelectedEntityProperty;
                    if (type is TriggerType.None) return;

<<<<<<< HEAD
                    var pivot = new Pivot((int)mouseTilePos.X, (int)mouseTilePos.Y);
                    var b = HitboxTrigger.CreateHitboxTrigger(type, pivot, Globals.SceneManager.GetEntities());
                    _entities.Add(pivot);
=======
                    var pivot = new Pivot((int)tilePos.X, (int)tilePos.Y);
                    var b = HitboxTrigger.CreateHitboxTrigger(type, pivot,Globals.SceneManager.GetEntities());
                    _storage.AddEntity(pivot, tilePos, Layer);
>>>>>>> Constructor-features
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
