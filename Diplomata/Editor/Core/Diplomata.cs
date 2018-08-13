using System.Collections.Generic;
using DiplomataEditor.Helpers;
using DiplomataLib;
using UnityEngine;

namespace DiplomataEditor.Core
{
  public class Diplomata : ScriptableObject
  {
    public static string resourcesFolder = "Assets/Resources/";
    public List<Character> characters = new List<Character>();
    public DiplomataLib.Preferences preferences = new DiplomataLib.Preferences();
    public Inventory inventory = new Inventory();
    public CustomFlags customFlags = new CustomFlags();

    public int workingContextMessagesId;
    public int workingContextEditId;
    public string workingCharacter;
    public int workingItemId;

    public static void Instantiate()
    {
      if (DiplomataLib.Diplomata.instance == null && FindObjectsOfType<DiplomataLib.Diplomata>().Length < 1)
      {
        GameObject obj = new GameObject("[Diplomata]");
        obj.AddComponent<DiplomataLib.Diplomata>();
      }

      JSONHelper.CreateFolder("Diplomata/Characters/");

      if (!JSONHelper.Exists("preferences", "Diplomata/"))
      {
        JSONHelper.Create(new DiplomataLib.Preferences(), "preferences", false, "Diplomata/");
      }

      if (!JSONHelper.Exists("inventory", "Diplomata/"))
      {
        JSONHelper.Create(new Inventory(), "inventory", false, "Diplomata/");
      }

      if (!JSONHelper.Exists("customFlags", "Diplomata/"))
      {
        JSONHelper.Create(new CustomFlags(), "customFlags", false, "Diplomata/");
      }

      DiplomataLib.Diplomata.Restart();
      var diplomataEditor = CreateInstance<Diplomata>();

      if (!AssetHelper.Exists("Diplomata.asset", "Diplomata/"))
      {
        diplomataEditor.preferences = DiplomataLib.Diplomata.preferences;
        diplomataEditor.inventory = DiplomataLib.Diplomata.inventory;
        diplomataEditor.customFlags = DiplomataLib.Diplomata.customFlags;
        diplomataEditor.characters = DiplomataLib.Diplomata.characters;

        AssetHelper.Create(diplomataEditor, "Diplomata.asset", "Diplomata/");
      }

      else
      {
        diplomataEditor = (Diplomata) AssetHelper.Read("Diplomata.asset", "Diplomata/");
        diplomataEditor.preferences = JSONHelper.Read<DiplomataLib.Preferences>("preferences", "Diplomata/");
        diplomataEditor.inventory = JSONHelper.Read<Inventory>("inventory", "Diplomata/");
        diplomataEditor.customFlags = JSONHelper.Read<CustomFlags>("customFlags", "Diplomata/");
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
      preferences.characterList = new string[0];

      foreach (Object obj in charactersFiles)
      {
        var json = (TextAsset) obj;
        var character = JsonUtility.FromJson<Character>(json.text);

        characters.Add(character);
        preferences.characterList = ArrayHandler.Add(preferences.characterList, obj.name);
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

      foreach (string characterName in preferences.characterList)
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
          preferences.playerCharacterName = character.name;
        }

        preferences.characterList = ArrayHandler.Add(preferences.characterList, character.name);
        SavePreferences();

        JSONHelper.Create(character, character.name, preferences.jsonPrettyPrint, "Diplomata/Characters/");
      }

      else
      {
        Debug.LogError("This name already exists!");
      }
    }

    public void SavePreferences()
    {
      JSONHelper.Update(preferences, "preferences", preferences.jsonPrettyPrint, "Diplomata/");
    }

    public void SaveInventory()
    {
      JSONHelper.Update(inventory, "inventory", preferences.jsonPrettyPrint, "Diplomata/");
    }

    public void SaveCustomFlags()
    {
      JSONHelper.Update(customFlags, "customFlags", preferences.jsonPrettyPrint, "Diplomata/");
    }

    public void Save(Character character)
    {
      JSONHelper.Update(character, character.name, preferences.jsonPrettyPrint, "Diplomata/Characters/");
    }
  }
}
