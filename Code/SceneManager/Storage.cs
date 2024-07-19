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
        private IEntity[,] Entities = new IEntity[40, 23];

        public Storage(List<IEntity> entities)
        {
            foreach (var entity in entities)
            {
                var pos = entity.TilePosition;
                Entities[(int)pos.X, (int)pos.Y] = entity;
                if (entity is EditorBlock)
                    Entities[(int)pos.X, (int)pos.Y - 1] = entity;
            }
        }

        public IEntity this[int index, int index2]
        {
            get { return Entities[index, index2]; }
            set { Entities[index, index2] = value; }
        }
    }
}
