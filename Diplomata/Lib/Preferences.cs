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

        public Language(Language other) {
            name = other.name;
            subtitle = other.subtitle;
            dubbing = other.dubbing;
        }
    }

    [System.Serializable]
    public class Preferences {
        public string[] attributes = new string[0];
        public Language[] languages = new Language[0];
        public static string defaultResourcesFolder = "Assets/Resources/";
        public string[] characterList = new string[0];
        public bool jsonPrettyPrint;
        public string workingCharacter;
        public string workingContext;
        public string playerCharacterName;
        
        public void Start() {
            if (!JSONHandler.Exists("preferences", "Diplomata/")) {
                attributes = new string[] { "fear", "politeness", "argumentation", "insistence", "charm", "confidence" };
                languages = new Language[] { new Language("English") };
                jsonPrettyPrint = false;
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
