using System;
using System.Collections.Generic;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Effect
  {
    public Type type;
    public EndOfContext endOfContext;
    public GoTo goTo;
    public AnimatorAttributeSetter animatorAttributeSetter = new AnimatorAttributeSetter();
    public Flag globalFlag;
    public int itemId;
    public QuestState questState;

    [NonSerialized]
    public Events onStart = new Events();

    [NonSerialized]
    public Events onComplete = new Events();

    public enum Type
    {
      None,
      EndOfContext,
      GoTo,
      SetAnimatorAttribute,
      GetItem,
      DiscardItem,
      SetGlobalFlag,
      EquipItem,
      EndOfDialogue,
      SetQuestState
    }

    [Serializable]
    public struct GoTo
    {
      public string uniqueId;

      public GoTo(string uniqueId)
      {
        this.uniqueId = uniqueId;
      }

      public Message GetMessage(Context context)
      {
        foreach (Column col in context.columns)
        {
          if (Message.Find(col.messages, uniqueId) != null)
          {
            return Message.Find(col.messages, uniqueId);
          }
        }

        return null;
      }
    }

    [Serializable]
    public struct EndOfContext
    {
      public string talkableName;
      public int contextId;

      public EndOfContext(string talkableName, int contextId)
      {
        this.talkableName = talkableName;
        this.contextId = contextId;
      }

      public void Set(string talkableName, int contextId)
      {
        this.talkableName = talkableName;
        this.contextId = contextId;
      }

      public Context GetContext(List<Character> characters, List<Interactable> interactables)
      {
        if (Context.Find(Character.Find(characters, talkableName), contextId) != null)
          return Context.Find(Character.Find(characters, talkableName), contextId);
        if (Context.Find(Interactable.Find(interactables, talkableName), contextId) != null)
          return Context.Find(Interactable.Find(interactables, talkableName), contextId);
        return null;
      }
    }

    [Serializable]
    public struct QuestState
    {
      public string questId;
      public string questStateId;
    }

    public Effect() {}

    public Effect(string talkableName)
    {
      endOfContext.talkableName = talkableName;
    }

    public string DisplayNone()
    {
      return "None";
    }

    public string DisplayEndOfContext(string contextName)
    {
      return string.Format("End of the context \"{0}\"", contextName);
    }

    public string DisplayGoTo(string messageTitle)
    {
      return string.Format("Go to \"{0}\"", messageTitle);
    }

    public string DisplaySetAnimatorAttribute()
    {
      if (animatorAttributeSetter != null)
      {
        switch (animatorAttributeSetter.type)
        {
          case AnimatorControllerParameterType.Bool:
            return string.Format("Set animator attribute {0} to {1}", animatorAttributeSetter.name, animatorAttributeSetter.setBool);

          case AnimatorControllerParameterType.Float:
            return string.Format("Set animator attribute {0} to {1}", animatorAttributeSetter.name, animatorAttributeSetter.setFloat);

          case AnimatorControllerParameterType.Int:
            return string.Format("Set animator attribute {0} to {1}", animatorAttributeSetter.name, animatorAttributeSetter.setInt);

          case AnimatorControllerParameterType.Trigger:
            return string.Format("Pull the trigger {0} of animator", animatorAttributeSetter.name);

          default:
            return "Animator attribute setter type not found.";
        }
      }

      else
      {
        return "Animator attribute setter not found.";
      }
    }

    public string DisplayGetItem(string itemName)
    {
      return string.Format("Get the item: \"{0}\"", itemName);
    }

    public string DisplayDiscardItem(string itemName)
    {
      return string.Format("Discard the item: \"{0}\"", itemName);
    }

    public string DisplayEquipItem(string itemName)
    {
      return string.Format("Equip the item: \"{0}\"", itemName);
    }

    public string DisplayGlobalFlagEqualTo()
    {
      return string.Format("\"{0}\" set to {1}", globalFlag.name, globalFlag.value);
    }

    public string DisplayEndOfDialogue()
    {
      return "End of dialogue.";
    }

    public string DisplaySetQuestState(Quest[] quests)
    {
      var quest = Quest.Find(quests, questState.questId);
      var questState = quest.GetState(questState.questStateId);
      return string.Format("Set quest {0} to: {1}.", quest.Name, questState.Name);
    }
  }
}
