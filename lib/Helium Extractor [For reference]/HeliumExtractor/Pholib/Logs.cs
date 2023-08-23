// Decompiled with JetBrains decompiler
// Type: Pholib.Logs
// Assembly: HeliumExtractor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6703BEC-AB0A-4945-98AA-05D561122F4E
// Assembly location: C:\Users\Marx_Admin\Documents\Daten\Github\Oni Mods\.Stuff\Helim maker [for reference]\HeliumExtractor.dll

namespace Pholib
{
  public class Logs
  {
    private static readonly string version = "1.3";
    public static bool DebugLog = false;
    private static bool initiated = false;

    public static void InitIfNot()
    {
      if (Logs.initiated)
        return;
      Debug.Log((object) ("== Game Launched with Pholib " + Logs.version + "  " + System.DateTime.Now.ToString()));
      Logs.initiated = true;
    }

    public static void Log(string informations)
    {
      Logs.InitIfNot();
      Debug.Log((object) ("Pholib: " + informations));
    }

    public static void Log(object informations)
    {
      Logs.InitIfNot();
      Debug.Log("Pholib: " + informations?.ToString() == null ? (object) "null" : (object) informations.ToString());
    }

    public static void LogIfDebugging(string informations)
    {
      Logs.InitIfNot();
      if (!Logs.DebugLog)
        return;
      Debug.Log((object) ("Pholib: " + informations));
    }
  }
}
