using System;
using LavaLeak.Diplomata.Models;

namespace LavaLeak.Diplomata
{ 
  /// <summary>
  /// Single source of Truth for all events that are occuring
  /// on Diplomata entities.
  /// Each entity still have local events for local subscriptions,
  /// but then will also trigger this global events to make easier
  /// to get information about what is happening on the state of
  /// quests and items.
  /// </summary>
  public class DiplomataEventController
  {
    /// <summary>
    /// Happens every time a Item is caught.
    /// </summary>
    public event Action<Item> OnItemWasCaught;
    
    /// <summary>
    /// Happens every time a Quest is Initialized.
    /// </summary>
    public event Action<Quest> OnQuestStart;
    
    /// <summary>
    /// Happens every time a Quest state changes.
    /// </summary>
    public event Action<Quest> OnQuestStateChange;
    
    /// <summary>
    /// Happens every time a quest ends.
    /// </summary>
    public event Action<Quest> OnQuestEnd;

    /// <summary>
    /// Happens every time a context ends.
    /// </summary>
    public event Action<Context> OnContextEnd;

    /// <summary>
    /// Happens every time a flag is set.
    /// </summary>
    public event Action<Flag> OnSetFlag;

    /// <summary>
    /// Trigger the event OnQuestStart with the quest data that have started.
    /// </summary>
    /// <param name="questStart">Quest data</param>
    public void SendQuestStart(Quest questStart)
    {
      if (OnQuestStart != null)
        OnQuestStart(questStart);
    }

    /// <summary>
    /// Trigger the event OnQuestStateChange with the quest data that have changed states.
    /// </summary>
    /// <param name="questStateChange">Quest data</param>
    public void SendQuestStateChange(Quest questStateChange)
    {
      if (OnQuestStateChange != null)
        OnQuestStateChange(questStateChange);
    }

    /// <summary>
    /// Trigger the event OnQuestEnd with the quest data that have completed. 
    /// </summary>
    /// <param name="questEnd">Quest data</param>
    public void SendQuestEnd(Quest questEnd)
    {
      if (OnQuestEnd != null)
        OnQuestEnd(questEnd);
    }

    /// <summary>
    /// Trigger the event OnItemWasCaught with the the item data that from the item that was caught.
    /// </summary>
    /// <param name="itemWasCaught">Item data</param>
    public void SendItemWasCaught(Item itemWasCaught)
    {
      if (OnItemWasCaught != null)
        OnItemWasCaught(itemWasCaught);
    }

    /// <summary>
    /// Trigger the event OnContextEnd with the ended context data.
    /// </summary>
    /// <param name="context"></param>
    public void SendContextEnd(Context context)
    {
      if (OnContextEnd != null)
        OnContextEnd(context);
    }
    
    /// <summary>
    /// Trigger the event OnSetFlag when a flag is set.
    /// </summary>
    /// <param name="flag">The flag.</param>
    public void SendOnSetFlag(Flag flag)
    {
      if (OnSetFlag != null)
        OnSetFlag(flag);
    }
  }
}
