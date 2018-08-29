using System.Collections.Generic;
using System.Linq;
using Diplomata.Helpers;
using Diplomata.Models;
using Diplomata.Persistence;
using Diplomata.Persistence.Models;
using UnityEngine;

namespace Diplomata
{
  [AddComponentMenu("")]
  [ExecuteInEditMode]
  sealed public class DiplomataData : MonoBehaviour
  {
    private static DiplomataData instance = null;
    public static Options options = new Options();
    public static List<Character> characters = new List<Character>();
    public static List<Interactable> interactables = new List<Interactable>();
    public static Inventory inventory = new Inventory();
    public static GlobalFlags globalFlags = new GlobalFlags();
    public static Quest[] quests = new Quest[0];
    public static TalkLog[] talkLogs = new TalkLog[0];
    public static bool isTalking;

    private void Awake()
    {
      if (instance == null)
      {
        instance = this;

        if (Application.isPlaying)
        {
          DontDestroyOnLoad(gameObject);
        }

        Restart();
      }

      else
      {
        DestroyImmediate(gameObject);
      }
    }

    public static void SetData()
    {
      if (instance == null && FindObjectsOfType<DiplomataData>().Length < 1)
      {
        GameObject obj = new GameObject("[Diplomata]");
        obj.AddComponent<DiplomataData>();
      }
    }

    public static void Restart()
    {
      options = new Options();

      var json = (TextAsset) Resources.Load("Diplomata/preferences");

      if (json != null)
      {
        options = JsonUtility.FromJson<Options>(json.text);
      }

      characters = new List<Character>();
      interactables = new List<Interactable>();
      Character.UpdateList();

      inventory = new Inventory();
      json = (TextAsset) Resources.Load("Diplomata/inventory");
      if (json != null)
      {
        inventory = JsonUtility.FromJson<Inventory>(json.text);
      }
      inventory.SetImagesAndSprites();

      globalFlags = new GlobalFlags();
      json = (TextAsset) Resources.Load("Diplomata/globalFlags");
      if (json != null)
      {
        globalFlags = JsonUtility.FromJson<GlobalFlags>(json.text);
      }

      quests = new Quest[0];
      json = (TextAsset) Resources.Load("Diplomata/quests");
      if (json != null)
      {
        quests = JsonUtility.FromJson<Quests>(json.text).GetQuests();
      }

      talkLogs = new TalkLog[0];
      foreach (var character in characters)
      {
        talkLogs = ArrayHelper.Add(talkLogs, new TalkLog(character.name));
      }
    }

    /// <summary>
    /// Method to dispose the game data for new game.
    /// </summary>
    public static void DisposeData()
    {
      DestroyImmediate(instance.gameObject);
    }

    /// <summary>
    /// Get a global flag by name.
    /// </summary>
    /// <param name="name">The name of the flag.</param>
    /// <returns>The <seealso cref="Diplomata.Models.Flag"> object or null.</returns>
    public static Flag GetFlag(string name)
    {
      return globalFlags.Find(globalFlags.flags, name);
    }

    /// <summary>
    /// Get a character by his name.
    /// </summary>
    /// <param name="name">The character name.</param>
    /// <returns>The <seealso cref="Diplomata.Models.Character"> object or null.</returns>
    public static Character GetCharacter(string name)
    {
      return Character.Find(characters, name);
    }

    /// <summary>
    /// Get a context by it's index.
    /// </summary>
    /// <param name="character">The character parent of the context.</param>
    /// <param name="contextIndex">The index of the context.</param>
    /// <returns>The <seealso cref="Diplomata.Models.Context"> object or null.</returns>
    public static Context GetContext(Character character, int contextIndex)
    {
      return Context.Find(character, contextIndex);
    }

    /// <summary>
    /// Get a message by it's context and row index.
    /// </summary>
    /// <param name="context">The context parent of the message.</param>
    /// <param name="columnIndex">The index of the column.</param>
    /// <param name="rowIndex">The index of the row in the column.</param>
    /// <returns></returns>
    public static Message GetMessage(Context context, int columnIndex, int rowIndex)
    {
      var column = Column.Find(context, columnIndex);
      if (column == null) return null;
      var message = Message.Find(column.messages, rowIndex);
      if (message != null) return message;
      return null;
    }

    /// <summary>
    /// Get a context by it's unique id.
    /// </summary>
    /// <param name="uniqueId">The unique id (a guid string).</param>
    /// <returns>The <seealso cref="Diplomata.Models.Message"> object or null.</returns>
    public static Message GetMessage(string uniqueId)
    {
      foreach (Character character in characters)
      {
        foreach (Context context in character.contexts)
        {
          foreach (Column column in context.columns)
          {
            var message = Message.Find(column.messages, uniqueId);
            if (message != null) return message;
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Get a quest by it's name.
    /// </summary>
    /// <param name="name">The quest name.</param>
    /// <returns>The <seealso cref="Diplomata.Models.Quest"> object or null.</returns>
    public static Quest GetQuest(string name)
    {
      return Quest.Find(quests, name);
    }

    /// <summary>
    /// Get a item by it's name.
    /// </summary>
    /// <param name="name">The item name.</param>
    /// <param name="language">The language of this name, if empty uses the options first language.</param>
    /// <returns>The <seealso cref="Diplomata.Models.Item"> object or null.</returns>
    public static Item GetItem(string name, string language = "")
    {
      if (language == "") language = options.languages[0].name;
      return Item.Find(inventory.items, name, language);
    }

    /// <summary>
    /// Return all characters persistent 
    /// </summary>
    /// <returns>A array of characters persistent </returns>
    public static CharacterPersistentContainer GetPersistentCharacters()
    {
      return new CharacterPersistentContainer(Persistence.Data.GetArrayData<CharacterPersistent>(characters.ToArray()));
    }

    /// <summary>
    /// Return all interactables persistent 
    /// </summary>
    /// <returns>A array of interactables persistent </returns>
    public static InteractablePersistentContainer GetPersistentInteractables()
    {
      return new InteractablePersistentContainer(Persistence.Data.GetArrayData<InteractablePersistent>(interactables.ToArray()));
    }

    /// <summary>
    /// Return all quests persistent 
    /// </summary>
    /// <returns>A array of quests persistent </returns>
    public static QuestPersistentContainer GetPersistentQuests()
    {
      return new QuestPersistentContainer(Persistence.Data.GetArrayData<QuestPersistent>(quests));
    }

    /// <summary>
    /// Return all talk logs persistent 
    /// </summary>
    /// <returns>A array of talk logs persistent </returns>
    public static TalkLogPersistentContainer GetPersistentTalkLogs()
    {
      return new TalkLogPersistentContainer(Persistence.Data.GetArrayData<TalkLogPersistent>(talkLogs));
    }
  }
}
