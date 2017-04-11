using System.Collections.Generic;

namespace DiplomataLib {

    [System.Serializable]
    public class Preferences {
        public List<string> attributes;
        public List<string> subLanguages;
        public List<string> dubLanguages;
        public static string defaultResourcesFolder = "Assets/Resources/";
        public List<string> characterList;
        
        public void Start() {
            if (!JSONHandler.Exists("preferences", "Diplomata/")) {
                attributes = new List<string>() { "fear", "politeness", "argumentation", "insistence", "charm", "confidence" };
                subLanguages = new List<string>() { "English" };
                dubLanguages = new List<string>() { "English" };
                characterList = new List<string>();

                JSONHandler.Create(this, "preferences", "Diplomata/");
            }

            else {
                Diplomata.preferences = JSONHandler.Read<Preferences>("preferences", "Diplomata/");
            }
        }
    }

}
