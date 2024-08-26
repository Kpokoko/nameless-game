using Microsoft.Xna.Framework.Input;
using nameless.Entity;
using nameless.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.SceneManager
{
    public class Storage
    {
        private TileGridEntity[][,] _entitiesArray = new[] { new TileGridEntity[StorageWidth, StorageHeight] , new TileGridEntity[StorageWidth, StorageHeight], new TileGridEntity[StorageWidth, StorageHeight] };
        private Dictionary<Vector2,Attacher> _attachers = new();

        public Dictionary<Block, MovingBlock> AttachedMovers = new();
        private Dictionary<MovingBlock, HashSet<Block>> AttachedToMover = new();

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
                    var attacher = entity as Attacher;
                    _attachers[attacher.BetweenTilePosition] = attacher;
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

        public Attacher this[Vector2 tilePos, Vector2 secondTilePos]
        {
            get
            {
                //var ordered = new[] { tilePos, secondTilePos };
                //ordered = ordered.OrderBy(x => x.Length()).ToArray(); 
                return _attachers[(tilePos + secondTilePos) / 2]; 
            }
            set { AddAttacher(value); }
        }

        public Attacher this[Vector2[] orderedTiles]
        {
            get => _attachers[(orderedTiles[0] + orderedTiles[1]) / 2];
            set { AddAttacher(value); }
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

        public void AddAttacher(Attacher attacher)
        {
            if (!_attachers.ContainsKey(attacher.BetweenTilePosition))
            {
                _attachers[attacher.BetweenTilePosition] = attacher;
                if (!Globals.SceneManager.GetEntities().Contains(attacher))
                    Globals.SceneManager.GetEntities().Add(attacher);
            }
        }

        public void RemoveAttacher(Attacher attacher)
        {
            _attachers.Remove(attacher.BetweenTilePosition);
            Globals.SceneManager.GetEntities().Remove(attacher);
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
            return tile == null || ((tile is IBreakable) && ((IBreakable)tile).Broken) || tile is MovingBlock;
        }

        public bool IsFreeBetweenTiles(Vector2 tile1, Vector2 tile2)
        {
            //var ordered = new[] { tile1, tile2 };
            //ordered = ordered.OrderBy(x => x.Length()).ToArray();
            return !_attachers.ContainsKey((tile1 + tile2) / 2);
        }

        public void UpdateAttachers()
        {
            foreach (var attacher in _attachers.Values)
                attacher.Attach();
            MeasureAttachedMovingBlocks(_attachers.Values.ToHashSet());
        }

        private void MeasureAttachedMovingBlocks(HashSet<Attacher> toInspect)
        {
            if (toInspect.Count == 0)
                return;
            var attacher = toInspect.First();
            toInspect.Remove(attacher);
            var toVisit = attacher.AttachedBlocks.ToList();
            var visited = new HashSet<Block>();
            var movingBlocks = new HashSet<Block>();
            while (toVisit.Count > 0)
            {
                var block = toVisit[^1];
                toVisit.RemoveAt(toVisit.Count-1);

                if (visited.Contains(block)) 
                    continue;
                visited.Add(block);

                if (block is Attacher)
                    toInspect.Remove((Attacher)block);
                else if (block is MovingBlock)
                    movingBlocks.Add((MovingBlock)block);
                
                foreach (var at in block.AttachedBlocks)
                    toVisit.Add(at);
                    
            }
            ResolveAttachedMovement(movingBlocks, out Block resultingBlock);
            //CheckIfBlocked(visited, resultingBlock);

            if (resultingBlock != null)
            {
                AttachedToMover[(MovingBlock)resultingBlock] = visited; 
                foreach (var groupBlock in visited)
                    AttachedMovers[groupBlock] = (MovingBlock)resultingBlock;
            }
            MeasureAttachedMovingBlocks(toInspect);
        }

        public void UpdateMovingBlocksState()
        {
            foreach (var moving in Globals.SceneManager.GetEntities()
                .Where(e => e is MovingBlock && (e as Block).AttachedBlocks == null)
                .Select(e => e as MovingBlock))
                moving.IsBlocked();
            foreach (var group in AttachedToMover)
            {
                CheckIfBlocked(group.Value, group.Key);
            }
        }

        private void CheckIfBlocked(HashSet<Block> attachedBlocks, Block movingBlock)
        {
            var dir = (movingBlock as MovingBlock).Direction;
            bool forward = false; 
            bool backward = false;
            foreach (var block in attachedBlocks)
            {
                var fBlock = block.TilePosition + dir;
                var bBlock = block.TilePosition - dir;
                if (!IsTileFree(fBlock) && !attachedBlocks.Contains(this[fBlock]))
                    forward = true;
                if (!IsTileFree(bBlock) && !attachedBlocks.Contains(this[bBlock]))
                    backward = true;
                if (forward && backward)
                    break;
            }
            if (forward && backward)
                (movingBlock as MovingBlock).Static = true;
            else 
                (movingBlock as MovingBlock).Static = false;
        }

        private void ResolveAttachedMovement(HashSet<Block> movingBlocks, out Block resultingBlock)
        {
            var fastestBlock = movingBlocks.MaxBy(block => ((MovingBlock)block).Speed);
            foreach (var block in movingBlocks.Select(b => b as MovingBlock).Where(b => b != (MovingBlock)fastestBlock))
                block.Static = true;
            //(fastestBlock as MovingBlock).Static = false;
            resultingBlock = fastestBlock;
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
