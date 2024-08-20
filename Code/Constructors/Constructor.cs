using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Code.SceneManager;
using nameless.Controls;
using nameless.Entity;
using nameless.Interfaces;
using nameless.Serialize;
using nameless.Tiles;
using nameless.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace nameless.Code.Constructors;

public class Constructor : IGameObject
{
    public int DrawOrder => 1;
    protected Vector2 _prevMouseTilePos = Vector2.Zero;
    protected Storage _storage { get { return Globals.SceneManager.GetStorage(); } }
    protected List<IEntity> _entities { get { return Globals.SceneManager.GetEntities(); } }
    private List<IConstructable> _holdingEntities { get; set; } = new();
    public EntityTypeEnum SelectedEntity { get; set; }
    public object SelectedEntityProperty { get; set; }
    public Type SelectedEntityType { get {  return EntityType.TranslateEntityEnumAndType(SelectedEntity); } }
    public int Layer { get { return (SelectedEntity is EntityTypeEnum.Pivot) ? 1 : 0; } }

    protected Stack<HistoryEventInfo> _history = new();

    protected Stack<HistoryEventInfo> _redoHistory = new();

    protected bool _groupInteraction;
    protected bool _isDeveloperAction = false;

    protected Vector2? _startMouseTilePos;
    protected Vector2? _previousDrawingTilePos;
    protected bool _blocksDeleted = false;
    protected bool _blocksSpawned = false;
    private bool _areaSelection = false;


    public void Draw(SpriteBatch spriteBatch)
    { }

    public void SwitchMode()
    {
        Globals.IsConstructorModeEnabled = Globals.IsConstructorModeEnabled ? false : true;
        if (Globals.IsConstructorModeEnabled)
        {
            var playerPos = Globals.SceneManager.GetPlayer().Position;
            Globals.SceneManager.LoadScene(Globals.SceneManager.CurrentLocation);
            Globals.SceneManager.GetPlayer().Position = playerPos;
            Globals.UIManager.SetScene(UIScenes.ConstructorScene);
            //Globals.UIManager.ShowMap();
            //Globals.UIManager.CurrentUIScenes[UIScenes.ConstructorScene]
            //    .AddElements(new Minimap(new Vector2(1600, 300 + 220), 0, 0, _storage, Alignment.Center));
        }
        else
        {
            Globals.UIManager.RemoveScene(UIScenes.ConstructorScene);
           // Globals.UIManager.HideMap();
            var serializer = new Serializer();
            serializer.SerializeScene(Globals.SceneManager.GetName(), Globals.SceneManager.GetEntities().Select(x => x as ISerializable).ToList());
            serializer.SaveInventory(Globals.Inventory.GetInventory());

        }
        _history.Clear();
        _redoHistory.Clear();
        ReleaseBlock();
    }



    public virtual void Update(GameTime gameTime)
    {
        var mouseTilePos = Storage.IsInBounds(MouseInputController.MouseTilePos) ? MouseInputController.MouseTilePos : _prevMouseTilePos;
        var entityUnderMouse = _storage[(int)mouseTilePos.X, (int)mouseTilePos.Y, Layer];

        _groupInteraction = IsGroupInteraction() || (_groupInteraction && _isDeveloperAction);



        if (MouseInputController.OnUIElement || _isDeveloperAction)
            return;

        else if (IsSelecting())
            SelectArea(mouseTilePos);

        else if (MouseInputController.LeftButton.IsJustPressed && PossibleToInteract(entityUnderMouse))
            HoldBlock(entityUnderMouse);

        else if (MouseInputController.IsJustPressed && _holdingEntities.Any() && !_holdingEntities.Contains(entityUnderMouse))
            ReleaseBlock();

        else if (((MouseInputController.LeftButton.IsJustReleased) || MouseInputController.LeftButton.IsPressed && IsDrawing()) && entityUnderMouse == null)
            SpawnBlock(mouseTilePos);

        else if (((MouseInputController.RightButton.IsJustReleased) || MouseInputController.RightButton.IsPressed && IsDrawing()) && PossibleToInteract(entityUnderMouse))
            DeleteBlock(entityUnderMouse);

        else if (IsMoving())
            MoveBlock(mouseTilePos);

        if (!_groupInteraction)
            ResetDrawing();

        _prevMouseTilePos = mouseTilePos;
    }

    private void SelectArea(Vector2 mouseTilePos)
    {
        _areaSelection = false;
        if (!IsMouseMoved(mouseTilePos, true))
            return;
        _areaSelection = true;

        _history.Push(new HistoryEventInfo(HistoryEventType.Separator));

        var startMousePos = (Vector2)_startMouseTilePos;
        var minX = (int)Math.Min(startMousePos.X, mouseTilePos.X);
        var maxX = (int)Math.Max(startMousePos.X, mouseTilePos.X);
        var minY = (int)Math.Min(startMousePos.Y, mouseTilePos.Y);
        var maxY = (int)Math.Max(startMousePos.Y, mouseTilePos.Y);

        Globals.UIManager.SelectionArea = new Rectangle(Tile.GetTileCenter(new Vector2(minX, minY)).ToPoint(), Tile.GetTileCenter(new Vector2(maxX - minX, maxY - minY)).ToPoint());

        _groupInteraction = true;
        for (var x = minX; x <= maxX; x++)
            for (var y = minY; y <= maxY; y++)
            {
                var tilePos = new Vector2(x, y);
                var entity = _storage[tilePos, Layer];
                if (entity == null)
                    continue;
                HoldBlock(entity);
                _blocksSpawned = true;
            }

    }

