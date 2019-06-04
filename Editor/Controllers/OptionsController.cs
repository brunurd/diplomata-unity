using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;

namespace LavaLeak.Diplomata.Editor.Controllers
{
  public static class OptionsController
  {
    public static Options GetOptions()
    {
      JSONHelper.CreateFolder("Diplomata/");
      if (!JSONHelper.Exists("preferences", "Diplomata/"))
      {
        JSONHelper.Create(new Options(), "preferences", false, "Diplomata/");
      }
      return JSONHelper.Read<Options>("preferences", "Diplomata/");
    }

    public static void Save(Options options, bool prettyPrint = false)
    {
      JSONHelper.Update(options, "preferences", prettyPrint, "Diplomata/");
    }
  }
}
