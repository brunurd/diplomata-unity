using System;
using System.Linq;
using LavaLeak.Diplomata.Models;
using LavaLeak.Diplomata.Persistence;
using LavaLeak.Diplomata.Persistence.Models;
using UnityEngine;

namespace LavaLeak.Diplomata
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
    /// It get all the persistent data from DiplomataData.
    /// </summary>
    public void Save()
    {
      options = (OptionsPersistent) DiplomataManager.Data.options.GetData();
      characters = Data.GetArrayData<CharacterPersistent>(DiplomataManager.Data.characters.ToArray());
      globalFlags = (GlobalFlagsPersistent) DiplomataManager.Data.globalFlags.GetData();
      interactables = Data.GetArrayData<InteractablePersistent>(DiplomataManager.Data.interactables.ToArray());
      inventory = (InventoryPersistent) DiplomataManager.Data.inventory.GetData();
      quests = Data.GetArrayData<QuestPersistent>(DiplomataManager.Data.quests);
      talkLogs = Data.GetArrayData<TalkLogPersistent>(DiplomataManager.Data.talkLogs);
    }

    /// <summary>
    /// Set the DiplomataData from persistent data.
    /// </summary>
    public void Load()
    {
      DiplomataManager.Data.Reset();
      DiplomataManager.Data.options.SetData(options);
      DiplomataManager.Data.characters = Data.SetArrayData<Character>(DiplomataManager.Data.characters.ToArray(), characters).OfType<Character>().ToList();
      DiplomataManager.Data.globalFlags.SetData(globalFlags);
      DiplomataManager.Data.interactables = Data.SetArrayData<Interactable>(DiplomataManager.Data.interactables.ToArray(), interactables).OfType<Interactable>().ToList();
      DiplomataManager.Data.inventory.SetData(inventory);
      DiplomataManager.Data.quests = Data.SetArrayData<Quest>(DiplomataManager.Data.quests, quests);
      DiplomataManager.Data.talkLogs = Data.SetArrayData<TalkLog>(DiplomataManager.Data.talkLogs, talkLogs);
    }
  }
}
