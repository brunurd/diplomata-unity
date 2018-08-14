using System;

namespace Diplomata.Models
{
  [Serializable]
  public class Condition
  {
    public Type type;
    public int comparedInfluence;
    public string characterInfluencedName;
    public AfterOf afterOf;
    public Flag customFlag;
    public int itemId;
    public string interactWith;

    [NonSerialized]
    public Events custom = new Events();

    [NonSerialized]
    public bool proceed = true;

    public enum Type
    {
      None,
      AfterOf,
      InfluenceEqualTo,
      InfluenceGreaterThan,
      InfluenceLessThan,
      HasItem,
      ItemWasDiscarded,
      CustomFlagEqualTo,
      ItemIsEquipped,
      InteractsWith,
      DoesNotHaveTheItem
    }

    [Serializable]
    public struct AfterOf
    {
      public string uniqueId;

      public AfterOf(string uniqueId)
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

    public Condition() {}

    public string DisplayNone()
    {
      return "None";
    }

    public string DisplayAfterOf(string messageContent)
    {
      return "After of \"" + messageContent + "\"";
    }

    public string DisplayCompareInfluence()
    {
      switch (type)
      {
        case Type.InfluenceEqualTo:
          return "Influence equal to " + comparedInfluence + " in " + characterInfluencedName;
        case Type.InfluenceGreaterThan:
          return "Influence greater then " + comparedInfluence + " in " + characterInfluencedName;
        case Type.InfluenceLessThan:
          return "Influence less then " + comparedInfluence + " in " + characterInfluencedName;
        default:
          return string.Empty;
      }
    }

    public string DisplayHasItem(string itemName)
    {
      return "Has the item: \"" + itemName + "\"";
    }

    public string DisplayDoesNotHaveItem(string itemName)
    {
      return "Does not have the item: \"" + itemName + "\"";
    }

    public string DisplayItemWasDiscarded(string itemName)
    {
      return "item was discarded: \"" + itemName + "\"";
    }

    public string DisplayItemIsEquipped(string itemName)
    {
      return "item is equipped: \"" + itemName + "\"";
    }

    public string DisplayCustomFlagEqualTo()
    {
      return "\"" + customFlag.name + "\" is " + customFlag.value;
    }

    public string DisplayInteractsWith(string objectName)
    {
      return "Interacts with \"" + objectName + "\"";
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
