using System;
using System.Collections.Generic;
using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEngine;

namespace LavaLeak.Diplomata
{
  /// <summary>
  /// The Diplomata Character component.
  /// </summary>
  public class DiplomataCharacter : DiplomataTalkable
  {
    public event Action<Message> OnMessageChosen;
    

    /// <summary>
    /// Set the main talkable fields.
    /// </summary>
    private void Start()
    {
      if (choices == null)
        choices = new List<Message>();

      controlIndexes = new Dictionary<string, int>();

      controlIndexes.Add("context", 0);
      controlIndexes.Add("column", 0);
      controlIndexes.Add("message", 0);

      if (talkable != null && Application.isPlaying)
      {
        talkable = Character.Find(DiplomataManager.Data.characters, talkable.name);
      }
    }

    /// <summary>
    /// Get all the choices in a list.
    /// </summary>
    /// <returns>A list with all the choices of the current column.</returns>
    public List<string> MessageChoices()
    {
      var choicesText = new List<string>();

      if (choices.Count > 0)
      {
        foreach (var choice in choices)
        {
          var content = DictionariesHelper.ContainsKey(choice.content, DiplomataManager.Data.options.currentLanguage).value;

          if (!choice.alreadySpoked && choice.disposable)
          {
            choicesText.Add(currentContext.ReplaceVariables(content));
          }
          else if (!choice.disposable)
          {
            choicesText.Add(currentContext.ReplaceVariables(content));
          }
        }
      }

      else
      {
        Debug.Log("There's no choice this time.");

        if (IsLastMessage())
        {
          EndTalk();
        }

        else
        {
          controlIndexes["column"] += 1;
          controlIndexes["message"] = 0;
          Next(false);
        }
      }

      return choicesText;
    }

    /// <summary>
    /// To set a choice by the player.
    /// </summary>
    /// <param name="content">The choice text.</param>
    public void ChooseMessage(string content)
    {
      var character = (Character) talkable;

      if (currentColumn != null)
      {
        foreach (var msg in choices)
        {
          var localContent = DictionariesHelper.ContainsKey(msg.content, DiplomataManager.Data.options.currentLanguage).value;

          if (currentContext.ReplaceVariables(localContent) == content)
          {
            currentMessage = msg;
            OnStartCallbacks();
            break;
          }
        }

        if (currentMessage != null)
        {
          if (OnMessageChosen != null)
            OnMessageChosen(currentMessage);
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

    /// <summary>
    /// Set the influence over the character using the message and player attributes.
    /// </summary>
    /// <returns>The influence value.</returns>
    public byte SetInfluence()
    {
      var character = (Character) talkable;

      if (currentMessage != null)
      {
        byte max = 0;
        var min = new List<byte>();

        foreach (var attrMsg in currentMessage.attributes)
        {
          foreach (var attrChar in character.attributes)
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
