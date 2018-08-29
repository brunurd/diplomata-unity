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
    public TalkLog[] talkLogs;

    /// <summary>
    /// The constructor, it return all the persistent data from DiplomataData into the new object.
    /// </summary>
    public DiplomataPersistentData()
    {
      options = new OptionsPersistent();
      options = (OptionsPersistent) DiplomataData.options.GetData();
      characters = Data.GetArrayData<CharacterPersistent>(DiplomataData.characters.ToArray());
      globalFlags = (GlobalFlagsPersistent) DiplomataData.globalFlags.GetData();
      interactables = Data.GetArrayData<InteractablePersistent>(DiplomataData.interactables.ToArray());
      inventory = (InventoryPersistent) DiplomataData.inventory.GetData();
      quests = Data.GetArrayData<QuestPersistent>(DiplomataData.quests);
      talkLogs = Data.GetArrayData<TalkLog>(DiplomataData.talkLogs);
    }
  }
}
