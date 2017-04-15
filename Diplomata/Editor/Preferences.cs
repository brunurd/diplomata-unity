using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {
    
    public class Preferences : EditorWindow {

        public static List<string> attributesTemp;
        public static List<Language> languagesTemp;
        public static string defaultResourcesFolderTemp;
        public static bool jsonPrettyPrintTemp;

        [MenuItem("Diplomata/Preferences")]
        static public void Init() {
            Diplomata.Instantiate();

            attributesTemp = new List<string>(Diplomata.preferences.attributes);
            languagesTemp = new List<Language>(Diplomata.preferences.languages);
            defaultResourcesFolderTemp = DiplomataLib.Preferences.defaultResourcesFolder;
            jsonPrettyPrintTemp = Diplomata.preferences.jsonPrettyPrint;

            Preferences window = (Preferences)GetWindow(typeof(Preferences), false, "Preferences");
            window.minSize = new Vector2(600, 345);
            window.Show();
        }

        public void OnGUI() {
            DGUI.WindowWrap(() => {
                GUILayout.Label("Default Resources folder:");
                defaultResourcesFolderTemp = EditorGUILayout.TextField(defaultResourcesFolderTemp);

                EditorGUILayout.Separator();

                DGUI.Horizontal(() => {
                    DrawAttributes();
                    DrawLanguages();
                });

                EditorGUILayout.Separator();

                jsonPrettyPrintTemp = GUILayout.Toggle(jsonPrettyPrintTemp, "JSON pretty print");

                EditorGUILayout.Separator();
                
                EditorGUILayout.HelpBox("\nClose or enter in play mode will restore the data of this window.\n", MessageType.Info);

                EditorGUILayout.Separator();

                if (GUILayout.Button("Save Preferences", GUILayout.Height(30))) {
                    Save();
                }
            });
        }

        public void DrawAttributes() {
            DGUI.Vertical(() => { 

                GUILayout.Label("Attributes:");

                for (int i = 0; i < attributesTemp.Count; i++) {
                    DGUI.Horizontal(() => {

                        attributesTemp[i] = EditorGUILayout.TextField(attributesTemp[i]);

                        if (GUILayout.Button("X", GUILayout.Width(20))) {
                            attributesTemp.Remove(attributesTemp[i]);
                        }

                    });
                }

                if (GUILayout.Button("Add attribute")) {
                    attributesTemp.Add("");
                }

            }, GUILayout.Width(Screen.width / 2));
        }

        public void DrawLanguages() {
            DGUI.Vertical(() => {

                GUILayout.Label("Languages:");

                for (int i = 0; i < languagesTemp.Count; i++) {
                    DGUI.Horizontal(() => {

                        languagesTemp[i].name = EditorGUILayout.TextField(languagesTemp[i].name);
                        languagesTemp[i].subtitle = GUILayout.Toggle(languagesTemp[i].subtitle, "Sub");
                        languagesTemp[i].dubbing = GUILayout.Toggle(languagesTemp[i].dubbing, "Dub");

                        if (GUILayout.Button("X", GUILayout.Width(20))) {
                            languagesTemp.Remove(languagesTemp[i]);
                        }
                    });
                }

                if (GUILayout.Button("Add language")) {
                    languagesTemp.Add(new Language(""));
                }

            });
        }

        public void Save() {
            Diplomata.preferences.attributes = new List<string>(attributesTemp);
            Diplomata.preferences.languages = new List<Language>(languagesTemp);
            DiplomataLib.Preferences.defaultResourcesFolder = defaultResourcesFolderTemp;
            Diplomata.preferences.jsonPrettyPrint = jsonPrettyPrintTemp;

            MessagesEditor.SetLanguagesList();

            JSONHandler.Update(Diplomata.preferences, "preferences", "Diplomata/");
            Close();
        }
    }

}