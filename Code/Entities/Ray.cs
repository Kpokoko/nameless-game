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

    public Ray(int x, int y, int width, int height, Ray prevSegment, RayCaster caster) : base (x, y, width, height)
    {
        _width = width;
        _height = height;
        var trigger = HitboxTrigger.CreateDamagePlayerTrigger(this, width, height);
        var hitbox = new HitboxTrigger(this, width, height, ReactOnProperty.ReactOnEntityType, SignalProperty.Continuous);
        hitbox.SetTriggerEntityTypes(typeof(MovingPlatform));
        hitbox.OnCollisionEvent += () => RemoveRay();
        Colliders.Add(trigger);
        Colliders.Add(hitbox);
        _caster = caster;
        _previousRaySegment = prevSegment;
        if (caster.Storage != null)
            _caster.Storage.AddEntity(this, new Vector2(TilePosition.X, TilePosition.Y), 2);
        Colliders[0].Color = Color.Transparent;
    }

    public int DrawOrder => 1;

    public override void Update(GameTime gameTime)
    {
        if (_caster.Storage[new Vector2(TilePosition.X + 1, TilePosition.Y), 0] is null && _caster.Storage[new Vector2(TilePosition.X + 1, TilePosition.Y), 2] is null)
            _nextRaySegment = new Ray((int)TilePosition.X + 1, (int)TilePosition.Y, (int)_width, (int)_height, this, _caster);
        if (_nextRaySegment != null)
        {
            if (_nextRaySegment.IsNeedToBeDeleted)
            {
                _nextRaySegment = null;
                return;
            }
            _nextRaySegment.Update(gameTime);
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


//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended;
//using MonoGame.Extended.Collisions;
//using nameless.Collisions;
//using nameless.Interfaces;
//using nameless.Tiles;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace nameless.Entity;

//public class Ray : IEntity, ICollider
//{
//    private Vector2 _position;
//    private Vector2 _tilePos;
//    private Vector2 _currentRayEnd;
//    private Vector2 _prevRayEnd = new Vector2(-100, -100);
//    private float _width;
//    private float _height;
//    private float _speed;
//    private Block _caster;
//    private Block _laserHead;
//    private float _absoluteWidth;
//    public Colliders Colliders { get; set; } = new();

//    public Ray(int x, int y, int width, int height, float speed, Block caster)
//    {
//        Position = new Vector2(x, y);
//        TilePosition = new Vector2(x, y);
//        _width = width;
//        _height = height;
//        var trigger = HitboxTrigger.CreateDamagePlayerTrigger(caster, width, height);
//        Colliders.Add(trigger);
//        Colliders[0].Color = Color.HotPink;
//        _caster = caster;
//        _speed = speed;
//        _laserHead = new Block(x, y, 1, 1);
//    }

//    public Vector2 Position
//    {
//        get { return _position; }
//        set
//        {
//            _position = value;
//        }
//    }
//    public virtual Vector2 TilePosition
//    {
//        get { return _tilePos; }
//        set
//        {
//            _tilePos = value;
//            Position = Tile.GetTileCenter(_tilePos);
//        }
//    }

//    public int DrawOrder => 1;

//    public void Remove()
//    {
//        Colliders.RemoveAll();
//    }

//    public void Update(GameTime gameTime)
//    {
//        //if (_prevRayEnd == _currentRayEnd) return;
//        //if (_absoluteWidth > 100) return;
//        Position = new Vector2(Position.X + _width, Position.Y);
//        var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // Use TotalSeconds instead of Milliseconds
//        if (elapsedTime != 0)
//            _width = _width + _speed * elapsedTime; // Adjust the multiplication logic
//        var a = Position.X;
//        _currentRayEnd.X = Position.X + _width;

//        _absoluteWidth = _currentRayEnd.X - _caster.Position.X;

//        _laserHead.Position = Position;

//        if (Colliders.colliders.Count > 100)
//        {
//            foreach (var collider in Colliders.colliders)
//            {
//                collider.RemoveCollider();
//            }
//            Colliders.colliders.Clear();
//        }
//        Colliders.Add(HitboxTrigger.CreateDamagePlayerTrigger(_laserHead, (int)_absoluteWidth, (int)_height));

//        _prevRayEnd = _currentRayEnd;
//    }


//    public void Draw(SpriteBatch spriteBatch)
//    {

//    }

//    public void OnCollision(params CollisionEventArgs[] collisionsInfo)
//    {
//        Globals.SceneManager.GetPlayer().Death();
//    }
//}
