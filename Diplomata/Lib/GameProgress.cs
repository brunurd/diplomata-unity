using System;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DiplomataLib {

    [Serializable]
    public class Options {
        public string currentSubtitledLanguage = string.Empty;
        public string currentDubbedLanguage = string.Empty;
        public float volumeScale = 1.0f;

        public Options() {
            foreach (Language lang in Diplomata.preferences.languages) {

                if (lang.subtitle && currentSubtitledLanguage == string.Empty) {
                    currentSubtitledLanguage = lang.name;
                }

                if (lang.dubbing && currentDubbedLanguage == string.Empty) {
                    currentDubbedLanguage = lang.name;
                }

            }
        }
    }

    [Serializable]
    public class TalkLog {
        public string characterName;
        public string[] messagesIds = new string[0];

        public TalkLog() { }

        public TalkLog(string characterName) {
            this.characterName = characterName;
        }

        public static TalkLog Find(TalkLog[] array, string characterName) {
            foreach (TalkLog talkLog in array) {
                if (talkLog.characterName == characterName) {
                    return talkLog;
                }
            }

            return null;
        }
    }

    [Serializable]
    public class CharacterProgress {
        public string name;
        public byte influence;
        public ContextProgress[] contexts = new ContextProgress[0];
        
        public CharacterProgress() { }

        public CharacterProgress(string name, byte influence) {
            this.name = name;
            this.influence = influence;
        }
    }

    [Serializable]
    public class ContextProgress {
        public uint id;
        public bool happened;
        public ColumnProgress[] columns = new ColumnProgress[0];

        public ContextProgress() { }

        public ContextProgress(int id, bool happened) {
            this.id = (uint) id;
            this.happened = happened;
        }
    }

    [Serializable]
    public class ColumnProgress {
        public uint id;
        public MessageProgress[] messages = new MessageProgress[0];

        public ColumnProgress() { }

        public ColumnProgress(int id) {
            this.id = (uint) id;
        }
    }

    [Serializable]
    public class MessageProgress {
        public uint id;
        public bool alreadySpoked;

        public MessageProgress() { }

        public MessageProgress(int id, bool alreadySpoked) {
            this.id = (uint) id;
            this.alreadySpoked = alreadySpoked;
        }
    }

    [Serializable]
    public class ItemProgress {
        public uint id;
        public bool have;
        public bool discarded;

        public ItemProgress() { }

        public ItemProgress(int id, bool have, bool discarded) {
            this.id = (uint)id;
            this.have = have;
            this.discarded = discarded;
        }
    }

    public enum Method {
        XML,
        JSON
    }

    [Serializable]
    public class GameProgress {

        public Options options;
        public CharacterProgress[] characters = new CharacterProgress[0];
        public ItemProgress[] inventory = new ItemProgress[0];
        public TalkLog[] talkLog = new TalkLog[0];
        public Flag[] flags = new Flag[0];

        public void Start() {
            options = new Options();
            SaveCharacters();
            SaveInventory();
            SaveFlags();
        }

        public void SaveCharacters() {
            characters = new CharacterProgress[0];
            
            foreach (Character character in Diplomata.characters) {
                var newCharacter = new CharacterProgress(character.name, character.influence);
                
                foreach (Context context in character.contexts) {
                    var newContext = new ContextProgress(context.id, context.happened);

                    foreach (Column column in context.columns) {
                        var newColumn = new ColumnProgress(column.id);

                        foreach(Message message in column.messages) {
                            newColumn.messages = ArrayHandler.Add(newColumn.messages, 
                                new MessageProgress(message.id, message.alreadySpoked));
                        }

                        newContext.columns = ArrayHandler.Add(newContext.columns, newColumn);
                    }

                    newCharacter.contexts = ArrayHandler.Add(newCharacter.contexts, newContext);
                }

                characters = ArrayHandler.Add(characters, newCharacter);
            }
        }

        public void LoadCharacters() {
            foreach (CharacterProgress character in characters) {
                var characterTemp = Character.Find(Diplomata.characters, character.name);
                characterTemp.influence = character.influence;

                foreach (ContextProgress context in character.contexts) {
                    var contextTemp = Context.Find(characterTemp, (int) context.id);
                    contextTemp.happened = context.happened;

                    foreach (ColumnProgress column in context.columns) {
                        var columnTemp = Column.Find(contextTemp, (int) column.id);

                        foreach (MessageProgress message in column.messages) {
                            Message.Find(columnTemp.messages, (int) message.id).alreadySpoked = message.alreadySpoked;
                        }
                    }
                }
            }
        }

        public void SaveInventory() {
            inventory = new ItemProgress[0];

            foreach (Item item in Diplomata.inventory.items) {
                inventory = ArrayHandler.Add(inventory, new ItemProgress(item.id, item.have, item.discarded));
            }
        }

        public void LoadInventory() {
            foreach (ItemProgress item in inventory) {
                var itemTemp = Item.Find(Diplomata.inventory.items, (int) item.id);
                itemTemp.have = item.have;
                itemTemp.discarded = item.discarded;
            }
        }

        public void SaveFlags() {
            flags = new Flag[0];

            foreach (Flag flag in Diplomata.customFlags.flags) {
                flags = ArrayHandler.Add(flags, flag);
            }
        }

        public void LoadFlags() {
            foreach (Flag flag in flags) {
                var flagTemp = Diplomata.customFlags.Find(Diplomata.customFlags.flags, flag.name);

                if (flagTemp != null) {
                    flagTemp.value = flag.value;
                }

                else {
                    Diplomata.customFlags.flags = ArrayHandler.Add(Diplomata.customFlags.flags, flag);
                }
            }
        }

        public string Serialize(Method method) {
            switch (method) {
                case Method.JSON:
                    return JsonUtility.ToJson(Diplomata.gameProgress);

                case Method.XML:
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameProgress));

                    using (StringWriter textWriter = new StringWriter()) {
                        xmlSerializer.Serialize(textWriter, Diplomata.gameProgress);
                        return textWriter.GetStringBuilder().ToString();
                    }
                    
                default:
                    return null;
            }
        }

        public void Deserialize(string data, Method method) {
            try {
                switch (method) {
                    case Method.JSON:
                        Diplomata.gameProgress = JsonUtility.FromJson<GameProgress>(data);
                        break;

                    case Method.XML:
                        var serializer = new XmlSerializer(typeof(GameProgress));

                        using (TextReader reader = new StringReader(data)) {
                            Diplomata.gameProgress = (GameProgress)serializer.Deserialize(reader);
                        }

                        break;
                }
            }

            catch (Exception e) {
                Debug.LogError("Cannot deserialize this string data, make sure this was serialized from a gameProgress object. " + e.Message);
            }
        }

        public void Save(string extension = ".sav") {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            SaveCharacters();
            SaveInventory();
            SaveFlags();
           
            using (FileStream fileStream = new FileStream(Application.persistentDataPath + "/diplomata_gameProgress" + extension, FileMode.Create)) {
                binaryFormatter.Serialize(fileStream, Diplomata.gameProgress);
            }
        }

        public void Load(string extension = ".sav") {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (FileStream fileStream = new FileStream(Application.persistentDataPath + "/diplomata_gameProgress" + extension, FileMode.Open)) {
                Diplomata.gameProgress = (GameProgress)binaryFormatter.Deserialize(fileStream);
            }

            LoadCharacters();
            LoadInventory();
            LoadFlags();
        }
    }

}
