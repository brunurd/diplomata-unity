using System.IO;
using UnityEngine;

namespace Diplomata {

    [System.Serializable]
    [ExecuteInEditMode]
    public class Preferences {
        public static string[] attributes;
        public static string[] subLanguages;
        public static string[] dubLanguages;
        public static string resPath;

        public Preferences() {
            if ((TextAsset)Resources.Load("preferences")) {
                LoadJSON();
            }

            else {
                attributes = new string[] { "fear", "politeness", "argumentation", "insistence", "charm", "confidence" };
                subLanguages = new string[] { "English" };
                dubLanguages = new string[] { "English" };
                resPath = "Assets/Resources/Diplomata";
                SaveJSON(this);
            }
        }

        static public Preferences LoadJSON() {
            TextAsset json = (TextAsset)Resources.Load("preferences");

            #if UNITY_EDITOR
            resPath = UnityEditor.AssetDatabase.GetAssetPath(json);
            #endif

            return JsonUtility.FromJson<Preferences>(json.text);
        }

        static public void SaveJSON(Preferences preferences) {
            string json = JsonUtility.ToJson(preferences);

            using (FileStream fs = new FileStream(resPath, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(fs)) {
                    writer.Write(json);
                }
            }

            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }
    }

}
