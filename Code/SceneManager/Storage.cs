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
        public IEntity[,] Entities = new IEntity[40, 23];

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
    }
}
