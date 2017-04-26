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
        public static string defaultResourcesFolder = "Assets/Resources/";
        public bool jsonPrettyPrint;
        public Language[] languages = new Language[0];
        public string[] languagesList = new string[0];
        public string[] characterList = new string[0];
        public string[] attributes = new string[0];
        public string currentLanguage;
        public string playerCharacterName;
        
        
        public void Start() {
            if (!JSONHandler.Exists("preferences", "Diplomata/")) {
                attributes = new string[] { "fear", "politeness", "argumentation", "insistence", "charm", "confidence" };

                languages = new Language[] { new Language("English") };
                SetCurrentLanguage("English");
                jsonPrettyPrint = false;

                JSONHandler.Create(this, "preferences", "Diplomata/");
            }

            else {
                Diplomata.preferences = JSONHandler.Read<Preferences>("preferences", "Diplomata/");
            }
        }

        public void SetCurrentLanguage(string language) {
            currentLanguage = language;

            SetLanguageList();
        }

        public void SetLanguageList() {
            languagesList = new string[languages.Length];

            for (int i = 0; i < languages.Length; i++) {
                languagesList[i] = languages[i].name;
            }
        }
    }

}
