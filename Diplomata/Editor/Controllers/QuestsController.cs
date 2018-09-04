using System.Collections.Generic;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Models;
using LavaLeak.Diplomata.Models.Collections;

namespace LavaLeak.Diplomata.Editor.Controllers
{
  public static class QuestsController
  {
    public static Quest[] GetQuests(bool prettyPrint = false)
    {
      JSONHelper.CreateFolder("Diplomata/");
      if (!JSONHelper.Exists("quests", "Diplomata/"))
      {
        JSONHelper.Create(new Quests(), "quests", prettyPrint, "Diplomata/");
      }
      return JSONHelper.Read<Quests>("quests", "Diplomata/").GetQuests();
    }

    public static void Save(Quest[] quests, bool prettyPrint = false)
    {
      JSONHelper.Update(new Quests(quests), "quests", prettyPrint, "Diplomata/");
    }
  }
}
