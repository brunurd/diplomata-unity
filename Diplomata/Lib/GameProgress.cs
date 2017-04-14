using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
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
            if (Diplomata.preferences.languages.Count > 0) {
                currentDubbedLanguage = Diplomata.preferences.languages[0].name;
                currentSubtitledLanguage = Diplomata.preferences.languages[0].name;
            }

            UpdateCharacters();
        }

        public void UpdateCharacters() {
            characters = new Character[Diplomata.characters.Count];
            characters = ArrayHandler.ListToArray(Diplomata.characters);
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

        public void Save(string extension = ".sav") {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
           
            using (FileStream fileStream = new FileStream(Application.persistentDataPath + "/diplomata_gameProgress" + extension, FileMode.Create)) {
                binaryFormatter.Serialize(fileStream, Diplomata.gameProgress);
            }
        }

        public IEnumerator SaveWeb(string url, string extension = ".sav") {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            WWWForm form = new WWWForm();

            using (MemoryStream stream = new MemoryStream()) {
                byte[] bytes = new byte[stream.Length];

                binaryFormatter.Serialize(stream, Diplomata.gameProgress);
                stream.Position = 0;

                using (StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(bytes);
                }

                form.AddBinaryData("fileUpload", bytes, "diplomata_gameProgress" + extension, "text/plain");
            }

            WWW www = new WWW(url + "/diplomata_gameProgress" + extension, form);
            yield return www;

            if (www.error == null) {
                Debug.Log("Upload done: " + www.text);
            }

            else {
                Debug.Log("Error during upload: " + www.error);
            }
        }

        public void Load(string extension = ".sav") {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            
            using (FileStream fileStream = new FileStream(Application.persistentDataPath + "/diplomata_gameProgress" + extension, FileMode.Open)) {
                Diplomata.gameProgress = (GameProgress) binaryFormatter.Deserialize(fileStream);
            }
        }
    }

}
