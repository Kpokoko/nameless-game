using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Code.SceneManager;
using nameless.Controls;
using nameless.Entity;
using nameless.Interfaces;
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
    public int DrawOrder => 1;
    private Storage _storage { get { return Globals.SceneManager.GetStorage(); } }
    protected List<IEntity> _entities { get { return Globals.SceneManager.GetEntities(); } }
    private IConstructable _holdingEntity { get; set; }
    public EntityType SelectedEntity { get; set; }

    public void Draw(SpriteBatch spriteBatch)
    { }

    public void SwitchMode()
    {
        Globals.IsConstructorModeEnabled = Globals.IsConstructorModeEnabled ? false : true;
        if (Globals.IsConstructorModeEnabled)
        {
            Globals.UIManager.SetScene(UIScenes.ConstructorScene);
        }
        else
        {
            Globals.UIManager.RemoveScene(UIScenes.ConstructorScene);
        }
    }

    public void Update(GameTime gameTime)
    {
        var mouseTilePos = MouseInputController.MouseTilePos;
        var entityUnderMouse = _storage[(int)mouseTilePos.X, (int)mouseTilePos.Y];

        if (MouseInputController.LeftButton.IsJustPressed && PossibleToInteract(entityUnderMouse))
            HoldBlock(entityUnderMouse as IConstructable);

        if (!MouseInputController.LeftButton.IsPressed && _holdingEntity is not null) 
            ReleaseBlock();

        if ((MouseInputController.LeftButton.IsJustReleased || MouseInputController.LeftButton.IsPressed && Keyboard.GetState().IsKeyDown(Keys.Space)) && entityUnderMouse == null)
            SpawnBlock(mouseTilePos);

        if ((MouseInputController.RightButton.IsJustReleased || MouseInputController.RightButton.IsPressed && Keyboard.GetState().IsKeyDown(Keys.Space)) && PossibleToInteract(entityUnderMouse))
            DeleteBlock(entityUnderMouse);

        if (entityUnderMouse is null && _holdingEntity is not null)
           MoveBlock(mouseTilePos);
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

    protected virtual void SpawnBlock(Vector2 mouseTilePos)
    {
        if (SelectedEntity is EntityType.None) return;
        switch (SelectedEntity)
        {
            case EntityType.InventoryBlock:
                _entities.Add(new InventoryBlock((int)mouseTilePos.X, (int)mouseTilePos.Y));
                break;
            default:
                break;
        }
    }

    private void DeleteBlock(IEntity entity)
    {
        _entities.Remove(entity);
        (entity as Block).colliders.RemoveAll();
    }

    private void MoveBlock(Vector2 mouseTilePos)
    {
        _holdingEntity.UpdateConstructor();
        var tileEntity = _holdingEntity as TileGridEntity;
        if (tileEntity.TilePosition != mouseTilePos)
            tileEntity.TilePosition = mouseTilePos;
    }



    private bool PossibleToInteract(IEntity entity)
    {
        return entity is IConstructable && 
            (Globals.IsDeveloperModeEnabled || ((IConstructable)entity).IsEnableToPlayer);
    }
}
