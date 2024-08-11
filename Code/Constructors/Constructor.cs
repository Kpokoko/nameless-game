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
    private Vector2 _prevMouseTilePos;
    public int DrawOrder => 1;
    private Storage _storage { get { return Globals.SceneManager.GetStorage(); } }
    protected List<IEntity> _entities { get { return Globals.SceneManager.GetEntities(); } }
    private IConstructable _holdingEntity { get; set; }
    public EntityTypeEnum SelectedEntity { get; set; }
    public object SelectedEntityProperty { get; set; }
    public Type SelectedEntityType { get {  return EntityType.TranslateEntityEnumAndType(SelectedEntity); } }
    public int Layer { get { return (SelectedEntity is EntityTypeEnum.Pivot) ? 1 : 0; } }

    private Stack<HistoryEventInfo> _history = new();

    private Stack<HistoryEventInfo> _redoHistory = new();

    private bool _groupInteraction;

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



    public void Update(GameTime gameTime)
    {
        var mouseTilePos = Storage.IsInBounds(MouseInputController.MouseTilePos) ? MouseInputController.MouseTilePos : _prevMouseTilePos;
        var entityUnderMouse = _storage[(int)mouseTilePos.X, (int)mouseTilePos.Y, Layer];

        _groupInteraction = IsGroupInteraction();

        if (MouseInputController.OnUIElement)
            return;

        if (MouseInputController.LeftButton.IsJustPressed && PossibleToInteract(entityUnderMouse))
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

    private bool IsGroupInteraction()
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
        Block entity = null;
        switch (SelectedEntity)
        {
            case EntityTypeEnum.InventoryBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    entity = new InventoryBlock((int)tilePos.X, (int)tilePos.Y);
                    _entities.Add(entity);
                }
                break;
            case EntityTypeEnum.StickyBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    entity = new StickyBlock((int)tilePos.X, (int)tilePos.Y);
                    _entities.Add(entity);
                }
                break;
            case EntityTypeEnum.FragileBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    entity = new FragileBlock((int)tilePos.X, (int)tilePos.Y);
                    _entities.Add(entity);
                }
                break;
            case EntityTypeEnum.TemporaryBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    entity = new TemporaryBlock((int)tilePos.X, (int)tilePos.Y);
                    _entities.Add(entity);
                }
                break;
            case EntityTypeEnum.DelayedDeathBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    entity = new DelayedDeathBlock((int)tilePos.X, (int)tilePos.Y);
                    _entities.Add(entity);
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
        _entities.Remove(entity as IEntity);
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
        if (tileEntity.TilePosition != mouseTilePos)
            tileEntity.TilePosition = mouseTilePos;
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
}
