// Decompiled with JetBrains decompiler
// Type: HeliumExtractor.HeliumExtractorConfig
// Assembly: HeliumExtractor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6703BEC-AB0A-4945-98AA-05D561122F4E
// Assembly location: C:\Users\Marx_Admin\Documents\Daten\Github\Oni Mods\.Stuff\Helim maker [for reference]\HeliumExtractor.dll

using TUNING;
using UnityEngine;

namespace HeliumExtractor
{
  public class HeliumExtractorConfig : IBuildingConfig
  {
    private readonly ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-1, 2));
    public static float totalConversion = 0.5f;
    public static float heliumConversionRate = 0.025f;
    public static float sulfureConversionRate = 0.025f;
    public static float propaneConversionRate = HeliumExtractorConfig.totalConversion - HeliumExtractorConfig.heliumConversionRate - HeliumExtractorConfig.sulfureConversionRate;
    public const string ID = "HeliumExtractor";

    public override BuildingDef CreateBuildingDef()
    {
      int width = 3;
      int height = 4;
      string anim = "helium_extractor_kanim";
      int hitpoints = 30;
      float construction_time = 30f;
      float[] tieR3_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
      string[] allMetals = MATERIALS.ALL_METALS;
      float melting_point = 800f;
      BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
      EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
      BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("HeliumExtractor", width, height, anim, hitpoints, construction_time, tieR3_1, allMetals, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER2, tieR3_2);
      buildingDef.RequiresPowerInput = true;
      buildingDef.PowerInputOffset = new CellOffset(0, 0);
      buildingDef.EnergyConsumptionWhenActive = 420f;
      buildingDef.SelfHeatKilowattsWhenActive = 12f;
      buildingDef.AudioCategory = "HollowMetal";
      buildingDef.ViewMode = OverlayModes.GasConduits.ID;
      buildingDef.InputConduitType = ConduitType.Gas;
      buildingDef.OutputConduitType = ConduitType.Gas;
      buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
      buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
      buildingDef.ModifiesTemperature = false;
      return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
      go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
      go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
      go.AddOrGet<HeliumExtractor.HeliumExtractor>().overpressureMass = 10f;
      ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
      conduitConsumer.conduitType = ConduitType.Gas;
      conduitConsumer.consumptionRate = HeliumExtractorConfig.totalConversion;
      conduitConsumer.capacityTag = SimHashes.Methane.CreateTag();
      conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
      conduitConsumer.capacityKG = 10f;
      conduitConsumer.forceAlwaysSatisfied = true;
      ConduitDispenser conduitDispenser1 = go.AddOrGet<ConduitDispenser>();
      conduitDispenser1.conduitType = ConduitType.Gas;
      conduitDispenser1.invertElementFilter = true;
      conduitDispenser1.elementFilter = new SimHashes[2]
      {
        SimHashes.Methane,
        SimHashes.Propane
      };
      ConduitDispenser conduitDispenser2 = go.AddComponent<ConduitDispenser>();
      conduitDispenser2.conduitType = ConduitType.Gas;
      conduitDispenser2.invertElementFilter = true;
      conduitDispenser2.elementFilter = new SimHashes[2]
      {
        SimHashes.Methane,
        SimHashes.Helium
      };
      conduitDispenser2.useSecondaryOutput = true;
      go.AddOrGet<ConduitSecondaryOutput>().portInfo = this.secondaryPort;
      go.AddOrGet<Storage>().showInUI = true;
      ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
      elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
      {
        new ElementConverter.ConsumedElement(SimHashes.Methane.CreateTag(), HeliumExtractorConfig.totalConversion)
      };
      elementConverter.outputElements = new ElementConverter.OutputElement[3]
      {
        new ElementConverter.OutputElement(HeliumExtractorConfig.heliumConversionRate, SimHashes.Sulfur, 350.15f),
        new ElementConverter.OutputElement(HeliumExtractorConfig.heliumConversionRate, SimHashes.Helium, 80.15f, storeOutput: true),
        new ElementConverter.OutputElement(HeliumExtractorConfig.propaneConversionRate, SimHashes.Propane, 81.15f, storeOutput: true)
      };
      Prioritizable.AddRef(go);
    }

    public override void DoPostConfigureComplete(GameObject go) => this.AttachPort(go);

    public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => this.AttachPort(go);

    public override void DoPostConfigureUnderConstruction(GameObject go) => this.AttachPort(go);

    private void AttachPort(GameObject go) => go.AddOrGet<ConduitSecondaryOutput>().portInfo = this.secondaryPort;
  }
}
