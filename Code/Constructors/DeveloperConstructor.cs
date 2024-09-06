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
        private bool _fill = false;
        public override void Update(GameTime gameTime)
        {

            var mouseTilePos = Storage.IsInBounds(MouseInputController.MouseTilePos) ? MouseInputController.MouseTilePos : _prevMouseTilePos;
            var entityUnderMouse = _storage[(int)mouseTilePos.X, (int)mouseTilePos.Y, Layer];

            _groupInteraction = IsDeveloperGroupInteraction();

            if (MouseInputController.OnUIElement)
                return;


            if (SelectedEntityType.IsSubclassOf(typeof(SlimBlock)))
            {

            }

            else if ((MouseInputController.LeftButton.IsPressed && IsDrawingRectangle()))
                DrawRectangle(mouseTilePos);

            else if ((MouseInputController.RightButton.IsPressed && IsDrawingRectangle()))
                DeleteRectangle(mouseTilePos);

            else if (MouseInputController.LeftButton.IsPressed && IsDrawingLine())
                DrawLine(mouseTilePos);

            else if ((MouseInputController.RightButton.IsPressed && IsDrawingLine()))
                DeleteLine(mouseTilePos);

            base.Update(gameTime);
        }

        private void DeleteLine(Vector2 mouseTilePos) => DrawLine(mouseTilePos, false);
        private void DrawLine(Vector2 mouseTilePos, bool spawn = true)
        {
            if (!IsMouseMoved(mouseTilePos, spawn))
                return;

            var startMousePos = (Vector2)_startMouseTilePos;
            var x1 = (int)startMousePos.X;
            var x2 = (int)mouseTilePos.X;
            var y1 = (int)startMousePos.Y;
            var y2 = (int)mouseTilePos.Y;
            int xDiff = Math.Abs(x1 - x2);
            int yDiff = Math.Abs(y1 - y2);

            double deltaX, deltaY;
            int num;
            if (xDiff > yDiff)
            {
                //deltaY = 0;
                //if ((x2 - x1) != 0)
                //    deltaY = (y2 - y1) / (x2 - x1);
                //deltaX = 1;
                num = xDiff;
            }
            else
            {
                //deltaX = 0;
                //if ((y2 - y1) != 0)
                //    deltaX = (x2 - x1) / (y2 - y1);
                //deltaY = 1;
                num = yDiff;
            }

            _groupInteraction = true;

            var entity = _storage[startMousePos, Layer];
            if (spawn && entity == null)
            {
                SpawnBlock(startMousePos);
                _blocksSpawned = true;
            }
            else if (!spawn && entity != null)
            {
                DeleteBlock(entity);
                _blocksDeleted = true;
            }

            for (int i = 1; i <= num; i++)
            {
                var mouseDelta = (mouseTilePos - startMousePos) / num * (i);
                var tilePos = startMousePos + new Vector2((float)Math.Round(mouseDelta.X), (float)Math.Round(mouseDelta.Y));
                entity = _storage[tilePos, Layer];
                if (spawn && entity == null)
                {
                    SpawnBlock(tilePos);
                    _blocksSpawned = true;
                }
                else if (!spawn && entity != null)
                {
                    DeleteBlock(entity);
                    _blocksDeleted = true;
                }
            }

        }

        private void DeleteRectangle(Vector2 mouseTilePos) => DrawRectangle(mouseTilePos, false);

        private void DrawRectangle(Vector2 mouseTilePos, bool spawn = true)
        {
            if (!IsMouseMoved(mouseTilePos , spawn))
                return;

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
                        _blocksSpawned = true;
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
            _isDeveloperAction = false;
            if ((IsDrawingRectangle() || IsDrawingLine()) && MouseInputController.IsPressed)//EVERY FRAME&!&!&!&
            {
                _isDeveloperAction = true;
                if (MouseInputController.IsJustPressed)
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
                case EntityTypeEnum.MovingBlock:
                    var mp = new MovingBlock((int)tilePos.X, (int)tilePos.Y, Vector2.UnitX, 5);
                    EditMovingPlatform(mp);
                    _storage.AddEntity(mp, tilePos, Layer);
                    break;
                case EntityTypeEnum.Pivot:
                    if (SelectedEntityProperty is not TriggerType) return;
                    var type = (TriggerType)SelectedEntityProperty;
                    if (type is TriggerType.None) return;
                    var pivot = new Pivot((int)tilePos.X, (int)tilePos.Y);
                    var b = HitboxTrigger.CreateHitboxTrigger(type, pivot,Globals.SceneManager.GetEntities());
                    _storage.AddEntity(pivot, tilePos, Layer);
                    break;
                case EntityTypeEnum.RayCaster:
                    var caster = new RayCaster((int)tilePos.X, (int)tilePos.Y, Vector2.UnitX);
                    EditRayCaster(caster);
                    _storage.AddEntity(caster, tilePos, Layer);
                    break;
                case EntityTypeEnum.Spawner:
                    var spawner = new Spawner((int)tilePos.X, (int)tilePos.Y, Vector2.UnitX, 5);
                    EditSpawner(spawner);
                    _storage.AddEntity(spawner, tilePos, Layer);
                    break;
                case EntityTypeEnum.BlinkingBlock:
                    var blink = new BlinkingBlock((int)tilePos.X, (int)tilePos.Y);
                    _storage.AddEntity(blink, tilePos, Layer);
                    break;
                default:
                    break;
            }
        }

        private void EditMovingPlatform(MovingBlock platform)
        {
            var circle = new CircleController(platform.Position);
            circle.SetPossibleValues(0.25f, 0.5f, 0.75f, 1f, 2f);
            circle.Length = platform.Speed / 10; circle.Direction = platform.Direction;
            circle.OnValueSet += () =>
                platform.SetMovement(circle.Vector, circle.Vector.Length() * 10);
        }

        private void EditRayCaster(RayCaster caster)
        {
            var circle = new CircleController(caster.Position);
            circle.Direction = caster.Direction;
            circle.OnValueSet += () =>
                caster.SetMovement(circle.Vector);
        }

        private void EditSpawner(Spawner spawner)
        {
            var circle = new CircleController(spawner.Position);
            circle.SetPossibleValues(0.25f, 0.5f, 0.75f, 1f, 2f);
            circle.Length = spawner.Speed / 10; circle.Direction = spawner.Direction;
            circle.OnValueSet += () =>
                spawner.SetMovement(circle.Vector, circle.Length * 10);

            var bar = new BarController(spawner.Position + new Vector2(0, 80));
            bar.SetPossibleValues(0.25f, 0.5f, 0.75f, 1f, 2f, 4f);
            bar.Length = (float)spawner.Timer.DelayTime / 1000;
            bar.OnValueSet += () =>
                spawner.SetInterval(bar.Length * 1000);

            var circle2 = new CircleController(spawner.Position + new Vector2(-57, 67), 20);
            circle2.SetPossibleValues(0, 1f);
            circle2.Direction = spawner.SpawnOffset;
            circle2.OnValueSet += () =>
                spawner.SetSpawnOffset(circle2.Vector);

            List<Controller> group = new List<Controller>() { bar, circle, circle2};
            bar.ControllerGroup = group;
            circle.ControllerGroup = group;
            circle2.ControllerGroup = group;
        }

    }
}
