using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using nameless.Code.SceneManager;
using nameless.Collisions;
using nameless.Interfaces;
using nameless.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public class Ray : Pivot
{
    private float _width;
    private float _height;
    private Ray _previousRaySegment;
    private Ray _nextRaySegment;
    private RayCaster _caster;
    public bool IsNeedToBeDeleted = false;
    private Vector2 _direction;

    public Ray(int x, int y, int width, int height, Ray prevSegment, RayCaster caster, Vector2 dir) : base(x, y, width, height)
    {
        _width = width;
        _height = height;
        var trigger = HitboxTrigger.CreateDamagePlayerTrigger(this, width, height);
        var hitbox = new HitboxTrigger(this, width, height, ReactOnProperty.ReactOnEntityType, SignalProperty.Continuous);
        hitbox.SetTriggerEntityTypes(typeof(MovingBlock));
        hitbox.OnCollisionEvent += () => RemoveRay();
        Colliders.Add(trigger);
        Colliders.Add(hitbox);
        _caster = caster;
        _previousRaySegment = prevSegment;
        if (caster.Storage != null)
            _caster.Storage.AddEntity(this, new Vector2(TilePosition.X, TilePosition.Y), 2);
        Colliders[0].Color = Color.Transparent;
        _direction = dir;
    }

    public int DrawOrder => 1;

    public override void Update(GameTime gameTime)
    {
        if (_caster.Storage[new Vector2(TilePosition.X + _direction.X, TilePosition.Y + _direction.Y), 0] is null &&
        _caster.Storage[new Vector2(TilePosition.X + _direction.X, TilePosition.Y + _direction.Y), 2] is null &&
        Storage.IsInBounds(new Vector2(TilePosition.X + _direction.X, TilePosition.Y + _direction.Y)))
            _nextRaySegment = new Ray((int)(TilePosition.X + _direction.X), (int)(TilePosition.Y + _direction.Y), (int)_width, (int)_height, this, _caster, _direction);
        if (_nextRaySegment != null)
        {
            if (_width > 0)
            {
                _width = _caster.Distances.Min();
                Position = new Vector2(_width / 2 + _caster.Position.X, _caster.Position.Y);
                _laser = HitboxTrigger.CreateDamagePlayerTrigger(this, Math.Abs((int)_width), (int)_height);
            }
            else
            {
                _width = _caster.Distances.Max();
                Position = new Vector2(_width / 2 + 32 + _caster.Position.X, _caster.Position.Y);
                _laser = HitboxTrigger.CreateDamagePlayerTrigger(this, Math.Abs((int)_width + 64), (int)_height);
            }
        }
        else
        {
            if (_height > 0)
            {
                _height = _caster.Distances.Min();
                Position = new Vector2(_caster.Position.X, _height / 2 + _caster.Position.Y);
                _laser = HitboxTrigger.CreateDamagePlayerTrigger(this, (int)_width, Math.Abs((int)_height));
            }
            else
            {
                _height = _caster.Distances.Max();
                Position = new Vector2(_caster.Position.X, _height / 2 + _caster.Position.Y + 32);
                _laser = HitboxTrigger.CreateDamagePlayerTrigger(this, (int)_width, Math.Abs((int)_height + 64));
            }
        }

    }

    public void OnCollision(params CollisionEventArgs[] collisionsInfo)
    { }

    public void RemoveRay()
    {
        if (_nextRaySegment != null)
            _nextRaySegment.RemoveRay();
        _caster.Storage.RemoveEntity(TilePosition, 2);
        this.Remove();
        IsNeedToBeDeleted = true;
    }
}
