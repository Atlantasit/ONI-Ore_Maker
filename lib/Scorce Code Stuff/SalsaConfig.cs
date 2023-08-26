// Decompiled with JetBrains decompiler
// Type: SalsaConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AD882E55-D8AC-4937-9773-540EE16F428F
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class SalsaConfig : IEntityConfig
{
  public const string ID = "Salsa";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("Salsa", (string) ITEMS.FOOD.SALSA.NAME, (string) ITEMS.FOOD.SALSA.DESC, 1f, false, Assets.GetAnim((HashedString) "zestysalsa_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, true), TUNING.FOOD.FOOD_TYPES.SALSA);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
