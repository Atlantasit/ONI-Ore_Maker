using TUNING;
using UnityEngine;

namespace OreMakerConfig
{
    public class OreMakerConfig : IBuildingConfig
    {
        private readonly ConduitPortInfo conduitPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-1, 2));
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
            int hitpoints = 25;
            float construction_time = 30f;
            float[] tieR3_1 = BUILDING:CONSTRUCTION_MASS_KG.TIER3;
            string[] allMetals = MATERIALS.ALL_METALS;
        }
    }
}
