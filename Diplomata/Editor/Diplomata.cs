using System.Collections.Generic;
using UnityEngine;
using DiplomataLib;

namespace DiplomataEditor {

    public class Diplomata : ScriptableObject {
        public static string resourcesFolder = "Assets/Resources/";
        public List<Character> characters = new List<Character>();
        public DiplomataLib.Preferences preferences = new DiplomataLib.Preferences();
        public Inventory inventory = new Inventory();
        public CustomFlags customFlags = new CustomFlags();

        private int workingContextMessagesId;
        private int workingContextEditId;
        private string workingCharacter;
        private int workingItemId;

        public static void Instantiate() {
            if (DiplomataLib.Diplomata.instance == null && FindObjectsOfType<DiplomataLib.Diplomata>().Length < 1) {
                GameObject obj = new GameObject("[Diplomata]");
                obj.AddComponent<DiplomataLib.Diplomata>();
            }

            JSONHandler.CreateFolder("Diplomata/Characters/");
            
            if (!JSONHandler.Exists("preferences", "Diplomata/")) {
                JSONHandler.Create(new DiplomataLib.Preferences(), "preferences", false, "Diplomata/");
            }

            if (!JSONHandler.Exists("inventory", "Diplomata/")) {
                JSONHandler.Create(new Inventory(), "inventory", false, "Diplomata/");
            }

            if (!JSONHandler.Exists("customFlags", "Diplomata/")) {
                JSONHandler.Create(new CustomFlags(), "customFlags", false, "Diplomata/");
            }

            DiplomataLib.Diplomata.Restart();
            var diplomataEditor = CreateInstance<Diplomata>();

            if (!AssetHandler.Exists("Diplomata.asset", "Diplomata/")) {
                diplomataEditor.preferences = DiplomataLib.Diplomata.preferences;
                diplomataEditor.inventory = DiplomataLib.Diplomata.inventory;
                diplomataEditor.customFlags = DiplomataLib.Diplomata.customFlags;
                diplomataEditor.characters = DiplomataLib.Diplomata.characters;

                AssetHandler.Create(diplomataEditor, "Diplomata.asset", "Diplomata/");
            }

            else {
                diplomataEditor = (Diplomata) AssetHandler.Read("Diplomata.asset", "Diplomata/");
                diplomataEditor.preferences = JSONHandler.Read<DiplomataLib.Preferences>("preferences", "Diplomata/");
                diplomataEditor.inventory = JSONHandler.Read<Inventory>("inventory", "Diplomata/");
                diplomataEditor.customFlags = JSONHandler.Read<CustomFlags>("customFlags", "Diplomata/");
                diplomataEditor.UpdateList();
            }
        }

        public void Awake() {
            workingCharacter = string.Empty;
            workingContextMessagesId = -1;
            workingContextEditId = -1;
            workingItemId = -1;
        }

        public void UpdateList() {
            var charactersFiles = Resources.LoadAll("Diplomata/Characters/");

            characters = new List<Character>();
            preferences.characterList = new string[0];

            foreach (Object obj in charactersFiles) {
                var json = (TextAsset)obj;
                var character = JsonUtility.FromJson<Character>(json.text);

                characters.Add(character);
                preferences.characterList = ArrayHandler.Add(preferences.characterList, obj.name);
            }
        }

        public void AddCharacter(string name) {
            Character character = new Character(name);

            CheckRepeatedCharacter(character);
        }

        public void CheckRepeatedCharacter(Character character) {
            bool canAdd = true;

            foreach (string characterName in preferences.characterList) {
                if (characterName == character.name) {
                    canAdd = false;
                    break;
                }
            }

            if (canAdd) {
                characters.Add(character);

                if (characters.Count == 1) {
                    preferences.playerCharacterName = character.name;
                }

                preferences.characterList = ArrayHandler.Add(preferences.characterList, character.name);
                SavePreferences();

                JSONHandler.Create(character, character.name, preferences.jsonPrettyPrint, "Diplomata/Characters/");
            }

            else {
                Debug.LogError("This name already exists!");
            }
        }

        public void SavePreferences() {
            JSONHandler.Update(preferences, "preferences", preferences.jsonPrettyPrint, "Diplomata/");
        }

        public void SaveInventory() {
            JSONHandler.Update(inventory, "inventory", preferences.jsonPrettyPrint, "Diplomata/");
        }

        public void SaveCustomFlags() {
            JSONHandler.Update(customFlags, "customFlags", preferences.jsonPrettyPrint, "Diplomata/");
        }

        public void Save(Character character) {
            JSONHandler.Update(character, character.name, preferences.jsonPrettyPrint, "Diplomata/Characters/");
        }

        public void SetWorkingCharacter(string characterName) {
            workingCharacter = characterName;
        }

        public string GetWorkingCharacter() {
            return workingCharacter;
        }

        public void SetWorkingContextMessagesId(int contextId) {
            workingContextMessagesId = contextId;
        }

        public int GetWorkingContextMessagesId() {
            return workingContextMessagesId;
        }

        public void SetWorkingContextEditId(int contextId) {
            workingContextEditId = contextId;
        }

        public int GetWorkingContextEditId() {
            return workingContextEditId;
        }

        public void SetWorkingItemId(int itemId) {
            workingItemId = itemId;
        }

        public int GetWorkingItemId() {
            return workingItemId;
        }
    }

}