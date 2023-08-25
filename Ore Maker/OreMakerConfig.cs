using Ore_Maker;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using static STRINGS.DUPLICANTS.CHORES;

namespace OreMakerConfig
{
    public class OreMakerConfig : IBuildingConfig
    {   
        public const string ID = "OreMaker";
        public const float OreStorage_Capacity = 550f;
        public const float Emit_Mass = 500f;
        public const float WaterStorage_Capacity = 250f;
        public const float OxyStorage_Capacity = 10f;

        


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
            buildingDef.SelfHeatKilowattsWhenActive = 10f;
            buildingDef.ViewMode = OverlayModes.TileMode.ID;
            buildingDef.ModifiesTemperature = false;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent.< KPrefabID > ().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
            go.AddOrGet<OreMaker.OreMaker>().overpressureMass = 10f;
            OreMaker fabricator = go.AddOrGet<OreMaker>();

            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas; 
            conduitConsumer.capacityKG = OxyStorage_Capacity;
            conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            conduitConsumer.alwaysConsume = false; // look at this maybe 

            ConduitConsumer conduitConsumer1 = go.AddOrGet<ConduitConsumer>();
            conduitConsumer1.conduitType = ConduitType.Liquid;
            conduitConsumer1.capacityKG = WaterStorage_Capacity;
            conduitConsumer1.capacityTag = ElementLoader.FindElementByHash (SimHashes.Water).tag;
            conduitConsumer1.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            conduitConsumer1.alwaysConsume = false; // look at this maybe 

            //  \|/ May need some Work \|/
            fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            go.AddOrGet<ComplexFabricatorWorkable>();

            this.ConfigureRecipes();
            Prioritizable.AddRef(go);
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<PoweredActiveStoppableController.Def>();
            go.GetComponent<KPrefabID>().prefabSpawnFn += (KPrefabID.PrefabFn)(game_object =>
            {
                ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
                component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
                component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
                component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
                component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
            });

        }
            public void ConfigureRecipes()
            
        {       // Iron Ore Recipe
              ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[2]
              {
                new ComplexRecipe.RecipeElement((Tag) "GrilledPrickleFruit", 2f),
                new ComplexRecipe.RecipeElement((Tag) SpiceNutConfig.ID, 2f)
              };
              ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
              {
                new ComplexRecipe.RecipeElement((Tag) "Salsa", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
              };

              SalsaConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", (IList<ComplexRecipe.RecipeElement>)recipeElementArray1, (IList<ComplexRecipe.RecipeElement>)recipeElementArray2), recipeElementArray1, recipeElementArray2)
              {
                time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME,
                description = (string)ITEMS.FOOD.SALSA.RECIPEDESC,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag>()
                 {
                     (Tag) "GourmetCookingStation"
                 },
                sortOrder = 300
              };
            
                if (!DlcManager.IsExpansion1Active())
                 return;


        }
    }
}
