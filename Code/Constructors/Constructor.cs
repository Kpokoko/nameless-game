using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Code.SceneManager;
using nameless.Controls;
using nameless.Entity;
using nameless.Interfaces;
using nameless.Serialize;
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
    private IConstructable _holdingEntity { get; set; }
    public EntityTypeEnum SelectedEntity { get; set; }
    public object SelectedEntityProperty { get; set; }
    public Type SelectedEntityType { get {  return EntityType.TranslateEntityEnumAndType(SelectedEntity); } }
    public int Layer { get { return (SelectedEntity is EntityTypeEnum.Pivot) ? 1 : 0; } }

    protected Stack<HistoryEventInfo> _history = new();

    protected Stack<HistoryEventInfo> _redoHistory = new();

    protected bool _groupInteraction;

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
            Globals.UIManager.ShowMap();
            //Globals.UIManager.CurrentUIScenes[UIScenes.ConstructorScene]
            //    .AddElements(new Minimap(new Vector2(1600, 300 + 220), 0, 0, _storage, Alignment.Center));
        }
        else
        {
            Globals.UIManager.RemoveScene(UIScenes.ConstructorScene);
            Globals.UIManager.HideMap();
            var serializer = new Serializer();
            serializer.SerializeScene(Globals.SceneManager.GetName(), Globals.SceneManager.GetEntities().Select(x => x as ISerializable).ToList());
            serializer.SaveInventory(Globals.Inventory.GetInventory());
            _history.Clear();
            _redoHistory.Clear();
        }
    }



    public virtual void Update(GameTime gameTime)
    {
        var mouseTilePos = Storage.IsInBounds(MouseInputController.MouseTilePos) ? MouseInputController.MouseTilePos : _prevMouseTilePos;
        var entityUnderMouse = _storage[(int)mouseTilePos.X, (int)mouseTilePos.Y, Layer];

        _groupInteraction = IsGroupInteraction();

        if (MouseInputController.OnUIElement)
            return;

        if (MouseInputController.LeftButton.IsJustPressed && PossibleToInteract(entityUnderMouse) && ! _groupInteraction)
            HoldBlock(entityUnderMouse as IConstructable);

        if (!MouseInputController.LeftButton.IsPressed && _holdingEntity is not null) 
            ReleaseBlock();

        if (((MouseInputController.LeftButton.IsJustReleased) || MouseInputController.LeftButton.IsPressed && _groupInteraction) && entityUnderMouse == null)
            SpawnBlock(mouseTilePos);

        if (((MouseInputController.RightButton.IsJustReleased) || MouseInputController.RightButton.IsPressed && _groupInteraction) && PossibleToInteract(entityUnderMouse))
            DeleteBlock(entityUnderMouse);

        if (entityUnderMouse is null && _holdingEntity is not null)
           MoveBlock(mouseTilePos);

        _prevMouseTilePos = mouseTilePos;
    }

    protected bool IsGroupInteraction()
    {
        if (Globals.KeyboardInputController.IsPressed(Keys.Space) && MouseInputController.IsJustPressed)
            _history.Push(new HistoryEventInfo(null, HistoryEventType.Separator));
        return Globals.KeyboardInputController.IsPressed(Keys.Space);
    }

    private void HoldBlock(IConstructable entity)
    {
        entity.IsHolding = true;
        _holdingEntity = entity;
    }

    private void ReleaseBlock()
    {
        _holdingEntity.IsHolding = false;
        _holdingEntity = null;
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

    private void MoveBlock(Vector2 mouseTilePos)
    {
        var tileEntity = _holdingEntity as TileGridEntity;
        _storage[tileEntity.TilePosition, Layer] = null;
        if (tileEntity.TilePosition != mouseTilePos)
            tileEntity.TilePosition = mouseTilePos;
        _storage[mouseTilePos, Layer] = tileEntity;
        _holdingEntity.UpdateConstructor();

    }


    private bool PossibleToInteract(TileGridEntity entity)
    {
        return (entity is IConstructable && 
            (Globals.IsDeveloperModeEnabled || ((IConstructable)entity).IsEnableToPlayer)) ;
    }

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
}
