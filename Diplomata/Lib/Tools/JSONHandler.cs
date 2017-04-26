using System;
using System.IO;
using UnityEngine;

namespace DiplomataLib {
    
    public class JSONHandler {
        
        public static T Read<T>(string filename, string folder = "") {
            try {
                TextAsset json = (TextAsset)Resources.Load(folder + filename);

                if (json == null) {
                    json = (TextAsset)Resources.Load(filename);
                }

                return JsonUtility.FromJson<T>(json.text);
            }

            catch (Exception e) {
                Debug.LogError("Cannot read " + filename + " in Resources folder. " + e.Message);
                return default(T);
            }
        }

        public static void Create(System.Object obj, string filename, string folder = "") {
            #if UNITY_EDITOR
            try {
                CreateFolder(folder);

                var file = File.Create(Preferences.defaultResourcesFolder + folder + filename + ".json");
                file.Close();

                UnityEditor.AssetDatabase.Refresh();

                Update(obj, filename, folder);
            }

            catch (Exception e) {
                Debug.LogError("Cannot create " + folder + filename + ".json in " + Preferences.defaultResourcesFolder + ". " + e.Message);
            }
            #endif
        }


        public static void Update(System.Object obj, string filename, string folder = "") {
            #if UNITY_EDITOR
            try {
                string json = JsonUtility.ToJson(obj, Diplomata.preferences.jsonPrettyPrint);

                using (FileStream fs = new FileStream(Preferences.defaultResourcesFolder + folder + filename + ".json", FileMode.Create)) {
                    using (StreamWriter writer = new StreamWriter(fs)) {
                        writer.Write(json);
                    }
                }

                UnityEditor.AssetDatabase.Refresh();
            }

            catch (Exception e) {
                Debug.LogError("Cannot update " + filename + " in Resources folder. " + e.Message);
            }
            #endif
        }

        public static void Delete(string filename, string folder = "") {
            #if UNITY_EDITOR
            try {
                File.Delete(Preferences.defaultResourcesFolder + folder + filename + ".json");
                File.Delete(Preferences.defaultResourcesFolder + folder + filename + ".json.meta");
            }

            catch (Exception e) {
                Debug.LogError("Cannot delete " + filename + ".json. " + e.Message);
            }

            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        public static bool Exists(string filename, string folder = "") {
            #if UNITY_EDITOR
            if (!File.Exists(Preferences.defaultResourcesFolder + filename + ".json") &&
                !File.Exists(Preferences.defaultResourcesFolder + folder + filename + ".json")) {
                return false;
            }

            else {
                return true;
            }
            #else
            return false;
            #endif
        }

        public static void CreateFolder(string folderName) {
            #if UNITY_EDITOR
            if (!Directory.Exists(Preferences.defaultResourcesFolder + folderName) && folderName != "") {
                Directory.CreateDirectory(Preferences.defaultResourcesFolder + folderName);

                UnityEditor.AssetDatabase.Refresh();
            }
            #endif
        }

    }

}
