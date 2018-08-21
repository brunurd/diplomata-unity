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
    public QuestAndState questAndState;

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
      var quest = Quest.Find(quests, questAndState.questId);
      var questState = quest.GetState(questAndState.questStateId);
      return string.Format("Set quest {0} to: {1}.", quest.Name, questState.Name);
    }
  }
}
