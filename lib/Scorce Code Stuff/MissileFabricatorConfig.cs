// Decompiled with JetBrains decompiler
// Type: MissileFabricatorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AD882E55-D8AC-4937-9773-540EE16F428F
// Assembly location: C:\Users\Marx_Admin\Documents\Daten\Github\Oni Mods\ONI-Ore_Maker\lib\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MissileFabricatorConfig : IBuildingConfig
{
  public const string ID = "MissileFabricator";
  public const float MISSILE_FABRICATION_TIME = 80f;
  public const float CO2_PRODUCTION_RATE = 0.0125f;
  private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>()
  {
    Storage.StoredItemModifier.Hide,
    Storage.StoredItemModifier.Preserve,
    Storage.StoredItemModifier.Seal
  };

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] refinedMetals = TUNING.MATERIALS.REFINED_METALS;
    EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
    EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR6;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MissileFabricator", 5, 4, "missile_fabricator_kanim", 250, 60f, tieR5, refinedMetals, 1600f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 960f;
    buildingDef.SelfHeatKilowattsWhenActive = 8f;
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PowerInputOffset = new CellOffset(1, 0);
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.UtilityInputOffset = new CellOffset(-1, 1);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<DropAllWorkable>();
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    fabricator.keepExcessLiquids = true;
    fabricator.allowManualFluidDelivery = false;
    ComplexFabricatorWorkable fabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
    fabricator.duplicantOperated = true;
    BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
    fabricator.storeProduced = false;
    fabricator.inStorage.SetDefaultStoredItemModifiers(MissileFabricatorConfig.RefineryStoredItemModifiers);
    fabricator.buildStorage.SetDefaultStoredItemModifiers(MissileFabricatorConfig.RefineryStoredItemModifiers);
    fabricator.outputOffset = new Vector3(1f, 0.5f);
    KAnimFile[] kanimFileArray = new KAnimFile[1]
    {
      Assets.GetAnim((HashedString) "anim_interacts_missile_fabricator_kanim")
    };
    fabricatorWorkable.overrideAnims = kanimFileArray;
    BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
    buildingElementEmitter.emitRate = 0.0125f;
    buildingElementEmitter.temperature = 313.15f;
    buildingElementEmitter.element = SimHashes.CarbonDioxide;
    buildingElementEmitter.modifierOffset = new Vector2(2f, 2f);
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.capacityTag = GameTags.Liquid;
    conduitConsumer.capacityKG = 100f;
    conduitConsumer.storage = fabricator.inStorage;
    conduitConsumer.alwaysConsume = false;
    conduitConsumer.forceAlwaysSatisfied = true;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Iron).tag, 25f, true),
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Petroleum).tag, 50f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement((Tag) "MissileBasic", 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    string obsolete_id1 = ComplexRecipeManager.MakeObsoleteRecipeID("MissileFabricator", recipeElementArray1[0].material);
    string str1 = ComplexRecipeManager.MakeRecipeID("MissileFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2);
    MissileBasicConfig.recipe = new ComplexRecipe(str1, recipeElementArray1, recipeElementArray2)
    {
      time = 80f,
      nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
      description = string.Format((string) STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION, (object) ITEMS.MISSILE_BASIC.NAME, (object) ElementLoader.GetElement(recipeElementArray1[0].material).name, (object) ElementLoader.GetElement(recipeElementArray1[1].material).name),
      fabricators = new List<Tag>()
      {
        TagManager.Create("MissileFabricator")
      }
    };
    ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id1, str1);
    ComplexRecipe.RecipeElement[] recipeElementArray3 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Copper).tag, 25f, true),
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Petroleum).tag, 50f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray4 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement((Tag) "MissileBasic", 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    string obsolete_id2 = ComplexRecipeManager.MakeObsoleteRecipeID("MissileFabricator", recipeElementArray3[0].material);
    string str2 = ComplexRecipeManager.MakeRecipeID("MissileFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray3, (IList<ComplexRecipe.RecipeElement>) recipeElementArray4);
    MissileBasicConfig.recipe = new ComplexRecipe(str2, recipeElementArray3, recipeElementArray4)
    {
      time = 80f,
      nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
      description = string.Format((string) STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION, (object) ITEMS.MISSILE_BASIC.NAME, (object) ElementLoader.GetElement(recipeElementArray3[0].material).name, (object) ElementLoader.GetElement(recipeElementArray3[1].material).name),
      fabricators = new List<Tag>()
      {
        TagManager.Create("MissileFabricator")
      }
    };
    ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id2, str2);
    ComplexRecipe.RecipeElement[] recipeElementArray5 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Aluminum).tag, 25f, true),
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Petroleum).tag, 50f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray6 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement((Tag) "MissileBasic", 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    string obsolete_id3 = ComplexRecipeManager.MakeObsoleteRecipeID("MissileFabricator", recipeElementArray5[0].material);
    string str3 = ComplexRecipeManager.MakeRecipeID("MissileFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray5, (IList<ComplexRecipe.RecipeElement>) recipeElementArray6);
    MissileBasicConfig.recipe = new ComplexRecipe(str3, recipeElementArray5, recipeElementArray6)
    {
      time = 80f,
      nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
      description = string.Format((string) STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION, (object) ITEMS.MISSILE_BASIC.NAME, (object) ElementLoader.GetElement(recipeElementArray5[0].material).name, (object) ElementLoader.GetElement(recipeElementArray5[1].material).name),
      fabricators = new List<Tag>()
      {
        TagManager.Create("MissileFabricator")
      }
    };
    ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id3, str3);
    if (ElementLoader.FindElementByHash(SimHashes.Cobalt) != null)
    {
      ComplexRecipe.RecipeElement[] recipeElementArray7 = new ComplexRecipe.RecipeElement[2]
      {
        new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Cobalt).tag, 25f, true),
        new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Petroleum).tag, 50f)
      };
      ComplexRecipe.RecipeElement[] recipeElementArray8 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement((Tag) "MissileBasic", 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
      };
      string obsolete_id4 = ComplexRecipeManager.MakeObsoleteRecipeID("MissileFabricator", recipeElementArray7[0].material);
      string str4 = ComplexRecipeManager.MakeRecipeID("MissileFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray7, (IList<ComplexRecipe.RecipeElement>) recipeElementArray8);
      MissileBasicConfig.recipe = new ComplexRecipe(str4, recipeElementArray7, recipeElementArray8)
      {
        time = 80f,
        nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
        description = string.Format((string) STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION, (object) ITEMS.MISSILE_BASIC.NAME, (object) ElementLoader.GetElement(recipeElementArray7[0].material).name, (object) ElementLoader.GetElement(recipeElementArray7[1].material).name),
        fabricators = new List<Tag>()
        {
          TagManager.Create("MissileFabricator")
        }
      };
      ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id4, str4);
    }
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go) => go.GetComponent<KPrefabID>().prefabSpawnFn += (KPrefabID.PrefabFn) (game_object =>
  {
    ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
    component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
    component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
    component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
    component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    component.requiredSkillPerk = Db.Get().SkillPerks.CanMakeMissiles.Id;
  });
}
