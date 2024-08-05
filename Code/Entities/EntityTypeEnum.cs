﻿using nameless.Code.Entities.Blocks;
using nameless.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameless.Entity;

public enum EntityTypeEnum
{
    None,
    Block,
    InventoryBlock,
    EditorBlock,
    HitboxTrigger,
    Platform,
    MovingPlatform,
    FragileBlock
}

public static class EntityType
{
    public static Dictionary<EntityTypeEnum, Type> EntityEnumToTypeTranslator = new Dictionary<EntityTypeEnum, Type> {
        { EntityTypeEnum.None, typeof(void)},
        { EntityTypeEnum.Block, typeof(Block)},
        { EntityTypeEnum.InventoryBlock, typeof(InventoryBlock)},
        { EntityTypeEnum.EditorBlock, typeof(EditorBlock)},
        { EntityTypeEnum.HitboxTrigger, typeof(HitboxTrigger)},
        { EntityTypeEnum.Platform, typeof(Platform)},
        { EntityTypeEnum.MovingPlatform, typeof(MovingPlatform)},
        { EntityTypeEnum.FragileBlock, typeof(FragileBlock)},
    };

    public static Dictionary<Type, EntityTypeEnum> EntityTypeToEnumTranslator = EntityEnumToTypeTranslator
        .ToDictionary(pair => pair.Value, pair => pair.Key);

    public static Type TranslateEntityEnumAndType(EntityTypeEnum entityType) => EntityEnumToTypeTranslator[entityType];

    public static EntityTypeEnum TranslateEntityEnumAndType(Type entityType) => EntityTypeToEnumTranslator[entityType];

}
