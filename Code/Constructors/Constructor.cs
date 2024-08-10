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

    public void Undo()
    {
        while (_history.Count > 0 && _history.Peek().IsSeparator)
        {
            _redoHistory.Push(_history.Pop());
        }

        if (_history.Count == 0)
            return;

        if (_history.Peek().IsInGroup)
        {
            _groupInteraction = true;
            while (_history.Count > 0 && _history.Peek().IsInGroup)
                _history.Pop().Invoke();
            _groupInteraction = false;
        }
        else
        {
            _history.Pop().Invoke();
        }
    }

    public void Redo()
    {
        while (_redoHistory.Count > 0 && _redoHistory.Peek().IsSeparator)
        {
            _history.Push(_redoHistory.Pop());
        }

        if (_redoHistory.Count == 0)
            return;

        if (_redoHistory.Peek().IsInGroup)
        {
            _groupInteraction = true;
            while (_redoHistory.Count > 0 && _redoHistory.Peek().IsInGroup)
                _redoHistory.Pop().Invoke();
            _groupInteraction = false;
        }
        else
        {
            _redoHistory.Pop().Invoke();
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

    public virtual void SpawnBlock(Vector2 mouseTilePos, bool calledFromHistory = false)
    {
        if (SelectedEntity is EntityTypeEnum.None) return;
        Block entity = null;
        switch (SelectedEntity)
        {
            case EntityTypeEnum.InventoryBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    entity = new InventoryBlock((int)mouseTilePos.X, (int)mouseTilePos.Y);
                    _entities.Add(entity);
                }
                break;
            case EntityTypeEnum.StickyBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    entity = new StickyBlock((int)mouseTilePos.X, (int)mouseTilePos.Y);
                    _entities.Add(entity);
                }
                break;
            case EntityTypeEnum.FragileBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    entity = new FragileBlock((int)mouseTilePos.X, (int)mouseTilePos.Y);
                    _entities.Add(entity);
                }
                break;
            case EntityTypeEnum.TemporaryBlock:
                if (Globals.Inventory.TryGetEntity(SelectedEntity))
                {
                    entity = new TemporaryBlock((int)mouseTilePos.X, (int)mouseTilePos.Y);
                    _entities.Add(entity);
                }
                break;
            default:
                break;
        }

        if (!calledFromHistory)
        {
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => DeleteBlock(_storage[(int)mouseTilePos.X, (int)mouseTilePos.Y], true);
            _history.Push(newEvent);
        }
        else if (entity != null)
        {
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => SpawnBlock(entity.Info);
            _redoHistory.Push(newEvent);
        }
    }

    public virtual void SpawnBlock(SerializationInfo info)
    {
        SelectedEntity = EntityType.TranslateEntityEnumAndString(info.TypeOfElement);
        SpawnBlock(info.TilePos, true);
    }

    public void DeleteBlock(TileGridEntity entity, bool calledFromHistory = false)
    {
        Globals.Inventory.AddEntity(EntityType.TranslateEntityEnumAndType(entity.GetType()));
        _entities.Remove(entity as IEntity);
        (entity as IEntity).Remove();

        if (!calledFromHistory)
        {
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => SpawnBlock((entity as Block).Info);
            _history.Push(newEvent);
        }
        else
        {
            var newEvent = new HistoryEventInfo(_groupInteraction ? HistoryEventType.Group : HistoryEventType.Solo);
            newEvent.Action = () => SpawnBlock((entity as Block).Info);
            _redoHistory.Push(newEvent);
        }
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
}
