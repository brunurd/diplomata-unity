using System.Collections.Generic;

namespace DiplomataLib {

    [System.Serializable]
    public class Language {
        public string name;
        public bool subtitle;
        public bool dubbing;

        public Language(string name) {
            this.name = name;
            subtitle = true;
            dubbing = true;
        }
    }

    [System.Serializable]
    public class Preferences {
        public List<string> attributes;
        public List<Language> languages;
        public static string defaultResourcesFolder = "Assets/Resources/";
        public List<string> characterList;
        public bool jsonPrettyPrint;
        public string workingCharacter;
        public string workingContext;
        
        public void Start() {
            if (!JSONHandler.Exists("preferences", "Diplomata/")) {
                attributes = new List<string>() { "fear", "politeness", "argumentation", "insistence", "charm", "confidence" };
                languages = new List<Language>() { new Language("English") };
                characterList = new List<string>();
                jsonPrettyPrint = true;
                workingCharacter = string.Empty;
                workingContext = string.Empty;

                JSONHandler.Create(this, "preferences", "Diplomata/");
            }

            else {
                Diplomata.preferences = JSONHandler.Read<Preferences>("preferences", "Diplomata/");
            }
        }

        public void SetWorkingCharacter(string characterName) {
            workingCharacter = characterName;
            JSONHandler.Update(this, "preferences", "Diplomata/");
        }

        public void SetWorkingContext(string contextName) {
            workingContext = contextName;
            JSONHandler.Update(this, "preferences", "Diplomata/");
        }
    }

}
