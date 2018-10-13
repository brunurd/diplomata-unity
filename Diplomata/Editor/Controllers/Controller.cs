using System.Collections.Generic;
using LavaLeak.Diplomata.Models;
using LavaLeak.Diplomata.Models.Collections;
using UnityEditor;

namespace LavaLeak.Diplomata.Editor.Controllers
{
  [InitializeOnLoad]
  public class Controller
  {
    static Controller()
    {
      _instance = null;
    }

    private static Controller _instance = null;
    private Options _options;
    private List<Character> _characters;
    private List<Interactable> _interactables;
    private GlobalFlags _globalFlags;
    private Inventory _inventory;
    private Quest[] _quests;

    public static Controller Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new Controller();
        }
        return _instance;
      }
      set { _instance = value; }
    }

    public Options Options
    {
      get
      {
        if (_options == null)
          _options = OptionsController.GetOptions();
        return _options;
      }
    }

    public List<Character> Characters
    {
      get
      {
        if (_characters == null)
          _characters = CharactersController.GetCharacters(Options);
        return _characters;
      }
      set { _characters = value; }
    }

    public List<Interactable> Interactables
    {
      get
      {
        if (_interactables == null)
          _interactables = InteractablesController.GetInteractables(Options);
        return _interactables;
      }
      set { _interactables = value; }
    }

    public GlobalFlags GlobalFlags
    {
      get
      {
        if (_globalFlags == null)
          _globalFlags = GlobalFlagsController.GetGlobalFlags(Options.jsonPrettyPrint);
        return _globalFlags;
      }
      set { _globalFlags = value; }
    }
    
    public Inventory Inventory
    {
      get
      {
        if (_inventory == null)
          _inventory = InventoryController.GetInventory(Options.jsonPrettyPrint);
        return _inventory;
      }
      set { _inventory = value; }
    }
    
    public Quest[] Quests
    {
      get
      {
        if (_quests == null)
          _quests = QuestsController.GetQuests(Options.jsonPrettyPrint);
        return _quests;
      }
      set { _quests = value; }
    }
  }
}