    private void HoldBlock(IConstructable entity, bool calledFromHistory = false)
    {
        if (entity == null || entity.IsSelected)
            return;
        entity.IsSelected = true;
        if (!_holdingEntities.Contains(entity))
            _holdingEntities.Add(entity);
        //else
        //    return;

        if (!calledFromHistory)
        {
            var pos = (entity as TileGridEntity).TilePosition;
            var layer = (entity as TileGridEntity).Layer;
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => ReleaseBlock(_storage[pos, layer], true);
            _history.Push(newEvent);
        }
        else
        {
            var pos = (entity as TileGridEntity).TilePosition;
            var layer = (entity as TileGridEntity).Layer;
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => ReleaseBlock(_storage[pos, layer]);
            _redoHistory.Push(newEvent);
        }
    }

    private void ReleaseBlock()
    {
        _groupInteraction = true;
        foreach (var entity in _holdingEntities.ToArray())
            ReleaseBlock(entity);
    }

    private void ReleaseBlock(IConstructable entity, bool calledFromHistory = false)
    {
        if (entity == null || !entity.IsSelected)
            return;
        entity.IsSelected = false;
        _holdingEntities.Remove(entity);

        if (!calledFromHistory)
        {
            var pos = (entity as TileGridEntity).TilePosition;
            var layer = (entity as TileGridEntity).Layer;
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => HoldBlock(_storage[pos, layer], true);
            _history.Push(newEvent);
        }
        else
        {
            var pos = (entity as TileGridEntity).TilePosition;
            var layer = (entity as TileGridEntity).Layer;
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => HoldBlock(_storage[pos, layer]);
            _redoHistory.Push(newEvent);
        }
    }

