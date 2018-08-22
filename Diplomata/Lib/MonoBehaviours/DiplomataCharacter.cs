using System.Collections.Generic;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata
{
  public class DiplomataCharacter : DiplomataTalkable
  {
    private void Start()
    {
      choices = new List<Message>();
      controlIndexes = new Dictionary<string, int>();

      controlIndexes.Add("context", 0);
      controlIndexes.Add("column", 0);
      controlIndexes.Add("message", 0);

      if (talkable != null && Application.isPlaying)
      {
        talkable = (Character) Find.In(DiplomataData.characters.ToArray()).Where("name", talkable.name).Results[0];
      }
    }

    public void ChooseMessage(string content)
    {
      Character character = (Character) talkable;

      if (currentColumn != null)
      {
        foreach (Message msg in choices)
        {
          var localContent = DictionariesHelper.ContainsKey(msg.content, DiplomataData.gameProgress.options.currentSubtitledLanguage).value;

          if (localContent == content)
          {
            currentMessage = msg;
            OnStartCallbacks();
            break;
          }
        }

        if (currentMessage != null)
        {
          choiceMenu = false;
          choices = new List<Message>();
          character.influence = SetInfluence();
        }

        else
        {
          Debug.LogError("Unable to found the message with the content \"" + content + "\".");
          EndTalk();
        }
      }

      else
      {
        Debug.LogError("No column setted.");
        EndTalk();
      }
    }

    public byte SetInfluence()
    {
      Character character = (Character) talkable;

      if (currentMessage != null)
      {
        byte max = 0;
        List<byte> min = new List<byte>();

        foreach (AttributeDictionary attrMsg in currentMessage.attributes)
        {
          foreach (AttributeDictionary attrChar in character.attributes)
          {
            if (attrMsg.key == attrChar.key)
            {
              if (attrMsg.value < attrChar.value)
              {
                min.Add(attrMsg.value);
                break;
              }
              else if (attrMsg.value >= attrChar.value)
              {
                min.Add(attrChar.value);
                break;
              }
            }
          }
        }

        foreach (byte val in min)
        {
          if (val > max)
          {
            max = val;
          }
        }

        int tempInfluence = (max + character.influence) / 2;
        return (byte) tempInfluence;
      }

      else
      {
        Debug.Log("Cannot set influence, no character attached or message selected.");
        return 50;
      }
    }
  }
}
