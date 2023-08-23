// Decompiled with JetBrains decompiler
// Type: Pholib.Utilities
// Assembly: HeliumExtractor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6703BEC-AB0A-4945-98AA-05D561122F4E
// Assembly location: C:\Users\Marx_Admin\Documents\Daten\Github\Oni Mods\.Stuff\Helim maker [for reference]\HeliumExtractor.dll

using HarmonyLib;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pholib
{
  public class Utilities
  {
    private static readonly List<System.Type> alreadyLoaded = new List<System.Type>();

    public static bool IsSurfaceDiscovered() => Game.Instance.savedInfo.discoveredSurface;

    public static bool IsOilFieldDiscovered() => Game.Instance.savedInfo.discoveredOilField;

    public static bool IsTagDiscovered(string tag) => DiscoveredResources.Instance.IsDiscovered((Tag) tag);

    public static bool IsSimHashesDiscovered(SimHashes hash) => DiscoveredResources.Instance.IsDiscovered(ElementLoader.FindElementByHash(hash).tag);

    public static bool CycleCondition(int cycle) => GameClock.Instance.GetCycle() >= cycle;

    public static bool CycleInRange(int minimum, int maximum) => GameClock.Instance.GetCycle() >= minimum && GameClock.Instance.GetCycle() <= maximum;

    public static bool IsOnCluster(string clusterName)
    {
      if ((UnityEngine.Object) CustomGameSettings.Instance == (UnityEngine.Object) null)
        return false;
      Dictionary<string, string> dictionary = Traverse.Create((object) CustomGameSettings.Instance).Field<Dictionary<string, string>>("CurrentQualityLevelsBySetting").Value;
      return dictionary != null && dictionary["ClusterLayout"] != null && dictionary["ClusterLayout"].Replace("clusters/", "") == clusterName;
    }

    public static void LoadTranslations(System.Type locStringRoot, string modPath, string translationsDir = "translations")
    {
      Localization.RegisterForTranslation(locStringRoot);
      Localization.Locale locale = Localization.GetLocale();
      if (locale == null)
        return;
      if (string.IsNullOrEmpty(modPath))
      {
        Debug.LogError((object) "modPath is empty");
      }
      else
      {
        string path = System.IO.Path.Combine(System.IO.Path.Combine(modPath, translationsDir ?? ""), locale.Code + ".po");
        Debug.Log((object) string.Format("Loading translation file for {0} ({1}) language: '{2}'", (object) locale.Lang, (object) locale.Code, (object) path));
        if (!File.Exists(path))
        {
          Debug.LogWarning((object) ("Translation file not found: '" + path + "'"));
        }
        else
        {
          try
          {
            Localization.OverloadStrings(Localization.LoadStringsFile(path, false));
          }
          catch (Exception ex)
          {
            Debug.LogError((object) ("Unexpected error while loading translation file: '" + path + "'"));
            Debug.LogError((object) ex);
          }
        }
      }
    }

    public static void AddCarePackage(
      ref Immigration immigration,
      string objectId,
      float amount,
      Func<bool> requirement = null)
    {
      Traverse traverse = Traverse.Create((object) immigration).Field("carePackages");
      List<CarePackageInfo> list = ((IEnumerable<CarePackageInfo>) traverse.GetValue<CarePackageInfo[]>()).ToList<CarePackageInfo>();
      list.Add(new CarePackageInfo(objectId, amount, requirement));
      traverse.SetValue((object) list.ToArray());
    }

    public static void AddWorldYaml(System.Type className)
    {
      if (Utilities.alreadyLoaded.Contains(className))
        return;
      ModUtil.RegisterForTranslation(className);
      Utilities.alreadyLoaded.Add(className);
    }

    public static void AddBuilding(
      string category,
      string id,
      string name,
      string desc,
      string effect)
    {
      string upperInvariant = id.ToUpperInvariant();
      Strings.Add("STRINGS.BUILDINGS.PREFABS." + upperInvariant + ".NAME", UI.FormatAsLink(name, id));
      Strings.Add("STRINGS.BUILDINGS.PREFABS." + upperInvariant + ".DESC", desc);
      Strings.Add("STRINGS.BUILDINGS.PREFABS." + upperInvariant + ".EFFECT", effect);
      ModUtil.AddBuildingToPlanScreen((HashedString) category, id);
    }

    public static void AddBuildingTech(string techId, string buildingId) => Db.Get().Techs.Get(techId).unlockedItemIDs.Add(buildingId);

    public static ComplexRecipe AddComplexRecipe(
      ComplexRecipe.RecipeElement[] input,
      ComplexRecipe.RecipeElement[] output,
      string fabricatorId,
      float productionTime,
      LocString recipeDescription,
      ComplexRecipe.RecipeNameDisplay nameDisplayType,
      int sortOrder,
      string requiredTech = null)
    {
      return new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(fabricatorId, (IList<ComplexRecipe.RecipeElement>) input, (IList<ComplexRecipe.RecipeElement>) output), input, output)
      {
        time = productionTime,
        description = (string) recipeDescription,
        nameDisplay = nameDisplayType,
        fabricators = new List<Tag>() { (Tag) fabricatorId },
        sortOrder = sortOrder,
        requiredTech = requiredTech
      };
    }
  }
}
