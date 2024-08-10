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

    private Stack<Action> _history = new Stack<Action>();

    private Stack<Action> _redoHistory = new Stack<Action>();

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
        if (_history.Count > 0)
            _history.Pop().Invoke();
    }

    public void Redo()
    {
        if (_redoHistory.Count > 0)
            _redoHistory.Pop().Invoke();
    }

    public void Update(GameTime gameTime)
    {
        var mouseTilePos = Storage.IsInBounds(MouseInputController.MouseTilePos) ? MouseInputController.MouseTilePos : _prevMouseTilePos;
        var entityUnderMouse = _storage[(int)mouseTilePos.X, (int)mouseTilePos.Y, Layer];

        if (MouseInputController.OnUIElement)
            return;

        if (MouseInputController.LeftButton.IsJustPressed && PossibleToInteract(entityUnderMouse))
            HoldBlock(entityUnderMouse as IConstructable);

        if (!MouseInputController.LeftButton.IsPressed && _holdingEntity is not null) 
            ReleaseBlock();

        if (((MouseInputController.LeftButton.IsJustReleased) || MouseInputController.LeftButton.IsPressed && Keyboard.GetState().IsKeyDown(Keys.Space)) && entityUnderMouse == null)
            SpawnBlock(mouseTilePos, false);

        if (((MouseInputController.RightButton.IsJustReleased) || MouseInputController.RightButton.IsPressed && Keyboard.GetState().IsKeyDown(Keys.Space)) && PossibleToInteract(entityUnderMouse))
            DeleteBlock(entityUnderMouse, false);

        if (entityUnderMouse is null && _holdingEntity is not null)
           MoveBlock(mouseTilePos);
        _prevMouseTilePos = mouseTilePos;
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

    public virtual void SpawnBlock(Vector2 mouseTilePos, bool isCalledFromStack)
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
        if (!isCalledFromStack)
            _history.Push(() => DeleteBlock(_storage[(int)mouseTilePos.X, (int)mouseTilePos.Y], true));
        else if (entity != null)
            _redoHistory.Push(() => SpawnBlock(entity.Info));
    }

    public virtual void SpawnBlock(SerializationInfo info)
    {
        SelectedEntity = EntityType.TranslateEntityEnumAndString(info.TypeOfElement);
        SpawnBlock(info.TilePos, false);
        //_history.Push(() => DeleteBlock((new Block((int)info.TilePos.X, (int)info.TilePos.Y)) as TileGridEntity, false));
    }

    public void DeleteBlock(TileGridEntity entity, bool isCalledFromStack)
    {
        Globals.Inventory.AddEntity(EntityType.TranslateEntityEnumAndType(entity.GetType()));
        _entities.Remove(entity as IEntity);
        (entity as IEntity).Remove();
        if (!isCalledFromStack)
            _history.Push(() => SpawnBlock((entity as Block).Info));
        else
            _redoHistory.Push(() => SpawnBlock((entity as Block).Info));
    }

    public void DeleteBlock(SerializationInfo info)
    {
        var entity = _storage[(int)info.TilePos.X, (int)info.TilePos.Y];
        _entities.Remove(entity as IEntity);
        (entity as IEntity).Remove();
        //switch (info.TypeOfElement)
        //{
        //    case "Block":
        //        var block = new Block((int)info.TilePos.X, (int)info.TilePos.Y);
        //        _entities.Add(new Block((int)info.TilePos.X, (int)info.TilePos.Y));
        //        break;
        //    default:
        //        break;
        //}
        _history.Push(() => SpawnBlock(new Vector2(info.TilePos.X, info.TilePos.Y), false));
    }

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
