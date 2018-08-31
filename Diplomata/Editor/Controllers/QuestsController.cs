using System.Collections.Generic;
using Diplomata.Editor.Helpers;
using Diplomata.Models;
using Diplomata.Models.Collections;

namespace Diplomata.Editor.Controllers
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
