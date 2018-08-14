using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata.GameProgess
{
  public enum Method
  {
    XML,
    JSON
  }

  [Serializable]
  public class GameProgressManager
  {
    public OptionsGameProgess options;
    public CharacterProgress[] characters = new CharacterProgress[0];
    public ItemProgress[] inventory = new ItemProgress[0];
    public TalkLog[] talkLog = new TalkLog[0];
    public Flag[] flags = new Flag[0];

    public void Start()
    {
      options = new OptionsGameProgess();
      SaveCharacters();
      SaveInventory();
      SaveFlags();
    }

    public void SaveCharacters()
    {
      characters = new CharacterProgress[0];

      foreach (Character character in DiplomataManager.characters)
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

    public void LoadCharacters()
    {
      foreach (CharacterProgress character in characters)
      {
        var characterTemp = Character.Find(DiplomataManager.characters, character.name);
        characterTemp.influence = character.influence;

        foreach (ContextProgress context in character.contexts)
        {
          var contextTemp = Context.Find(characterTemp, (int) context.id);
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

      foreach (Item item in DiplomataManager.inventory.items)
      {
        inventory = ArrayHelper.Add(inventory, new ItemProgress(item.id, item.have, item.discarded));
      }
    }

    public void LoadInventory()
    {
      foreach (ItemProgress item in inventory)
      {
        var itemTemp = Item.Find(DiplomataManager.inventory.items, (int) item.id);
        itemTemp.have = item.have;
        itemTemp.discarded = item.discarded;
      }
    }

    public void SaveFlags()
    {
      flags = new Flag[0];

      foreach (Flag flag in DiplomataManager.globalFlags.flags)
      {
        flags = ArrayHelper.Add(flags, flag);
      }
    }

    public void LoadFlags()
    {
      foreach (Flag flag in flags)
      {
        var flagTemp = DiplomataManager.globalFlags.Find(DiplomataManager.globalFlags.flags, flag.name);

        if (flagTemp != null)
        {
          flagTemp.value = flag.value;
        }

        else
        {
          DiplomataManager.globalFlags.flags = ArrayHelper.Add(DiplomataManager.globalFlags.flags, flag);
        }
      }
    }

    public string Serialize(Method method)
    {
      switch (method)
      {
        case Method.JSON:
          return JsonUtility.ToJson(DiplomataManager.gameProgress);

        case Method.XML:
          XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameProgressManager));

          using(StringWriter textWriter = new StringWriter())
          {
            xmlSerializer.Serialize(textWriter, DiplomataManager.gameProgress);
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
            DiplomataManager.gameProgress = JsonUtility.FromJson<GameProgressManager>(data);
            break;

          case Method.XML:
            var serializer = new XmlSerializer(typeof(GameProgressManager));

            using(TextReader reader = new StringReader(data))
            {
              DiplomataManager.gameProgress = (GameProgressManager) serializer.Deserialize(reader);
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
      SaveInventory();
      SaveFlags();

      using(FileStream fileStream = new FileStream(Application.persistentDataPath + "/diplomata_gameProgress" + extension, FileMode.Create))
      {
        binaryFormatter.Serialize(fileStream, DiplomataManager.gameProgress);
      }
    }

    public void Load(string extension = ".sav")
    {
      BinaryFormatter binaryFormatter = new BinaryFormatter();

      using(FileStream fileStream = new FileStream(Application.persistentDataPath + "/diplomata_gameProgress" + extension, FileMode.Open))
      {
        DiplomataManager.gameProgress = (GameProgressManager) binaryFormatter.Deserialize(fileStream);
      }

      LoadCharacters();
      LoadInventory();
      LoadFlags();
    }
  }
}
