using HarmonyLib;

namespace Ore_Maker
{
    internal class OreMakerPatch
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadingGenerratedBuilding")]
        public class ImplementationPatch
        {
            public static LocString Name = new LocString("Ore Maker", "STRINGS.BUILDUNGS.PREFABS." + "OreMaker".ToUpper() + ".NAME");

            public static LocString DESC = new LocString("Used to make Refined Metals back into its Ore form", "STRINGS.BUILDINGS.PREFABS." + "HeliumExtractor".ToUpper() + ".DESC");
        
            public static LocString EFFECT = new LocString( UI.FormatAsLink("Water") + " with " + UI.FormatAsLink("Oxygen") + " and an sprikle of some " + UI.FormatAsLink("Iron") + " and out comes some nice and hot " UI.FormatAsLink("IronOre"));
        }
    }
}
