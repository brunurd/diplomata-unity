using System;
using System.Reflection;
using Diplomata;
using Diplomata.Helpers;
using Diplomata.Models;

namespace Diplomata.Persistence
{
    /// <summary>
    /// A mirror class of Diplomata Data to Save and Load values.
    /// </summary>
    [Serializable]
    public class DiplomataPersistentData
    {
        private SaveFlags save;
        public OptionsPersistent options;
        public CharacterPersistent[] characters;
        public InteractablePersistent[] interactables;
        public InventoryPersistent inventory;
        public GlobalFlagsPersistent globalFlags;
        public QuestPersistent[] quests;
        public TalkLogPersistent[] talkLogs;

        /// <summary>
        /// The constructor, here the data already are saved to the instantiated object.
        /// </summary>
        public DiplomataPersistentData()
        {
            options = new OptionsPersistent();
            DiplomataData.options.GetData(ref options);

            Character.GetArrayData(ref characters, DiplomataData.characters.ToArray());
            
            // Interactable.GetArrayData(ref interactables, DiplomataData.interactables);
            // DiplomataData.inventory.GetData(ref inventory);
            // DiplomataData.globalFlags.GetData(ref globalFlags);
            // Quest.GetArrayData(ref quests, DiplomataData.quests);
            // TalkLog.GetArrayData(ref talkLogs, DiplomataData.talkLogs);
        }

        /// <summary>
        /// Set the Diplomata Data values.
        /// </summary>
        public void SetData()
        {
            // options.SetData(ref DiplomataData.options);
            // CharacterPersistent.SetArrayData(characters, DiplomataData.characters);
            // InteractablePersistent.SetArrayData(interactables, DiplomataData.interactables);
            // inventory.SetData(ref DiplomataData.inventory);
            // globalFlags.SetData(ref DiplomataData.globalFlags);
            // QuestPersistent.SetArrayData(quests, DiplomataData.quests);
            // TalkLogPersistent.SetArrayData(talkLogs, DiplomataData.talkLogs);
        }
    }
}
