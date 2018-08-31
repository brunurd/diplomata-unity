using Diplomata.Editor.Helpers;
using Diplomata.Models;
using Diplomata.Models.Collections;

namespace Diplomata.Editor.Controllers
{
  public static class InventoryController
  {
    public static Inventory GetInventory(bool prettyPrint = false)
    {
      JSONHelper.CreateFolder("Diplomata/");
      if (!JSONHelper.Exists("inventory", "Diplomata/"))
      {
        JSONHelper.Create(new Inventory(), "inventory", prettyPrint, "Diplomata/");
      }
      return JSONHelper.Read<Inventory>("inventory", "Diplomata/");
    }

    public static void Save(Inventory inventory, bool prettyPrint = false)
    {
      JSONHelper.Update(inventory, "inventory", prettyPrint, "Diplomata/");
    }
  }
}
