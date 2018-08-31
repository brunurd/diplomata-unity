using System;
using Diplomata.Models;
using Diplomata.Persistence;
using Diplomata.Persistence.Models;
using UnityEngine;

namespace Diplomata
{
  /// <summary>
  /// Class to return all persistent data from the DiplomataData object.
  /// </summary>
  [Serializable]
  sealed public class DiplomataPersistentData
  {
    public OptionsPersistent options;
    public CharacterPersistent[] characters;
    public GlobalFlagsPersistent globalFlags;
    public InteractablePersistent[] interactables;
    public InventoryPersistent inventory;
    public QuestPersistent[] quests;
    public TalkLogPersistent[] talkLogs;

    /// <summary>
    /// The constructor, it return all the persistent data from DiplomataData into the new object.
    /// </summary>
    public DiplomataPersistentData()
    {
      options = new OptionsPersistent();
      options = (OptionsPersistent) DiplomataManager.Data.options.GetData();
      characters = Data.GetArrayData<CharacterPersistent>(DiplomataManager.Data.characters.ToArray());
      globalFlags = (GlobalFlagsPersistent) DiplomataManager.Data.globalFlags.GetData();
      interactables = Data.GetArrayData<InteractablePersistent>(DiplomataManager.Data.interactables.ToArray());
      inventory = (InventoryPersistent) DiplomataManager.Data.inventory.GetData();
      quests = Data.GetArrayData<QuestPersistent>(DiplomataManager.Data.quests);
      talkLogs = Data.GetArrayData<TalkLogPersistent>(DiplomataManager.Data.talkLogs);
    }
  }
}
