using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace DiplomataEditor {

    [Serializable]
    public class Preferences : EditorWindow {

        public static Diplomata.Preferences preferences;
        public static Texture logo;
        public static string resPath = "Assets/Diplomata/Resources/";
        public static List<string> attributes = new List<string>();
        public static List<string> subLanguages = new List<string>();
        public static List<string> dubLanguages = new List<string>();

        [MenuItem("Diplomata/Preferences")]
        static public void Init() {
            GameObject obj = new GameObject("Diplomata");
            obj.hideFlags = HideFlags.HideInHierarchy;
            obj.AddComponent<Diplomata.Manager>();

            GetPrefsStrings();

            Preferences window = (Preferences)GetWindow(typeof(Preferences), false, "Preferences");
            window.minSize = new Vector2(455,250);
            window.Show();
        }

        public void OnGUI() {

            int margin = 15;

            GUILayout.Space(margin);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3 - 5));
            GUILayout.Label("Attributes: ");
            for (int i = 0; i < attributes.Count; i++) {
                GUILayout.BeginHorizontal();
                attributes[i] = GUILayout.TextField(attributes[i]);
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    attributes.Remove(attributes[i]);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add Attribute")) {
                attributes.Add("");
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3 - 5));
            GUILayout.Label("Sub. languages: ");
            for (int i = 0; i < subLanguages.Count; i++) {
                GUILayout.BeginHorizontal();
                subLanguages[i] = GUILayout.TextField(subLanguages[i]);
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    subLanguages.Remove(subLanguages[i]);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add language")) {
                subLanguages.Add("");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3 - 5));
            GUILayout.Label("Dub. languages: ");
            for (int i = 0; i < dubLanguages.Count; i++) {
                GUILayout.BeginHorizontal();
                dubLanguages[i] = GUILayout.TextField(dubLanguages[i]);
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    dubLanguages.Remove(dubLanguages[i]);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add language")) {
                dubLanguages.Add("");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(margin);
            if (GUILayout.Button("Save Preferences", GUILayout.Height(50))) {
                SetPrefsStrings();
                Diplomata.Manager.UpdatePreferences();
            }
        }

        public string[] ListToStringArray(List<string> list) {
            string[] array = new string[list.Count];

            for (int i = 0; i < list.Count; i++) {
                array[i] = list[i];
            }

            return array;
        }

        public static void LoadJson() {
            TextAsset json = (TextAsset)Resources.Load("preferences");
            resPath = AssetDatabase.GetAssetPath(json);
            preferences = JsonUtility.FromJson<Diplomata.Preferences>(json.text);
        }

        public void SaveJson() {
            string json = JsonUtility.ToJson(preferences);
            using (FileStream fs = new FileStream(resPath, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(fs)) {
                    writer.Write(json);
                }
            }
            AssetDatabase.Refresh();
        }

        public static void GetPrefsStrings() {
            LoadJson();

            attributes = new List<string>();
            subLanguages = new List<string>();
            dubLanguages = new List<string>();

            foreach (string str in preferences.attributes) {
                attributes.Add(str);
            }

            foreach (string str in preferences.subLanguages) {
                subLanguages.Add(str);
            }

            foreach (string str in preferences.dubLanguages) {
                dubLanguages.Add(str);
            }
        }

        public void SetPrefsStrings() {

            preferences.attributes = ListToStringArray(attributes);
            preferences.subLanguages = ListToStringArray(subLanguages);
            preferences.dubLanguages = ListToStringArray(dubLanguages);

            SaveJson();
        }
    }

}