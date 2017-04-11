using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

namespace DiplomataLib {

    public enum Method {
        XML,
        JSON,
        Binary
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

        public T Serialize<T>(Method method) {
            System.Object obj;

            switch (method) {
                case Method.JSON:
                    obj = JsonUtility.ToJson(Diplomata.gameProgress);
                    break;

                case Method.XML:
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameProgress));

                    using (StringWriter textWriter = new StringWriter()) {
                        xmlSerializer.Serialize(textWriter, Diplomata.gameProgress);
                        obj = textWriter.GetStringBuilder().ToString();
                        break;
                    }

                case Method.Binary:
                    MemoryStream stream = new MemoryStream();
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, typeof(GameProgress));
                    obj = stream;
                    break;

                default:
                    obj = null;
                    break;
            }

            return (T)obj;
        }

        ///<summary>
        /// Save a file in Application.persistentDataPath
        /// <para>Choose the type of file: Method.JSON, Method.XML or Method.Binary. (default: Method.Binary)</para>
        ///</summary>

        public void Save(Method method = Method.Binary) {
            string str = "null";
            MemoryStream stream = new MemoryStream();
            string extension = ".sav";

            UpdateCharacters();

            switch (method) {
                case Method.JSON:
                    str = Serialize<string>(Method.JSON);
                    extension = ".json";
                    break;

                case Method.XML:
                    str = Serialize<string>(Method.XML);
                    extension = ".xml";
                    break;

                case Method.Binary:
                    stream = Serialize<MemoryStream>(Method.Binary);
                    break;
            }

            using (FileStream fs = new FileStream(Application.persistentDataPath + "/diplomata_gameProgress" + extension, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(fs)) {
                    if (method == Method.Binary) {
                        stream.WriteTo(fs);
                    }
                    else {
                        writer.Write(str);
                    }
                }
            }
        }

        /*
        public void JSONDeserialize(string path) {
            var file = new StreamReader(path);
            var content = file.ReadToEnd();
            Diplomata.gameProgress = JsonUtility.FromJson<GameProgress>(content);
        }

        public void XMLDeserialize(string path) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameProgress));

            Stream file = File.Open(path, FileMode.Open);
            XmlReader reader = XmlReader.Create(file);
            Diplomata.gameProgress = (GameProgress) xmlSerializer.Deserialize(reader);
            file.Close();
        }

        public void BinaryDeserialize(string path) {

        }*/
    }

}
