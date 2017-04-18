using System.Collections.Generic;
using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    public class Character {
        public string name;
        public string description;
        public bool startOnPlay;
        public DictAttr[] attributes;
        public byte influence = 50;
        public Context[] contexts;

        [System.NonSerialized]
        public bool onScene;

        public Character() { }

        public Character(string name) {
            this.name = name;
            contexts = new Context[0];

            if (Diplomata.characters.Count == 0) {
                Diplomata.preferences.playerCharacterName = this.name;
            }

            SetAttributes();
            CheckRepeatedCharacter();
        }

        public void SetAttributes() {
            attributes = new DictAttr[0];

            foreach (string attrName in Diplomata.preferences.attributes) {
                attributes = ArrayHandler.Add(attributes, new DictAttr(attrName));
            }
        }

        public void CheckRepeatedCharacter() {
            bool canAdd = true;

            foreach (string characterName in Diplomata.preferences.characterList) {
                if (characterName == name) {
                    canAdd = false;
                    break;
                }
            }

            if (canAdd) {
                Diplomata.characters.Add(this);
                Diplomata.preferences.characterList = ArrayHandler.Add(Diplomata.preferences.characterList, name);
                JSONHandler.Update(Diplomata.preferences, "preferences", "Diplomata/");
                JSONHandler.Create(this, name, "Diplomata/Characters/");
            }

            else {
                Debug.LogError("This name already exists!");
            }
        }

        public static void UpdateList() {
            JSONHandler.CreateFolder("Diplomata/Characters/");
            var charactersFiles = Resources.LoadAll("Diplomata/Characters/");

            Diplomata.characters = new List<Character>();
            Diplomata.preferences.characterList = new string[0];

            foreach (Object obj in charactersFiles) {
                var json = (TextAsset)obj;
                var character = JsonUtility.FromJson<Character>(json.text);

                Diplomata.characters.Add(character);
                Diplomata.preferences.characterList = ArrayHandler.Add(Diplomata.preferences.characterList, obj.name);
            }

            var charactersOnScene = Object.FindObjectsOfType<DiplomataCharacter>();

            foreach (Character character in Diplomata.characters) {
                foreach (DiplomataCharacter diplomataCharacter in charactersOnScene) {
                    if (diplomataCharacter.character != null) {
                        if (character.name == diplomataCharacter.character.name) {
                            character.onScene = true;
                        }
                    }
                }
            }
        }

        public static Character Find(string name) {
            foreach (Character character in Diplomata.characters) {
                if (character.name == name) {
                    return character;
                }
            }

            return null;
        }

    }

}