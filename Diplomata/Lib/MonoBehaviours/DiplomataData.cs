using System.Collections.Generic;
using System.Linq;
using Diplomata.GameProgress;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata
{
  [AddComponentMenu("")]
  [ExecuteInEditMode]
  public class DiplomataData : MonoBehaviour
  {
    private static DiplomataData instance = null;
    public static Options options = new Options();
    public static GameProgressManager gameProgress = new GameProgressManager();
    public static List<Character> characters = new List<Character>();
    public static List<Interactable> interactables = new List<Interactable>();
    public static Inventory inventory = new Inventory();
    public static GlobalFlags globalFlags = new GlobalFlags();
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

      gameProgress = new GameProgressManager();
      gameProgress.Start();
    }

    public static Message GetMessageById(string uniqueId)
    {
      foreach (Character character in characters)
      {
        foreach (Context context in character.contexts)
        {
          foreach (Column column in context.columns)
          {
            foreach (Message message in column.messages)
            {
              if (message.uniqueId == uniqueId)
              {
                return message;
              }
            }
          }
        }
      }
      return null;
    }

    public static List<Message> GetMessageByLabel(Context context, string labelName)
    {
      string labelId = null;
      List<Message> messages = new List<Message>();

      foreach (Label label in context.labels)
      {
        if (label.name == labelName)
        {
          labelId = label.id;
        }
      }

      if (labelId != null)
      {
        foreach (Column column in context.columns)
        {
          foreach (Message message in column.messages)
          {
            if (message.labelId == labelId)
            {
              messages.Add(message);
            }
          }
        }
      }
      else
      {
        Debug.LogWarning("No label with the name \"" + labelName + "\"");
      }

      return messages;
    }
  }
}
