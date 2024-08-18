using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using nameless.Collisions;
using nameless.Entity;
using nameless.Interfaces;
using nameless.Serialize;
using nameless.Tiles;

namespace nameless.Entity;

/// <summary>
/// Invisible entity.
/// </summary>
public class Pivot :  Block, IEntity, ICollider
{

    public Pivot(int x, int y, int width = 64, int height = 64)
    {
        TilePosition = new Vector2(x, y);
        Layer = 1;
        Colliders.Add(new Collider(this, width, height));
        Globals.CollisionManager.CollisionComponent.Remove(Colliders[0]);
        Colliders[0].Color = Color.LightSteelBlue * 0.4f;
    }
    public Pivot(Vector2 tilePosition) : this((int)tilePosition.X, (int)tilePosition.Y) { }


    public override void PrepareSerializationInfo()
    {
        base.PrepareSerializationInfo();
        Info.TriggerType = Colliders.GetTriggerType();
    }
}
