using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DiplomataLib;

namespace DiplomataEditor {
    
    public class Preferences : EditorWindow {

        public readonly byte MARGIN = 15;

        [MenuItem("Diplomata/Preferences")]
        static public void Init() {
            Diplomata.Instantiate();

            Preferences window = (Preferences)GetWindow(typeof(Preferences), false, "Preferences");
            window.minSize = new Vector2(455,285);
            window.Show();
        }

        public void OnGUI() {

            GUILayout.Space(MARGIN);

            GUILayout.Label("Default Resources folder:");
            DiplomataLib.Preferences.defaultResourcesFolder = GUILayout.TextField(DiplomataLib.Preferences.defaultResourcesFolder);
            GUILayout.Space(MARGIN);

            GUILayout.BeginHorizontal();
            
            ShowColunm(Diplomata.preferences.attributes, "Attributes:", "Add attribute");
            ShowColunm(Diplomata.preferences.subLanguages, "Sub. Languages:", "Add language");
            ShowColunm(Diplomata.preferences.dubLanguages, "Dub. Languages:", "Add language");

            GUILayout.EndHorizontal();
            
            GUILayout.Space(MARGIN);
            if (GUILayout.Button("Save Preferences", GUILayout.Height(30))) {
                JSONHandler.Update(Diplomata.preferences, "preferences", "Diplomata/");
            }
        }

        public void ShowColunm(List<string> list, string label, string addLabel) {
            GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3 - 5));

            GUILayout.Label(label);

            for (int i = 0; i < list.Count; i++) {
                GUILayout.BeginHorizontal();

                    list[i] = GUILayout.TextField(list[i]);

                    if (GUILayout.Button("X", GUILayout.Width(20))) {
                        list.Remove(list[i]);
                    }

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button(addLabel)) {
                list.Add("");
            }
            GUILayout.EndVertical();
        }
    }

}