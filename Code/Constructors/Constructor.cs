using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Code.SceneManager;
using nameless.Controls;
using nameless.Entity;
using nameless.Interfaces;
using nameless.Serialize;
using nameless.UI;
using nameless.UI.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public int Layer { get { return (SelectedEntity is EntityTypeEnum.HitboxTrigger) ? 1 : 0; } }

    public void Draw(SpriteBatch spriteBatch)
    { }

    public void SwitchMode()
    {
        Globals.IsConstructorModeEnabled = Globals.IsConstructorModeEnabled ? false : true;
        if (Globals.IsConstructorModeEnabled)
        {
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
            serializer.Serialize(Globals.SceneManager.GetName(), Globals.SceneManager.GetEntities().Select(x => x as ISerializable).ToList());
        }
    }

    public void Update(GameTime gameTime)
    {
        var mouseTilePos = Storage.IsInBounds(MouseInputController.MouseTilePos) ? MouseInputController.MouseTilePos : _prevMouseTilePos;
        var entityUnderMouse = _storage[(int)mouseTilePos.X, (int)mouseTilePos.Y, Layer];

        if (MouseInputController.LeftButton.IsJustPressed && PossibleToInteract(entityUnderMouse))
            HoldBlock(entityUnderMouse as IConstructable);

        if (!MouseInputController.LeftButton.IsPressed && _holdingEntity is not null) 
            ReleaseBlock();

        if (((MouseInputController.LeftButton.IsJustReleased && !MouseInputController.OnUIElement) || MouseInputController.LeftButton.IsPressed && Keyboard.GetState().IsKeyDown(Keys.Space)) && entityUnderMouse == null)
            SpawnBlock(mouseTilePos);

        if (((MouseInputController.RightButton.IsJustReleased && !MouseInputController.OnUIElement) || MouseInputController.RightButton.IsPressed && Keyboard.GetState().IsKeyDown(Keys.Space)) && PossibleToInteract(entityUnderMouse))
            DeleteBlock(entityUnderMouse);

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

    public virtual void SpawnBlock(Vector2 mouseTilePos)
    {
        if (SelectedEntity is EntityTypeEnum.None) return;
        switch (SelectedEntity)
        {
            case EntityTypeEnum.InventoryBlock:
                _entities.Add(new InventoryBlock((int)mouseTilePos.X, (int)mouseTilePos.Y));
                break;
            default:
                break;
        }
    }

    public void DeleteBlock(TileGridEntity entity)
    {
        _entities.Remove(entity as IEntity);
        (entity as IEntity).Remove();
    }

    private void MoveBlock(Vector2 mouseTilePos)
    {
        _holdingEntity.UpdateConstructor();
        var tileEntity = _holdingEntity as TileGridEntity;
        if (tileEntity.TilePosition != mouseTilePos)
            tileEntity.TilePosition = mouseTilePos;
    }



    private bool PossibleToInteract(TileGridEntity entity)
    {
        return entity is IConstructable && 
            ((Globals.IsDeveloperModeEnabled) || ((IConstructable)entity).IsEnableToPlayer);
    }
}