    public virtual void SpawnBlock(Vector2 tilePos, bool calledFromHistory = false)
    {
        if (SelectedEntity is EntityTypeEnum.None) return;
        switch (SelectedEntity)
        {
            case EntityTypeEnum.InventoryBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    _storage.AddEntity(new InventoryBlock(tilePos), tilePos, Layer);
                }
                break;
            case EntityTypeEnum.StickyBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    _storage.AddEntity(new StickyBlock(tilePos), tilePos, Layer);
                }
                break;
            case EntityTypeEnum.FragileBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    _storage.AddEntity(new FragileBlock(tilePos), tilePos, Layer);
                }
                break;
            case EntityTypeEnum.TemporaryBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    _storage.AddEntity(new TemporaryBlock(tilePos), tilePos, Layer);
                }
                break;
            case EntityTypeEnum.DelayedDeathBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    _storage.AddEntity(new DelayedDeathBlock((int)tilePos.X, (int)tilePos.Y), tilePos, Layer);
                }
                break;
            default:
                break;
        }

        if (!calledFromHistory)
        {
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            var layer = Layer;
            newEvent.Action = () => DeleteBlock(tilePos , layer, true);
            _history.Push(newEvent);
        }
        else 
        {
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            var layer = Layer;
            newEvent.Action = () => DeleteBlock(tilePos, layer);
            _redoHistory.Push(newEvent);
        }
    }

    public virtual void SpawnBlock(SerializationInfo info, bool calledFromHistory = false)
    {
        SelectedEntity = EntityType.TranslateEntityEnumAndString(info.TypeOfElement);
        SpawnBlock(info.TilePos, calledFromHistory);
    }

    public void DeleteBlock(TileGridEntity entity, bool calledFromHistory = false)
    {
        ReleaseBlock(entity);
        Globals.Inventory.AddEntity(EntityType.TranslateEntityEnumAndType(entity.GetType()));
        _storage.RemoveEntity(entity.TilePosition, Layer);
        (entity as IEntity).Remove();

        if (!calledFromHistory)
        {
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => SpawnBlock((entity as Block).Info.Clone(), true);
            _history.Push(newEvent);
        }
        else
        {
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => SpawnBlock((entity as Block).Info.Clone());
            _redoHistory.Push(newEvent);
        }
    }

    public void DeleteBlock(Vector2 tilePos, int layer, bool calledFromHistory = false)
    {
        var entity = _storage[(int)tilePos.X, (int)tilePos.Y, layer];
        DeleteBlock(entity , calledFromHistory);
    }

    //public void DeleteBlock(SerializationInfo info)
    //{
    //    var entity = _storage[(int)info.TilePos.X, (int)info.TilePos.Y];
    //    _entities.Remove(entity as IEntity);
    //    (entity as IEntity).Remove();

    //    _history.Push(new HistoryEventInfo(() => SpawnBlock(new Vector2(info.TilePos.X, info.TilePos.Y), false)));
    //}

    private void MoveBlock(Vector2 mouseTilePos, Vector2 moveOn = new Vector2(), bool calledFromHistory = false)
    {
        if (!calledFromHistory && moveOn == Vector2.Zero && !IsMouseMoved(mouseTilePos, true, true))
            return;

        if (!calledFromHistory && moveOn == Vector2.Zero)
            moveOn = mouseTilePos - (Vector2)_startMouseTilePos;


        _blocksSpawned = true;
        _groupInteraction = true;
        var toRemove = new List<TileGridEntity>();

        foreach (var entity in _holdingEntities)
        {
            _storage[((TileGridEntity)entity).TilePosition, Layer] = null;
        }
        foreach (var entity in _holdingEntities)
        {
            var tileEntity = entity as TileGridEntity;
            if (!Storage.IsInBounds(tileEntity.TilePosition + moveOn))
            {
                toRemove.Add(tileEntity);
                continue;
            }
            tileEntity.TilePosition += moveOn;

            var coveredEntity = _storage[tileEntity.TilePosition, Layer];
            if (coveredEntity != null)
            {
                DeleteBlock(coveredEntity);
            }

            _storage[tileEntity.TilePosition, Layer] = tileEntity;
            entity.UpdateConstructor();
        }
        foreach (var entity in toRemove)
            DeleteBlock(entity);


        if (!calledFromHistory)
        {
            var moveBack = - moveOn;
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => MoveBlock(Vector2.Zero, moveBack, true);
            _history.Push(newEvent);
        }
        else
        {
            var moveBack = -moveOn;
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => MoveBlock(Vector2.Zero, moveBack);
            _redoHistory.Push(newEvent);
        }

    }
    private bool PossibleToInteract(TileGridEntity entity)
    {
        return (entity is IConstructable &&
            (Globals.IsDeveloperModeEnabled || ((IConstructable)entity).IsEnableToPlayer));
    }

    protected bool IsGroupInteraction()
    {
        if ((IsDrawing() || IsMoving() || IsSelecting()) && MouseInputController.IsPressed)//EVERY FRAME&!&!&!&
            _history.Push(new HistoryEventInfo(HistoryEventType.Separator));
        return (IsDrawing() || IsMoving() || IsSelecting()) && MouseInputController.IsPressed;
    }

    private bool IsDrawing() => Globals.KeyboardInputController.IsPressed(Keys.Space);

    private bool IsMoving() => _holdingEntities.Any() && MouseInputController.LeftButton.IsPressed;

    private bool IsSelecting() => MouseInputController.MiddleButton.IsPressed;



    public void Undo()
    {
        ProcessHistoryStack(_history, _redoHistory);
    }

    public void Redo()
    {
        ProcessHistoryStack(_redoHistory, _history);
    }

    private void ProcessHistoryStack(Stack<HistoryEventInfo> stack, Stack<HistoryEventInfo> otherStack)
    {
        while (stack.Count > 0 && stack.Peek().IsSeparator)
        {
            otherStack.Push(stack.Pop());
        }

        if (stack.Count == 0)
            return;

        if (stack.Peek().IsInGroup)
        {
            _groupInteraction = true;
            while (stack.Count > 0 && stack.Peek().IsInGroup)
                stack.Pop().Invoke();
            _groupInteraction = false;
        }
        else
        {
            stack.Pop().Invoke();
        }
    }

    protected void ClearHistoryStackAction(Stack<HistoryEventInfo> stack)
    {
        while (stack.Count > 0 && stack.Peek().IsSeparator)
        {
            stack.Pop();
        }

        if (stack.Count == 0)
            return;

        if (stack.Peek().IsInGroup)
        {
            while (stack.Count > 0 && stack.Peek().IsInGroup)
                stack.Pop();
        }
        else
        {
            stack.Pop();
        }
    }

    protected bool IsMouseMoved(Vector2 mouseTilePos, bool spawn, bool move = false)
    {
        var moved = true;
        if (_startMouseTilePos == null)
        {
            _startMouseTilePos = mouseTilePos;
            if (move)
                moved = false;
        }
        if (_previousDrawingTilePos != null && mouseTilePos == _previousDrawingTilePos)
            return false;
        if ((spawn && _previousDrawingTilePos != mouseTilePos && _previousDrawingTilePos != null && _blocksSpawned)
            || (!spawn && _blocksDeleted))
        {
            Undo();
            ClearHistoryStackAction(_redoHistory);
            _blocksDeleted = false;
            _blocksSpawned = false;
        }
        _previousDrawingTilePos = mouseTilePos;
        return moved;
    }

    protected void ResetDrawing()
    {
        _startMouseTilePos = null;
        _previousDrawingTilePos = null;
        _blocksDeleted = false;
        _blocksSpawned = false;
        if (Globals.UIManager.SelectionArea.Location != Point.Zero)
            Globals.UIManager.SelectionArea = new Rectangle();
    }
}
