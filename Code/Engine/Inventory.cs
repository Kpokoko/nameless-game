using nameless.Entity;
using nameless.Interfaces;
using System.Collections.Generic;

namespace nameless.Engine
{
    public class Inventory
    {
        private Dictionary<EntityTypeEnum, int> _inventory;
        
        public Inventory()
        {
            _inventory = Globals.Serializer.GetInventory();
        }

        public bool TryGetEntity(EntityTypeEnum entity)
        {
            if (Globals.IsDeveloperModeEnabled) return true;
            if (!_inventory.ContainsKey(entity)) return false;
            if (_inventory[entity] > 0)
            {
                _inventory[entity]--;
                return true;
            }
            return false;
        }

        public void AddEntity(EntityTypeEnum entity)
        {
            if (Globals.IsDeveloperModeEnabled) return;
            if (!_inventory.ContainsKey(entity))
                _inventory.Add(entity, 0);
            _inventory[entity]++;
        }

        public Dictionary<EntityTypeEnum, int> GetInventory() { return _inventory; }
    }
}
