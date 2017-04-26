using UnityEngine;
using DiplomataLib;

namespace DiplomataEditor {

    public class EditorData : ScriptableObject {
        
        public int workingContextMessagesId;
        public int workingContextEditId;
        public string workingCharacter;

        public static void Instantiate() {
            if (Diplomata.instance == null && FindObjectsOfType<Diplomata>().Length < 1) {
                GameObject obj = new GameObject("[ Diplomata ]");
                obj.AddComponent<Diplomata>();
            }
            
            Diplomata.Restart();

            if (!JSONHandler.Exists("editorData.asset", "Diplomata/")) {
                AssetHandler.Create(CreateInstance<EditorData>(), "editorData.asset", "Diplomata/");
            }
        }

        public void Awake() {
            workingCharacter = string.Empty;
            workingContextMessagesId = -1;
            workingContextEditId = -1;
        }

        public void SetWorkingCharacter(string characterName) {
            workingCharacter = characterName;
        }

        public void SetWorkingContextMessagesId(int contextId) {
            workingContextMessagesId = contextId;
        }

        public void SetWorkingContextEditId(int contextId) {
            workingContextEditId = contextId;
        }
    }

}