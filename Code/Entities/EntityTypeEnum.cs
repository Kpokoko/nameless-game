using nameless.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nameless.Entity;

namespace nameless.Entity;

/// <summary>
/// EnumTypes that are corresponding to type names
/// </summary>
public enum EntityTypeEnum 
{
    None,
    Block,
    InventoryBlock,
    StickyBlock,
    EditorBlock,
    Pivot,
    Platform,
    MovingPlatform,
    FragileBlock,
    TemporaryBlock,
    DelayedDeathBlock,
    RayCaster,
    Attacher
}

public static class EntityType
{
    public static void ParseEntityTypeEnum()
    {
        var entityTypes = Enum.GetNames(typeof(EntityTypeEnum));
        EntityEnumToTypeTranslator[EntityTypeEnum.None] = typeof(void);
        foreach (var e in entityTypes)
        {
            if (e == "None") { continue; }
            var typeEnum = (EntityTypeEnum) Enum.Parse(typeof(EntityTypeEnum), e);
            EntityEnumToTypeTranslator[typeEnum] = Type.GetType("nameless.Entity." + e);
            EntityEnumToStringTranslator[typeEnum] = e;
        }
        EntityTypeToEnumTranslator = EntityEnumToTypeTranslator
            .ToDictionary(pair => pair.Value, pair => pair.Key);

        EntitySringToEnumTranslator = EntityEnumToStringTranslator
            .ToDictionary(pair => pair.Value, pair => pair.Key);
    }

    private static Dictionary<EntityTypeEnum, Type> EntityEnumToTypeTranslator = new Dictionary<EntityTypeEnum, Type> {
        //{ EntityTypeEnum.None, null},
        //{ EntityTypeEnum.Block, typeof(Block)},
        //{ EntityTypeEnum.InventoryBlock, typeof(InventoryBlock)},
        //{ EntityTypeEnum.StickyBlock, typeof(StickyBlock)},
        //{ EntityTypeEnum.EditorBlock, typeof(EditorBlock)},
        //{ EntityTypeEnum.HitboxTrigger, typeof(HitboxTrigger)},
        //{ EntityTypeEnum.Platform, typeof(Platform)},
        //{ EntityTypeEnum.MovingPlatform, typeof(MovingPlatform)},
        //{ EntityTypeEnum.FragileBlock, typeof(FragileBlock)},
    };

    private static Dictionary<Type, EntityTypeEnum> EntityTypeToEnumTranslator;

    private static Dictionary<EntityTypeEnum, string> EntityEnumToStringTranslator = new();

    private static Dictionary<string, EntityTypeEnum> EntitySringToEnumTranslator;


    public static Type TranslateEntityEnumAndType(EntityTypeEnum entityType) => EntityEnumToTypeTranslator[entityType];

    public static EntityTypeEnum TranslateEntityEnumAndType(Type entityType) => EntityTypeToEnumTranslator[entityType];


    public static string TranslateEntityEnumAndString(EntityTypeEnum entityType) => EntityEnumToStringTranslator[entityType];

    public static EntityTypeEnum TranslateEntityEnumAndString(string entityType) => EntitySringToEnumTranslator[entityType];

}
