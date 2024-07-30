using Microsoft.Xna.Framework.Input;
using nameless.Entity;
using nameless.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Code.SceneManager
{
    public class Storage
    {
        private TileGridEntity[][,] Entities = new[] { new TileGridEntity[StorageWidth, StorageHeight] , new TileGridEntity[StorageWidth, StorageHeight] };
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

                Entities[layer][(int)pos.X, (int)pos.Y] = entity;
                if (entity is EditorBlock)
                    Entities[layer][(int)pos.X, (int)pos.Y - 1] = entity;
            }
        }

        public TileGridEntity this[int index, int index2, int layer = 0]
        {
            get { return Entities[layer][index, index2]; }
            set { Entities[layer][index, index2] = value; }
        }

        public TileGridEntity[][,] GetArray()
        {
            return Entities;
        }

        public static bool IsInBounds(Vector2 pos)
        {
            return pos.X >= 0 && pos.X < StorageWidth && pos.Y >= 0 && pos.Y < StorageHeight;
        }

        public int GetLength(int dimension)
        {
            return Entities[0].GetLength(dimension);
        }

        public EntityTypeEnum[,] ConvertToEnum(int layer = 0)
        {
            var converted = new EntityTypeEnum[StorageWidth, StorageHeight];
            for (var i = 0; i < Entities[layer].GetLength(0); i++)
            {
                for (var j = 0; j < Entities[layer].GetLength(1); j++)
                {
                    var currCell = Entities[layer][i, j];
                    if (currCell == null) converted[i, j] = EntityTypeEnum.None;
                    else
                        converted[i, j] = EntityType.TranslateEntityEnumAndType(currCell.GetType());
                }
            }
            return converted;
        }
    }
}
