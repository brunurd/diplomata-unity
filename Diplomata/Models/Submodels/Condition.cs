using System;

namespace LavaLeak.Diplomata.Models.Submodels
{
  [Serializable]
  public class Condition
  {
    public Type type;
    public int comparedInfluence;
    public string characterInfluencedName;
    public AfterOf afterOf;
    public Flag globalFlag;
    public int itemId;
    public string interactWith;
    public QuestAndState questAndState;

    [NonSerialized]
    public Events custom = new Events();

    [NonSerialized]
    public bool proceed = true;

    public enum Type
    {
      None = 0,
      AfterOf = 1,
      InfluenceEqualTo = 2,
      InfluenceGreaterThan = 3,
      InfluenceLessThan = 4,
      HasItem = 5,
      ItemWasDiscarded = 6,
      GlobalFlagEqualTo = 7,
      ItemIsEquipped = 8,
      DoesNotHaveTheItem = 9,
      QuestStateIs = 10
    }

    public Condition() {}

    public string DisplayNone()
    {
      return "None";
    }

    public string DisplayAfterOf(string messageContent)
    {
      return string.Format("After of \"{0}\"", messageContent);
    }

    public string DisplayCompareInfluence()
    {
      switch (type)
      {
        case Type.InfluenceEqualTo:
          return string.Format("Influence equal to {0} in {1}", comparedInfluence, characterInfluencedName);
        case Type.InfluenceGreaterThan:
          return string.Format("Influence greater then {0} in {1}", comparedInfluence, characterInfluencedName);
        case Type.InfluenceLessThan:
          return string.Format("Influence less then {0} in {1}", comparedInfluence, characterInfluencedName);
        default:
          return string.Empty;
      }
    }

    public string DisplayHasItem(string itemName)
    {
      return string.Format("Has the item: \"{0}\"", itemName);
    }

    public string DisplayDoesNotHaveItem(string itemName)
    {
      return string.Format("Does not have the item: \"{0}\"", itemName);
    }

    public string DisplayItemWasDiscarded(string itemName)
    {
      return string.Format("Item was discarded: \"{0}\"", itemName);
    }

    public string DisplayItemIsEquipped(string itemName)
    {
      return string.Format("Item is equipped: \"{0}\"", itemName);
    }

    public string DisplayGlobalFlagEqualTo()
    {
      return string.Format("\"{0}\" is {1}", globalFlag.name, globalFlag.value);
    }

    public string DisplayQuestStateIs(Quest[] quests)
    {
      var quest = Quest.Find(quests, questAndState.questId);
      var questState = quest != null ? quest.GetState(questAndState.questStateId) : null;
      var questName = quest != null ? quest.Name : string.Empty;
      var questStateName = questState != null ? questState.ShortDescription : string.Empty;
      return string.Format("Quest \"{0}\" state is: {1}", questName, questStateName);
    }

    public static bool CanProceed(Condition[] conditions)
    {
      foreach (Condition condition in conditions)
      {
        if (!condition.proceed)
        {
          return false;
        }
      }

      return true;
    }
  }
}
