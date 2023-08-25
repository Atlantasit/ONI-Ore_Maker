using TUNING;
using UnityEngine;

namespace OreMakerConfig
{
    public class OreMakerConfig : IBuildingConfig
    {
        public const float OreStorage_Capacity = 550f;
        public const float Emit_Mass = 500f;
        public const float WaterStorage_Capacity = 250f;
        public const float MetalStorage_Capacity = 500f;
        public const float OxyStorage_Capacity = 10f;
        public const float Derefine_Mass_Per_Second = 5f;

        public const string ID = "OreMaker";


        public override BuildingDef CreateBuildingDef()
        {
            int width = 4;
            int height = 3;
            string anim = "oremaker_kanim";
            int hitpoints = 25;
            float construction_time = 30f;
            float[] tieR3_1 = BUILDING:CONSTRUCTION_MASS_KG.TIER3;
            string[] allMetals = MATERIALS.ALL_METALS;
            float melting_point = 800f;
            BuildLocationRule buidingLocationRule = BuildLocationRule.OnFloor;
            EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues tieR3_3 = BUILDINGS.DECOR.PENALTY.TIER3;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("OreMaker", width, height, anim, hitpoints, construction_time, tieR3_1, allMetals, melting_point, buidingLocationRule, tieR3_3, tieR3_2);
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.EnergyConsumptionWhenActive = 480f;
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.UtilityInputOffset = new CellOffset(-1, 2);
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.ViewMode = OverlayModes.TileMode.ID;
            buildingDef.ModifiesTemperature = false;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent.< KPrefabID > ().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
        }
    }
}
