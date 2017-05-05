using System.Collections.Generic;
using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    public class Character {
        public string name;
        public bool startOnPlay;
        public DictLang[] description;
        public DictAttr[] attributes;
        public Context[] contexts;
        public byte influence = 50;

        [System.NonSerialized]
        public bool onScene;
        
        public Character() { }

        public Character(string name) {
            this.name = name;
            contexts = new Context[0];
            description = new DictLang[0];
            
            foreach (Language lang in Diplomata.preferences.languages) {
                description = ArrayHandler.Add(description, new DictLang(lang.name, ""));
            }

            SetAttributes();
        }

        public void SetAttributes() {
            attributes = new DictAttr[0];

            foreach (string attrName in Diplomata.preferences.attributes) {
                attributes = ArrayHandler.Add(attributes, new DictAttr(attrName));
            }
        }

        public static void UpdateList() {
            var charactersFiles = Resources.LoadAll("Diplomata/Characters/");

            Diplomata.characters = new List<Character>();
            Diplomata.preferences.characterList = new string[0];

            foreach (Object obj in charactersFiles) {
                var json = (TextAsset)obj;
                var character = JsonUtility.FromJson<Character>(json.text);

                Diplomata.characters.Add(character);
                Diplomata.preferences.characterList = ArrayHandler.Add(Diplomata.preferences.characterList, obj.name);
            }

            SetOnScene();
        }

        public static void SetOnScene() {
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

        public static Character Find(List<Character> characters, string name) {
            foreach (Character character in characters) {
                if (character.name == name) {
                    return character;
                }
            }
            
            return null;
        }

    }

}