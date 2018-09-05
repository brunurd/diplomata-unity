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
    /// The constructor, it return all the persistent data from DiplomataData into the new object.
    /// </summary>
    public DiplomataPersistentData()
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
    /// <param name="data">A <seealso cref="LavaLeak.Diplomata.DiplomataData"> reference.</param>
    public DiplomataData SetDiplomataData()
    {
      var data = ScriptableObject.CreateInstance<DiplomataData>();
      data.ReadJSONs();

      data.options.SetData(options);
      data.characters = Data.SetArrayData<Character>(data.characters.ToArray(), characters).OfType<Character>().ToList();
      data.globalFlags.SetData(globalFlags);
      data.interactables = Data.SetArrayData<Interactable>(data.interactables.ToArray(), interactables).OfType<Interactable>().ToList();
      data.inventory.SetData(inventory);
      data.quests = Data.SetArrayData<Quest>(data.quests, quests);
      data.talkLogs = Data.SetArrayData<TalkLog>(data.talkLogs, talkLogs);

      return data;
    }
  }
}
