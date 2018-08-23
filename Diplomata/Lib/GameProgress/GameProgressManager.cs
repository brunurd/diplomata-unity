using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata.GameProgress
{
  public enum Method
  {
    XML,
    JSON
  }

  [Serializable]
  public class GameProgressManager
  {
    public OptionsGameProgress options;
    public CharacterProgress[] characters = new CharacterProgress[0];
    public InteractableProgress[] interactables = new InteractableProgress[0];
    public ItemProgress[] inventory = new ItemProgress[0];
    public TalkLog[] talkLog = new TalkLog[0];
    public Flag[] flags = new Flag[0];

    public void Start()
    {
      options = new OptionsGameProgress();
      SaveCharacters();
      SaveInteractables();
      SaveInventory();
      SaveFlags();
    }

    public void SaveCharacters()
    {
      characters = new CharacterProgress[0];

      foreach (Character character in DiplomataData.characters)
      {
        var newCharacter = new CharacterProgress(character.name, character.influence);

        foreach (Context context in character.contexts)
        {
          var newContext = new ContextProgress(context.id, context.happened);

          foreach (Column column in context.columns)
          {
            var newColumn = new ColumnProgress(column.id);

            foreach (Message message in column.messages)
            {
              newColumn.messages = ArrayHelper.Add(newColumn.messages,
                new MessageProgress(message.id, message.alreadySpoked));
            }

            newContext.columns = ArrayHelper.Add(newContext.columns, newColumn);
          }

          newCharacter.contexts = ArrayHelper.Add(newCharacter.contexts, newContext);
        }

        characters = ArrayHelper.Add(characters, newCharacter);
      }
    }

    public void SaveInteractables()
    {
      interactables = new InteractableProgress[0];

      foreach (Interactable interactable in DiplomataData.interactables)
      {
        var newInteractable = new InteractableProgress(interactable.name);

        foreach (Context context in interactable.contexts)
        {
          var newContext = new ContextProgress(context.id, context.happened);

          foreach (Column column in context.columns)
          {
            var newColumn = new ColumnProgress(column.id);

            foreach (Message message in column.messages)
            {
              newColumn.messages = ArrayHelper.Add(newColumn.messages,
                new MessageProgress(message.id, message.alreadySpoked));
            }

            newContext.columns = ArrayHelper.Add(newContext.columns, newColumn);
          }

          newInteractable.contexts = ArrayHelper.Add(newInteractable.contexts, newContext);
        }

        interactables = ArrayHelper.Add(interactables, newInteractable);
      }
    }

    public void LoadCharacters()
    {
      foreach (CharacterProgress character in characters)
      {
        var characterTemp = (Character) Find.In(DiplomataData.characters.ToArray()).Where("name", character.name).Results[0];
        characterTemp.influence = character.influence;

        foreach (ContextProgress context in character.contexts)
        {
          var contextTemp = (Context) Find.In(characterTemp.contexts)
            .Where("id", (int) context.id).Results[0];
          contextTemp.happened = context.happened;

          foreach (ColumnProgress column in context.columns)
          {
            var columnTemp = Column.Find(contextTemp, (int) column.id);

            foreach (MessageProgress message in column.messages)
            {
              Message.Find(columnTemp.messages, (int) message.id).alreadySpoked = message.alreadySpoked;
            }
          }
        }
      }
    }

    public void LoadInteractables()
    {
      foreach (InteractableProgress interactable in interactables)
      {
        var interactableTemp = Interactable.Find(DiplomataData.interactables, interactable.name);

        foreach (ContextProgress context in interactable.contexts)
        {
          var contextTemp = (Context) Find.In(interactableTemp.contexts)
            .Where("id", (int) context.id).Results[0];
          contextTemp.happened = context.happened;

          foreach (ColumnProgress column in context.columns)
          {
            var columnTemp = Column.Find(contextTemp, (int) column.id);

            foreach (MessageProgress message in column.messages)
            {
              Message.Find(columnTemp.messages, (int) message.id).alreadySpoked = message.alreadySpoked;
            }
          }
        }
      }
    }

    public void SaveInventory()
    {
      inventory = new ItemProgress[0];

      foreach (Item item in DiplomataData.inventory.items)
      {
        inventory = ArrayHelper.Add(inventory, new ItemProgress(item.id, item.have, item.discarded));
      }
    }

    public void LoadInventory()
    {
      foreach (ItemProgress item in inventory)
      {
        var itemTemp = Item.Find(DiplomataData.inventory.items, (int) item.id);
        itemTemp.have = item.have;
        itemTemp.discarded = item.discarded;
      }
    }

    public void SaveFlags()
    {
      flags = new Flag[0];

      foreach (Flag flag in DiplomataData.globalFlags.flags)
      {
        flags = ArrayHelper.Add(flags, flag);
      }
    }

    public void LoadFlags()
    {
      foreach (Flag flag in flags)
      {
        var flagTemp = DiplomataData.globalFlags.Find(DiplomataData.globalFlags.flags, flag.name);

        if (flagTemp != null)
        {
          flagTemp.value = flag.value;
        }

        else
        {
          DiplomataData.globalFlags.flags = ArrayHelper.Add(DiplomataData.globalFlags.flags, flag);
        }
      }
    }

    public string Serialize(Method method)
    {
      switch (method)
      {
        case Method.JSON:
          return JsonUtility.ToJson(DiplomataData.gameProgress);

        case Method.XML:
          XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameProgressManager));

          using(StringWriter textWriter = new StringWriter())
          {
            xmlSerializer.Serialize(textWriter, DiplomataData.gameProgress);
            return textWriter.GetStringBuilder().ToString();
          }

        default:
          return null;
      }
    }

    public void Deserialize(string data, Method method)
    {
      try
      {
        switch (method)
        {
          case Method.JSON:
            DiplomataData.gameProgress = JsonUtility.FromJson<GameProgressManager>(data);
            break;

          case Method.XML:
            var serializer = new XmlSerializer(typeof(GameProgressManager));

            using(TextReader reader = new StringReader(data))
            {
              DiplomataData.gameProgress = (GameProgressManager) serializer.Deserialize(reader);
            }

            break;
        }
      }

      catch (Exception e)
      {
        Debug.LogError("Cannot deserialize this string data, make sure this was serialized from a gameProgress object. " + e.Message);
      }
    }

    public void Save(string extension = ".sav")
    {
      BinaryFormatter binaryFormatter = new BinaryFormatter();

      SaveCharacters();
      SaveInteractables();
      SaveInventory();
      SaveFlags();

      using(FileStream fileStream = new FileStream(Application.persistentDataPath + "/diplomata_gameProgress" + extension, FileMode.Create))
      {
        binaryFormatter.Serialize(fileStream, DiplomataData.gameProgress);
      }
    }

    public void Load(string extension = ".sav")
    {
      BinaryFormatter binaryFormatter = new BinaryFormatter();

      using(FileStream fileStream = new FileStream(Application.persistentDataPath + "/diplomata_gameProgress" + extension, FileMode.Open))
      {
        DiplomataData.gameProgress = (GameProgressManager) binaryFormatter.Deserialize(fileStream);
      }

      LoadCharacters();
      LoadInteractables();
      LoadInventory();
      LoadFlags();
    }
  }
}
