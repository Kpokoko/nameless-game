using Microsoft.Xna.Framework.Input;
using nameless.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.UI;

public class ConstructorButton : Button
{
    public EntityTypeEnum EntityTypeSelect;
    public ConstructorButton(Vector2 position, float width, float height, EntityTypeEnum typeEnum) : base(position, width, height, EntityType.TranslateEntityEnumAndString(typeEnum), ButtonActivationProperty.Switch)
    {
        EntityTypeSelect = typeEnum;
        OnClickEvent += () => Globals.Constructor.SelectedEntity = EntityTypeSelect;
    }
}
