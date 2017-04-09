using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DiplomataEditor {
    
    public class Preferences : EditorWindow {

        public readonly byte MARGIN = 15;

        public static List<string> attributes = new List<string>();
        public static List<string> subLanguages = new List<string>();
        public static List<string> dubLanguages = new List<string>();

        [MenuItem("Diplomata/Preferences")]
        static public void Init() {
            Manager.Instantiate();

            GetPrefsStrings();

            Preferences window = (Preferences)GetWindow(typeof(Preferences), false, "Preferences");
            window.minSize = new Vector2(455,250);
            window.Show();
        }

        public void OnGUI() {

            GUILayout.Space(MARGIN);
            GUILayout.BeginHorizontal();

            // ATTRIBUTES COLUNM

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

            // SUBTITLE LANGUAGES COLUNM

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

            // DUBBED LANGUAGES COLUNM

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

            // SAVE BUTTON

            GUILayout.Space(MARGIN);
            if (GUILayout.Button("Save Preferences", GUILayout.Height(50))) {
                SetPrefsStrings();
                Diplomata.Preferences.SaveJSON(Manager.preferences);
            }
        }

        public static void GetPrefsStrings() {
            attributes = new List<string>();
            subLanguages = new List<string>();
            dubLanguages = new List<string>();

            foreach (string str in Diplomata.Preferences.attributes) {
                attributes.Add(str);
            }

            foreach (string str in Diplomata.Preferences.subLanguages) {
                subLanguages.Add(str);
            }

            foreach (string str in Diplomata.Preferences.dubLanguages) {
                dubLanguages.Add(str);
            }
        }
        
        public void SetPrefsStrings() {
            Diplomata.Preferences.attributes = ListToStringArray(attributes);
            Diplomata.Preferences.subLanguages = ListToStringArray(subLanguages);
            Diplomata.Preferences.dubLanguages = ListToStringArray(dubLanguages);
            Diplomata.Preferences.SaveJSON(Manager.preferences);
        }

        public string[] ListToStringArray(List<string> list) {
            string[] array = new string[list.Count];

            for (int i = 0; i < list.Count; i++) {
                array[i] = list[i];
            }

            return array;
        }
    }

}