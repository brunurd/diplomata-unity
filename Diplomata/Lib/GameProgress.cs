using System.Xml.Serialization;
using System.IO;
using UnityEngine;

namespace DiplomataLib {

    public enum Method {
        XML,
        JSON
    }

    [System.Serializable]
    public class GameProgress {   
        public string currentSubtitledLanguage;
        public string currentDubbedLanguage;
        public Character[] characters;

        public void Start() {
            if (Diplomata.preferences.subLanguages.Count > 0) {
                currentSubtitledLanguage = Diplomata.preferences.subLanguages[0];
            }

            if (Diplomata.preferences.dubLanguages.Count > 0) {
                currentDubbedLanguage = Diplomata.preferences.dubLanguages[0];
            }

            UpdateCharacters();
        }

        public void UpdateCharacters() {
            characters = new Character[Diplomata.characters.Count];
            characters = Diplomata.ListToArray(Diplomata.characters);
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

        public void Save(Method method) {
            string str = string.Empty;
            string extension = string.Empty;

            UpdateCharacters();

            switch (method) {
                case Method.JSON:
                    str = Serialize(Method.JSON);
                    extension = ".json";
                    break;

                case Method.XML:
                    str = Serialize(Method.XML);
                    extension = ".xml";
                    break;
            }
            
            using (FileStream fs = new FileStream(Application.persistentDataPath + "/diplomata_gameProgress" + extension, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(fs)) {
                    writer.Write(str);
                }
            }
        }

        public void Deserialize(string data, Method method) {
            switch (method) {
                case Method.JSON:
                    Diplomata.gameProgress = JsonUtility.FromJson<GameProgress>(data);
                    break;

                case Method.XML:
                    var serializer = new XmlSerializer(typeof(GameProgress));

                    using (TextReader reader = new StringReader(data)) {
                        Diplomata.gameProgress = (GameProgress) serializer.Deserialize(reader);
                    }

                    break;
            }
        }

        public void Load(Method method) {
            string extension = string.Empty;
            string path = Application.persistentDataPath + "/diplomata_gameProgress" + extension;
            string content = string.Empty;

            switch (method) {
                case Method.JSON:
                    extension = ".json";
                    break;

                case Method.XML:
                    extension = ".xml";
                    break;
            }

            if (File.Exists(path)) {
                using (StreamReader sr = new StreamReader(path)) {
                    content = sr.ReadToEnd();
                }
            }

            switch (method) {
                case Method.JSON:
                    Deserialize(content, Method.JSON);
                    break;

                case Method.XML:
                    Deserialize(content, Method.XML);
                    break;
            }
        }
    }

}
