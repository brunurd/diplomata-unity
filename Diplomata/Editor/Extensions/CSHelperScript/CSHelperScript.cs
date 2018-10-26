using LavaLeak.Diplomata.Editor.Controllers;
using UnityEditor;

namespace LavaLeak.Diplomata.Editor.Extensions
{
  public class CSHelperScript
  {
    [MenuItem("Tools/Diplomata/Export To.../C# Helper Script", false, 2)]
    public static void GenerateScript()
    {
      var options = OptionsController.GetOptions();
      var characters = CharactersController.GetCharacters(options);
      var interactables = InteractablesController.GetInteractables(options);
      var globalFlags = GlobalFlagsController.GetGlobalFlags(options.jsonPrettyPrint);
      var inventory = InventoryController.GetInventory(options.jsonPrettyPrint);
      var quests = QuestsController.GetQuests(options.jsonPrettyPrint);
    }
  }
}