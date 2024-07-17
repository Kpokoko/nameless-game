using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nameless.Code.SceneManager;
using nameless.Controls;
using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless;

public class Constructor : IGameObject
{
    public int DrawOrder => 1;

    private Storage _storage { get { return Globals.CurrentScene.Storage; } }
    private List<IEntity> _entities { get {  return Globals.CurrentScene.Entities; } }
    private IConstructable _holdingEntity { get; set; }

    public void Draw(SpriteBatch spriteBatch)
    {}

    public void Update(GameTime gameTime)
    {
        var mouseTilePos = MouseInputController.MouseTilePos;
        var entityUnderMouse = _storage[(int)mouseTilePos.X, (int)mouseTilePos.Y];

        if (MouseInputController.LeftButton.IsJustPressed && entityUnderMouse is IConstructable)
            HoldBlock(entityUnderMouse as IConstructable);

        if (!MouseInputController.LeftButton.IsPressed && _holdingEntity is not null)
            ReleaseBlock();

        if ((MouseInputController.LeftButton.IsJustReleased || (MouseInputController.LeftButton.IsPressed && Keyboard.GetState().IsKeyDown(Keys.Space))) && entityUnderMouse == null)
            SpawnBlock(mouseTilePos);

        if ((MouseInputController.RightButton.IsJustReleased || (MouseInputController.RightButton.IsPressed && Keyboard.GetState().IsKeyDown(Keys.Space))) && entityUnderMouse is IConstructable)
            DeleteBlock(entityUnderMouse);

        if (entityUnderMouse is null && _holdingEntity is not null)
            _holdingEntity.UpdateConstructor(gameTime);
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

    private void SpawnBlock(Vector2 mouseTilePos)
    {
        _entities.Add(new InventoryBlock((int)mouseTilePos.X, (int)mouseTilePos.Y));
    }

    private void DeleteBlock(IEntity entity)
    {
        _entities.Remove(entity);
        (entity as Block).colliders.RemoveAll();
    }
}
