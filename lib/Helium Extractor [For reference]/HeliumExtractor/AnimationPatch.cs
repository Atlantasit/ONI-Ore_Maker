// Decompiled with JetBrains decompiler
// Type: HeliumExtractor.AnimationPatch
// Assembly: HeliumExtractor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6703BEC-AB0A-4945-98AA-05D561122F4E
// Assembly location: C:\Users\Marx_Admin\Documents\Daten\Github\Oni Mods\.Stuff\Helim maker [for reference]\HeliumExtractor.dll

using HarmonyLib;

namespace HeliumExtractor
{
  [HarmonyPatch(typeof (OilRefinery.WorkableTarget))]
  [HarmonyPatch("OnPrefabInit")]
  public class AnimationPatch
  {
    public static void Postfix(OilRefinery.WorkableTarget __instance)
    {
      if (!(__instance.name == "HeliumExtractorComplete"))
        return;
      __instance.overrideAnims = new KAnimFile[1]
      {
        Assets.GetAnim((HashedString) "anim_interacts_metalrefinery_kanim")
      };
    }
  }
}
