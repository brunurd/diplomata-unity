using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {
    
    public class Preferences : EditorWindow {

        [MenuItem("Diplomata/Preferences")]
        static public void Init() {
            Diplomata.Instantiate();

            Preferences window = (Preferences)GetWindow(typeof(Preferences), false, "Preferences");
            window.minSize = new Vector2(600, 290);
            window.Show();
        }

        public void OnGUI() {
            DGUI.WindowWrap(() => {
                GUILayout.Label("Default Resources folder:");
                DiplomataLib.Preferences.defaultResourcesFolder = EditorGUILayout.TextField(DiplomataLib.Preferences.defaultResourcesFolder);

                EditorGUILayout.Separator();

                DGUI.Horizontal(() => {
                    DrawAttributes();
                    DrawLanguages();
                });

                EditorGUILayout.Separator();

                Diplomata.preferences.jsonPrettyPrint = GUILayout.Toggle(Diplomata.preferences.jsonPrettyPrint, "JSON pretty print");

                EditorGUILayout.Separator();

                if (GUILayout.Button("Save Preferences", GUILayout.Height(30))) {
                    JSONHandler.Update(Diplomata.preferences, "preferences", "Diplomata/");
                }
            });
        }

        public void DrawAttributes() {
            DGUI.Vertical(() => { 

                GUILayout.Label("Attributes:");

                for (int i = 0; i < Diplomata.preferences.attributes.Count; i++) {
                    DGUI.Horizontal(() => {

                        Diplomata.preferences.attributes[i] = EditorGUILayout.TextField(Diplomata.preferences.attributes[i]);

                        if (GUILayout.Button("X", GUILayout.Width(20))) {
                            Diplomata.preferences.attributes.Remove(Diplomata.preferences.attributes[i]);
                        }

                    });
                }

                if (GUILayout.Button("Add attribute")) {
                    Diplomata.preferences.attributes.Add("");
                }

            }, GUILayout.Width(Screen.width / 2));
        }

        public void DrawLanguages() {
            DGUI.Vertical(() => {

                GUILayout.Label("Languages:");

                for (int i = 0; i < Diplomata.preferences.languages.Count; i++) {
                    DGUI.Horizontal(() => {

                        Diplomata.preferences.languages[i].name = EditorGUILayout.TextField(Diplomata.preferences.languages[i].name);
                        Diplomata.preferences.languages[i].subtitle = GUILayout.Toggle(Diplomata.preferences.languages[i].subtitle, "Sub");
                        Diplomata.preferences.languages[i].dubbing = GUILayout.Toggle(Diplomata.preferences.languages[i].dubbing, "Dub");

                        if (GUILayout.Button("X", GUILayout.Width(20))) {
                            Diplomata.preferences.languages.Remove(Diplomata.preferences.languages[i]);
                        }
                    });
                }

                if (GUILayout.Button("Add language")) {
                    Diplomata.preferences.languages.Add(new Language(""));
                }

            });
        }

        private void OnDisable() {
            JSONHandler.Update(Diplomata.preferences, "preferences", "Diplomata/");
        }
    }

}