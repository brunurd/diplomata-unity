using Diplomata.Editor.Helpers;
using Diplomata.Models;
using Diplomata.Models.Collections;

namespace Diplomata.Editor.Controllers
{
  public static class GlobalFlagsController
  {
    public static GlobalFlags GetGlobalFlags(bool prettyPrint = false)
    {
      JSONHelper.CreateFolder("Diplomata/");

      // Rename legacy custom flags files.
      if (JSONHelper.Exists("customFlags", "Diplomata/"))
      {
        var globalFlags = JSONHelper.Read<GlobalFlags>("customFlags", "Diplomata/");
        JSONHelper.Create(globalFlags, "globalFlags", prettyPrint, "Diplomata/");
        JSONHelper.Delete("customFlags", "Diplomata/");
      }

      // Create global flags file if don't exists.
      if (!JSONHelper.Exists("globalFlags", "Diplomata/"))
      {
        JSONHelper.Create(new GlobalFlags(), "globalFlags", prettyPrint, "Diplomata/");
      }

      // Return the global flags file.
      return JSONHelper.Read<GlobalFlags>("globalFlags", "Diplomata/");
    }

    public static void Save(GlobalFlags globalFlags, bool prettyPrint = false)
    {
      JSONHelper.Update(globalFlags, "globalFlags", prettyPrint, "Diplomata/");
    }
  }
}
