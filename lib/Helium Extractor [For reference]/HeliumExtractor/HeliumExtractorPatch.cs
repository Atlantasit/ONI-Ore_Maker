// Decompiled with JetBrains decompiler
// Type: HeliumExtractor.HeliumExtractorPatch
// Assembly: HeliumExtractor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6703BEC-AB0A-4945-98AA-05D561122F4E
// Assembly location: C:\Users\Marx_Admin\Documents\Daten\Github\Oni Mods\.Stuff\Helim maker [for reference]\HeliumExtractor.dll

using HarmonyLib;
using Pholib;
using STRINGS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeliumExtractor
{
  internal class HeliumExtractorPatch
  {
    [HarmonyPatch(typeof (GeneratedBuildings))]
    [HarmonyPatch("LoadGeneratedBuildings")]
    public class ImplementationPatch
    {
      public static LocString NAME = new LocString("Helium Extractor", "STRINGS.BUILDINGS.PREFABS." + "HeliumExtractor".ToUpper() + ".NAME");
      public static LocString DESC = new LocString("Helium can only be produced from the extration of Natural gas.", "STRINGS.BUILDINGS.PREFABS." + "HeliumExtractor".ToUpper() + ".DESC");
      public static LocString EFFECT = new LocString("Transforms " + UI.FormatAsLink("natural gas", "METHANE") + " into " + UI.FormatAsLink("helium", "HELIUM") + ", " + UI.FormatAsLink("propane", "PROPANE") + " and " + UI.FormatAsLink("sulfur", "SULFUR") + ".\n\n" + UI.FormatAsLink("Helium", "HELIUM") + " is a useful gas with interesting physical properties.\nPropane can be used the same way than Natural gas.", "STRINGS.BUILDINGS.PREFABS." + "HeliumExtractor".ToUpper() + ".EFFECT");

      private static void Prefix()
      {
        Strings.Add(HeliumExtractorPatch.ImplementationPatch.NAME.key.String, HeliumExtractorPatch.ImplementationPatch.NAME.text);
        Strings.Add(HeliumExtractorPatch.ImplementationPatch.DESC.key.String, HeliumExtractorPatch.ImplementationPatch.DESC.text);
        Strings.Add(HeliumExtractorPatch.ImplementationPatch.EFFECT.key.String, HeliumExtractorPatch.ImplementationPatch.EFFECT.text);
        ModUtil.AddBuildingToPlanScreen((HashedString) "Refining", "HeliumExtractor");
      }
    }

    [HarmonyPatch(typeof (Db), "Initialize")]
    public class DatabaseAddingPatch
    {
      public static void Postfix() => Utilities.AddBuildingTech("HighTempForging", "HeliumExtractor");
    }

    [HarmonyPatch(typeof (ElementLoader), "CopyEntryToElement")]
    public class PropaneCombustibleAdder
    {
      public static void Postfix(Element elem)
      {
        if (elem.id != SimHashes.Propane)
          return;
        IEnumerable source = (IEnumerable) ((IEnumerable<Tag>) elem.oreTags).AddItem<Tag>(GameTags.CombustibleGas);
        elem.oreTags = source.Cast<Tag>().ToArray<Tag>();
      }
    }

    [HarmonyPatch(typeof (ElementLoader), "CopyEntryToElement")]
    public class HeliumEnablePatch
    {
      public static void Postfix(Element elem)
      {
        if (elem.id != SimHashes.Helium && elem.id != SimHashes.LiquidHelium && elem.id != SimHashes.Propane && elem.id != SimHashes.LiquidPropane && elem.id != SimHashes.SolidPropane)
          return;
        elem.disabled = false;
      }
    }

    [HarmonyPatch(typeof (MethaneGeneratorConfig), "DoPostConfigureComplete")]
    public class MethaneGeneratorPropanePatch
    {
      public static void Postfix(GameObject go)
      {
        go.AddOrGet<EnergyGenerator>().formula = new EnergyGenerator.Formula()
        {
          inputs = new EnergyGenerator.InputItem[1]
          {
            new EnergyGenerator.InputItem(GameTags.CombustibleGas, 0.09f, 0.900000036f)
          },
          outputs = new EnergyGenerator.OutputItem[2]
          {
            new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.0675f, false, new CellOffset(1, 1), 313.15f),
            new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.0225f, true, new CellOffset(0, 2), 383.15f)
          }
        };
        ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
        conduitDispenser.elementFilter = ((IEnumerable<SimHashes>) conduitDispenser.elementFilter).AddItem<SimHashes>(SimHashes.Propane).ToArray<SimHashes>();
      }
    }

    [HarmonyPatch(typeof (GourmetCookingStationConfig), "ConfigureBuildingTemplate")]
    public class GourmetCookingStationPropanePatch
    {
      public static void Prefix(GourmetCookingStationConfig __instance, GameObject go) => Traverse.Create((object) __instance).Field("FUEL_TAG").SetValue((object) GameTags.CombustibleGas);
    }

    [HarmonyPatch(typeof (ComplexFabricator), "DropExcessIngredients")]
    public class GourmetCookingStationPropanePatch2
    {
      public static bool Prefix(ComplexFabricator __instance, Storage storage)
      {
        ComplexRecipe[] complexRecipeArray = Traverse.Create((object) __instance).Field<ComplexRecipe[]>("recipe_list").Value;
        HashSet<Tag> tagSet = new HashSet<Tag>();
        if (__instance.keepAdditionalTag != Tag.Invalid)
          tagSet.Add(__instance.keepAdditionalTag);
        for (int index = 0; index < complexRecipeArray.Length; ++index)
        {
          ComplexRecipe recipe = complexRecipeArray[index];
          if (__instance.IsRecipeQueued(recipe))
          {
            foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
              tagSet.Add(ingredient.material);
          }
        }
        for (int index = storage.items.Count - 1; index >= 0; --index)
        {
          GameObject go = storage.items[index];
          if (!((Object) go == (Object) null))
          {
            PrimaryElement component1 = go.GetComponent<PrimaryElement>();
            if (!((Object) component1 == (Object) null) && (!__instance.keepExcessLiquids || !component1.Element.IsLiquid))
            {
              KPrefabID component2 = go.GetComponent<KPrefabID>();
              if (!component2.HasTag(__instance.keepAdditionalTag) && (bool) (Object) component2 && !tagSet.Contains(component2.PrefabID()))
                storage.Drop(go, true);
            }
          }
        }
        return false;
      }
    }
  }
}
