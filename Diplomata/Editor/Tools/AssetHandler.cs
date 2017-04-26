using UnityEditor;
using UnityEngine;

namespace DiplomataEditor {

    public class AssetHandler {
        
        public static void Create(Object obj, string name, string folder) {
            string path = DiplomataLib.Preferences.defaultResourcesFolder + folder + name;

            try {
                AssetDatabase.CreateFolder(DiplomataLib.Preferences.defaultResourcesFolder, folder);
                AssetDatabase.CreateAsset(obj, path);
                AssetDatabase.Refresh();
            }

            catch (System.Exception e) {
                Debug.LogError("Cannot create this asset. Review the path: \"" + path + "\". " + e.Message);
            }
        }

        public static Object Read<T>(string name, string folder) {
            string path = DiplomataLib.Preferences.defaultResourcesFolder + folder + name;

            try {
                return AssetDatabase.LoadAssetAtPath(path, typeof(T));
            }

            catch (System.Exception e) {
                Debug.LogError("This asset doesn't exist. Review the path: \"" + path + "\". " + e.Message);
                return null;
            }
        }

        public static void Update(Object obj, string name, string folder) {
            string path = DiplomataLib.Preferences.defaultResourcesFolder + folder + name;

            try {
                AssetDatabase.AddObjectToAsset(obj, path);
                AssetDatabase.Refresh();
            }

            catch (System.Exception e) {
                Debug.LogError("Cannot update this asset. Review the path: \"" + path + "\". " + e.Message);
            }
        }

        public static void Delete(string name, string folder) {
            string path = DiplomataLib.Preferences.defaultResourcesFolder + folder + name;

            try {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.Refresh();
            }

            catch (System.Exception e) {
                Debug.LogError("Cannot access the path: \"" + path + "\". " + e.Message);
            }
        }

    }

}