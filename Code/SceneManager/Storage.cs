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
        private TileGridEntity[][,] Entities = new[] { new TileGridEntity[40, 23] , new TileGridEntity[40, 23] };

        public Storage(List<IEntity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i] as TileGridEntity;
                if (entity is null) continue;
                var layer = entity.Layer;
                var pos = entity.TilePosition;
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

        public int GetLength(int dimension)
        {
            return Entities[0].GetLength(dimension);
        }
    }
}
