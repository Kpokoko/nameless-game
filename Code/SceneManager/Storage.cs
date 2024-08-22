using Microsoft.Xna.Framework.Input;
using nameless.Entity;
using nameless.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.SceneManager
{
    public class Storage
    {
        private TileGridEntity[][,] _entitiesArray = new[] { new TileGridEntity[StorageWidth, StorageHeight] , new TileGridEntity[StorageWidth, StorageHeight], new TileGridEntity[StorageWidth, StorageHeight] };
        private List<Attacher> _attachers = new();
        public const int StorageWidth = 23;
        public const int StorageHeight = 13;

        public Storage(List<IEntity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i] as TileGridEntity;
                if (entity is null) continue;
                var layer = entity.Layer;
                var pos = entity.TilePosition;

                if (!IsInBounds(pos))
                {
                    entities.Remove(entities[i]);
                    continue;
                }

                if (entity is Attacher)
                {
                    _attachers.Add(entity as Attacher);
                    continue;
                }
                _entitiesArray[layer][(int)pos.X, (int)pos.Y] = entity;
                if (entity is EditorBlock)
                    _entitiesArray[layer][(int)pos.X, (int)pos.Y - 1] = entity;
            }
        }

        public TileGridEntity this[int x, int y, int layer = 0]
        {
            get { return _entitiesArray[layer][x, y]; }
            set { _entitiesArray[layer][x, y] = value; }
        }

        public TileGridEntity this[Vector2 tilePos, int layer = 0]
        {
            get { return _entitiesArray[layer][(int)tilePos.X, (int)tilePos.Y]; }
            set { _entitiesArray[layer][(int)tilePos.X, (int)tilePos.Y] = value; }
        }

        public void RemoveEntity(int x, int y, int layer = 0)
        {
            var iEntity = (IEntity)this[x, y, layer];
            this[x, y, layer] = null;
            if (iEntity != null && Globals.SceneManager.GetEntities().Contains(iEntity))
                Globals.SceneManager.GetEntities().Remove(iEntity);
        }
        public void RemoveEntity(Vector2 tilePos, int layer = 0) => RemoveEntity((int)tilePos.X, (int)tilePos.Y, layer);

        public void AddEntity(TileGridEntity entity, int x, int y, int layer = 0)
        {
            _entitiesArray[layer][x, y] = entity;
            var iEntity = (IEntity)entity;
            if (!Globals.SceneManager.GetEntities().Contains(iEntity))
                Globals.SceneManager.GetEntities().Add(iEntity);
        }
        public void AddEntity(TileGridEntity entity, Vector2 tilePos, int layer = 0) => AddEntity(entity, (int)tilePos.X, (int)tilePos.Y, layer);

        public void AddSlimEntity(Attacher attacher)
        {
            if (!_attachers.Contains(attacher))
            {
                _attachers.Add(attacher);
                if (!Globals.SceneManager.GetEntities().Contains(attacher))
                    Globals.SceneManager.GetEntities().Add(attacher);
            }
        }
        public TileGridEntity[][,] GetArray()
        {
            return _entitiesArray;
        }

        public static bool IsInBounds(Vector2 tilePos)
        {
            return tilePos.X >= 0 && tilePos.X < StorageWidth && tilePos.Y >= 0 && tilePos.Y < StorageHeight;
        }

        public bool IsTileFree(Vector2 tilePos, int layer = 0)
        {
            var tile = this[tilePos, layer];
            return tile == null || ((tile is IBreakable) && ((IBreakable)tile).Broken) || tile is MovingPlatform;
        }

        public bool IsFreeBetweenTiles(Vector2 tile1, Vector2 tile2)
        {
            var ordered = new[] { tile1, tile2 };
            ordered = ordered.OrderBy(x => x.Length()).ToArray();
            return (!_attachers.Any(a => a.BetweenTiles[0] == ordered[0] && a.BetweenTiles[1] == ordered[1]));
        }

        public void UpdateAttachers()
        {
            foreach (var attacher in _attachers)
                attacher.Attach();
        }

        public int GetLength(int dimension)
        {
            return _entitiesArray[0].GetLength(dimension);
        }

        public EntityTypeEnum[,] ConvertToEnum(int layer = 0)
        {
            var converted = new EntityTypeEnum[StorageWidth, StorageHeight];
            for (var i = 0; i < _entitiesArray[layer].GetLength(0); i++)
            {
                for (var j = 0; j < _entitiesArray[layer].GetLength(1); j++)
                {
                    var currCell = _entitiesArray[layer][i, j];
                    if (currCell == null) converted[i, j] = EntityTypeEnum.None;
                    else
                        converted[i, j] = EntityType.TranslateEntityEnumAndType(currCell.GetType());
                }
            }
            return converted;
        }
    }
}
