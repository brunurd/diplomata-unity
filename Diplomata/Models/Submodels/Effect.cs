using System;
using System.Collections.Generic;
using UnityEngine;

namespace LavaLeak.Diplomata.Models.Submodels
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
      None = 0,
      EndOfContext = 1,
      GoTo = 2,
      SetAnimatorAttribute = 3,
      GetItem = 4,
      DiscardItem = 5,
      SetGlobalFlag = 6,
      EquipItem = 7,
      EndOfDialogue = 8,
      SetQuestState = 9,
      FinishQuest = 10,
      StartQuest = 11
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
      var questState = quest != null ? quest.GetState(questAndState.questStateId) : null;

      var questName = quest != null ? quest.Name : string.Empty;
      var questStateName = questState != null ? questState.ShortDescription : string.Empty;

      return string.Format("Set quest \"{0}\" to: {1}", questName, questStateName);
    }

    public string DiplayStartQuest(Quest[] quests)
    {
      var quest = Quest.Find(quests, questAndState.questId);
      var questName = quest != null ? quest.Name : string.Empty;
      return string.Format("Start the quest \"{0}\"", questName);
    }

    public string DisplayFinishQuest(Quest[] quests)
    {
      var quest = Quest.Find(quests, questAndState.questId);
      var questName = quest != null ? quest.Name : string.Empty;
      return string.Format("Finish the quest \"{0}\"", questName);
    }
  }
}
