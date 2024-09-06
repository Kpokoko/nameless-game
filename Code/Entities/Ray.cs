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
    private RayCaster _caster;
    private Vector2 _direction;
    private HitboxTrigger _laser;

    public Ray(int x, int y, int width, int height, RayCaster caster, Vector2 dir) : base(x, y, width, height)
    {
        if (dir.Y != 0)
        {
            _width = width;
            _height = 0;
        }
        else
        {
            _width = 0;
            _height = height;
        }
        _caster = caster;
        _direction = dir;
        _laser = HitboxTrigger.CreateDamagePlayerTrigger(this, width, height);
    }

    public int DrawOrder => 1;

    public override void Update(GameTime gameTime)
    {
        if (_caster.Distances == null || _caster.Distances.Count == 0)
            return;
        _laser.RemoveCollider();
        if (_direction.X != 0)
        {
            _width = _caster.Distances.Min();
            Position = new Vector2((_width / 2 * _direction.X) + _caster.Position.X, _caster.Position.Y);
            _laser = HitboxTrigger.CreateDamagePlayerTrigger(this, Math.Abs((int)_width), (int)_height);
        }
        else
        {
            _height = _caster.Distances.Min();
            Position = new Vector2(_caster.Position.X, (_height / 2 * _direction.Y) + _caster.Position.Y);
             _laser = HitboxTrigger.CreateDamagePlayerTrigger(this, (int)_width, Math.Abs((int)_height));
        }
    }
}
