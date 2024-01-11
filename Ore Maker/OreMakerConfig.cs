using Database;
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
                    int     width               = 4;
                    int     height              = 3;
                    string  anim                = "oremaker_kanim"; //not yet made
                    int     hitpoints           = 25;
                    float   construction_time   = 30f;
                    float[] construction_mass   = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3; 
                    string[] allMetals          = MATERIALS.ALL_METALS;
                    float   melting_point       = 800f;

                    BuildLocationRule buidingLocationRule   = BuildLocationRule.OnFloor;
                    EffectorValues noise_penalty            = NOISE_POLLUTION.NOISY.TIER2;
                    EffectorValues deco_penalty             = DECOR.PENALTY.TIER3; 

                    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(//Implementing the buildingdef values
                        "OreMaker", 
                        width, 
                        height, 
                        anim, 
                        hitpoints, 
                        construction_time, 
                        construction_mass, 
                        allMetals, melting_point, 
                        buidingLocationRule, 
                        deco_penalty, 
                        noise_penalty
                        );
                    
                    buildingDef.RequiresPowerInput          = true;
                    buildingDef.PowerInputOffset            = new CellOffset(0, 0);
                    buildingDef.EnergyConsumptionWhenActive = 480f;
                    buildingDef.InputConduitType            = ConduitType.Gas;
                    buildingDef.UtilityInputOffset          = new CellOffset(-1, 2);
                    buildingDef.InputConduitType            = ConduitType.Liquid;
                    buildingDef.UtilityInputOffset          = new CellOffset(-1, 0);
                    buildingDef.AudioCategory               = "HollowMetal";
                    buildingDef.SelfHeatKilowattsWhenActive = 10f;
                    buildingDef.ViewMode                    = OverlayModes.TileMode.ID;
                    buildingDef.ModifiesTemperature         = false;// for now
                    return buildingDef;
                }


        //-----Templateb Building---------------------------------

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {   
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);    //defines that the building is IndustrialMachinery
            go.AddOrGet<BuildingComplete>().isManuallyOperated = true;                                  //makes that its operrated by a duplicant
            go.AddOrGet<Ore_Maker.OreMaker>().overpressureMass = 10f;                                   //when too mutch pressure is present the building will stop working
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
            OreMaker.fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
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
        // \|/Need's work too
        public void ConfigureRecipes()
        {
            // Iron Ore Recipe
            ComplexRecipe.RecipeElement[] recipeElementArrayIron1 = new ComplexRecipe.RecipeElement[3]
            {
                new ComplexRecipe.RecipeElement(material: "Iron".ToTag(), 95f),
                new ComplexRecipe.RecipeElement("Oxygen".ToTag(), 50f),
                new ComplexRecipe.RecipeElement("Water".ToTag(), 10f)
              };
            ComplexRecipe.RecipeElement[] recipeElementArrayIron2 = new ComplexRecipe.RecipeElement[1]
            {
                new ComplexRecipe.RecipeElement((Tag) "IronOre", 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
            };

            IronOre.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("OreMaker", (IList<ComplexRecipe.RecipeElement>)recipeElementArrayIron1, (IList<ComplexRecipe.RecipeElement>)recipeElementArrayIron2), recipeElementArrayIron1, recipeElementArrayIron2)
            {
                time = 30f,
                description = "IronOre",
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag>()
                 {
                     (Tag) "OreMaker"
                 },
                sortOrder = 300
            };

            // Copper Ore Recipe
            ComplexRecipe.RecipeElement[] recipeElementArrayCopper1 = new ComplexRecipe.RecipeElement[3]
            {
                new ComplexRecipe.RecipeElement(material: "Copper".ToTag(), 95f),
                new ComplexRecipe.RecipeElement("Oxygen".ToTag(), 50f),
                new ComplexRecipe.RecipeElement("Water".ToTag(), 10f)
              };
            ComplexRecipe.RecipeElement[] recipeElementArrayCopper2 = new ComplexRecipe.RecipeElement[1]
            {
                new ComplexRecipe.RecipeElement((Tag) "CopperOre", 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
            };

            CopperOre.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("OreMaker", (IList<ComplexRecipe.RecipeElement>)recipeElementArrayCopper1, (IList<ComplexRecipe.RecipeElement>)recipeElementArrayCopper2), recipeElementArrayCopper1, recipeElementArrayCopper2)
            {
                time = 30f,
                description = "CopperOre",
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag>()
                 {
                     (Tag) "OreMaker"
                 },
                sortOrder = 300
            };
            /*
            if (!DlcManager.IsExpansion1Active())
                return;
            */


        }
    }
}
