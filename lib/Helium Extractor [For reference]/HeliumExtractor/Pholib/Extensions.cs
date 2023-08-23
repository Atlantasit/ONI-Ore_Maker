// Decompiled with JetBrains decompiler
// Type: Pholib.Extensions
// Assembly: HeliumExtractor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6703BEC-AB0A-4945-98AA-05D561122F4E
// Assembly location: C:\Users\Marx_Admin\Documents\Daten\Github\Oni Mods\.Stuff\Helim maker [for reference]\HeliumExtractor.dll

using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Pholib
{
  public static class Extensions
  {
    public static void RemoveDef<DefType>(this GameObject go) where DefType : StateMachine.BaseDef
    {
      StateMachineController machineController = go.AddOrGet<StateMachineController>();
      DefType def = machineController.GetDef<DefType>();
      if ((object) def == null)
        return;
      def.Configure((GameObject) null);
      machineController.cmpdef.defs.Remove((StateMachine.BaseDef) def);
    }

    public static string Dump(this object obj) => new SerializerBuilder().WithNamingConvention((INamingConvention) new CamelCaseNamingConvention()).Build().Serialize(obj);
  }
}
