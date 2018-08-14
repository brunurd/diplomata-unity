using System.Collections.Generic;
using Diplomata;
using Diplomata.Helpers;
using Diplomata.Models;
using DiplomataEditor.Helpers;
using UnityEngine;

namespace DiplomataEditor
{
  public class DiplomataEditorData : ScriptableObject
  {
    public static string resourcesFolder = "Assets/Resources/";
    public List<Character> characters = new List<Character>();
    public Options options = new Options();
    public Inventory inventory = new Inventory();
    public GlobalFlags globalFlags = new GlobalFlags();

    public int workingContextMessagesId;
    public int workingContextEditId;
    public string workingCharacter;
    public int workingItemId;

    public static void Instantiate()
    {
      DiplomataData.SetData();

      JSONHelper.CreateFolder("Diplomata/Characters/");

      if (!JSONHelper.Exists("preferences", "Diplomata/"))
      {
        JSONHelper.Create(new Options(), "preferences", false, "Diplomata/");
      }

      if (!JSONHelper.Exists("inventory", "Diplomata/"))
      {
        JSONHelper.Create(new Inventory(), "inventory", false, "Diplomata/");
      }

      if (!JSONHelper.Exists("globalFlags", "Diplomata/"))
      {
        JSONHelper.Create(new GlobalFlags(), "globalFlags", false, "Diplomata/");
      }

      DiplomataData.Restart();
      var diplomataEditor = CreateInstance<DiplomataEditorData>();

      if (!AssetHelper.Exists("Diplomata.asset", "Diplomata/"))
      {
        diplomataEditor.options = DiplomataData.options;
        diplomataEditor.inventory = DiplomataData.inventory;
        diplomataEditor.globalFlags = DiplomataData.globalFlags;
        diplomataEditor.characters = DiplomataData.characters;

        AssetHelper.Create(diplomataEditor, "Diplomata.asset", "Diplomata/");
      }

      else
      {
        diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
        diplomataEditor.options = JSONHelper.Read<Options>("preferences", "Diplomata/");
        diplomataEditor.inventory = JSONHelper.Read<Inventory>("inventory", "Diplomata/");
        diplomataEditor.globalFlags = JSONHelper.Read<GlobalFlags>("globalFlags", "Diplomata/");
        diplomataEditor.UpdateList();
      }
    }

    private void Awake()
    {
      workingCharacter = string.Empty;
      workingContextMessagesId = -1;
      workingContextEditId = -1;
      workingItemId = -1;
    }

    public void UpdateList()
    {
      var charactersFiles = Resources.LoadAll("Diplomata/Characters/");

      characters = new List<Character>();
      options.characterList = new string[0];

      foreach (Object obj in charactersFiles)
      {
        var json = (TextAsset) obj;
        var character = JsonUtility.FromJson<Character>(json.text);

        characters.Add(character);
        options.characterList = ArrayHelper.Add(options.characterList, obj.name);
      }
    }

    public void AddCharacter(string name)
    {
      Character character = new Character(name);

      CheckRepeatedCharacter(character);
    }

    public void CheckRepeatedCharacter(Character character)
    {
      bool canAdd = true;

      foreach (string characterName in options.characterList)
      {
        if (characterName == character.name)
        {
          canAdd = false;
          break;
        }
      }

      if (canAdd)
      {
        characters.Add(character);

        if (characters.Count == 1)
        {
          options.playerCharacterName = character.name;
        }

        options.characterList = ArrayHelper.Add(options.characterList, character.name);
        SavePreferences();

        JSONHelper.Create(character, character.name, options.jsonPrettyPrint, "Diplomata/Characters/");
      }

      else
      {
        Debug.LogError("This name already exists!");
      }
    }

    public void SavePreferences()
    {
      JSONHelper.Update(options, "preferences", options.jsonPrettyPrint, "Diplomata/");
    }

    public void SaveInventory()
    {
      JSONHelper.Update(inventory, "inventory", options.jsonPrettyPrint, "Diplomata/");
    }

    public void SaveGlobalFlags()
    {
      JSONHelper.Update(globalFlags, "globalFlags", options.jsonPrettyPrint, "Diplomata/");
    }

    public void Save(Character character)
    {
      JSONHelper.Update(character, character.name, options.jsonPrettyPrint, "Diplomata/Characters/");
    }
  }
}
