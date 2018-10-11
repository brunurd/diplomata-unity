using System;
using System.Collections.Generic;
using System.IO;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using LavaLeak.Diplomata.Models.Collections;
using UnityEngine;

namespace LavaLeak.Diplomata
{
  /// <summary>
  /// The Data storage class, here are all the Diplomata Data fields.
  /// </summary>
  [Serializable]
  public class DiplomataData : MonoBehaviour
  {
    public Options options;
    public List<Character> characters;
    public List<Interactable> interactables;
    public Inventory inventory;
    public GlobalFlags globalFlags;
    public Quest[] quests;
    public TalkLog[] talkLogs;
    public DiplomataEventController EventController;
    public bool OnATalk;

    private void Awake()
    {
      Reset();
      DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Reset all fields.
    /// </summary>
    public void Reset()
    {
      options = new Options();
      characters = new List<Character>();
      interactables = new List<Interactable>();
      inventory = new Inventory();
      globalFlags = new GlobalFlags();
      quests = new Quest[0];
      talkLogs = new TalkLog[0];
      EventController = new DiplomataEventController();
      OnATalk = false;
      ReadJSONs();
    }

    /// <summary>
    /// Get the JSON's data.
    /// </summary>
    public void ReadJSONs()
    {
      // Reset objects.
      options = new Options();
      inventory = new Inventory();
      globalFlags = new GlobalFlags();
      quests = new Quest[0];

      // Read JSON's.
      options = ReadJSON<Options>("preferences", "Diplomata");
      inventory = ReadJSON<Inventory>("inventory", "Diplomata");
      globalFlags = ReadJSON<GlobalFlags>("globalFlags", "Diplomata");
      quests = ReadJSON<Quests>("quests", "Diplomata").GetQuests();

      // Update characters and interactables.
      UpdateList();
    }

    /// <summary>
    /// Read a JSON file and turin into a object.
    /// </summary>
    /// <param name="filename">The json file name.</param>
    /// <param name="folder">A extra folder path in resources.</param>
    /// <typeparam name="T">The type of the return.</typeparam>
    /// <returns>The object from the json or null.</returns>
    private T ReadJSON<T>(string filename, string folder = "")
    {
      try
      {
        TextAsset json = (TextAsset) Resources.Load(Path.Combine(folder, filename));
        if (json == null) json = (TextAsset) Resources.Load(filename);
        return JsonUtility.FromJson<T>(json.text);
      }
      catch (Exception e)
      {
        Debug.LogError(string.Format("Cannot read {0} in Resources folder. {1}", filename, e.Message));
        return default(T);
      }
    }

    /// <summary>
    /// Update the characters and interactables lists from JSON's.
    /// </summary>
    public void UpdateList()
    {
      var charactersFiles = Resources.LoadAll("Diplomata/Characters/");
      var interactablesFiles = Resources.LoadAll("Diplomata/Interactables/");

      characters = new List<Character>();
      interactables = new List<Interactable>();

      options.characterList = new string[0];
      options.interactableList = new string[0];

      foreach (UnityEngine.Object obj in charactersFiles)
      {
        var json = (TextAsset) obj;
        var character = JsonUtility.FromJson<Character>(json.text);

        characters.Add(character);
        options.characterList = ArrayHelper.Add(options.characterList, obj.name);
      }

      foreach (UnityEngine.Object obj in interactablesFiles)
      {
        var json = (TextAsset) obj;
        var interactable = JsonUtility.FromJson<Interactable>(json.text);

        interactables.Add(interactable);
        options.interactableList = ArrayHelper.Add(options.interactableList, obj.name);
      }
    }
  }
}
