using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {
    
    public class Preferences : EditorWindow {

        public static string[] attributesTemp = new string[0];
        public static Language[] languagesTemp = new Language[0];
        public static string defaultResourcesFolderTemp = "";
        public static bool jsonPrettyPrintTemp = false;
        public static string currentLanguageTemp; 
        
        [MenuItem("Diplomata/Preferences")]
        static public void Init() {
            Diplomata.Instantiate();

            attributesTemp = ArrayHandler.Copy(Diplomata.preferences.attributes);
            languagesTemp = ArrayHandler.Copy(Diplomata.preferences.languages);
            defaultResourcesFolderTemp = string.Copy(DiplomataLib.Preferences.defaultResourcesFolder);
            jsonPrettyPrintTemp = Diplomata.preferences.jsonPrettyPrint;
            currentLanguageTemp = string.Copy(Diplomata.preferences.currentLanguage);

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

                var selected = 0;

                for (int i = 0; i < Diplomata.preferences.languagesList.Length; i++) {
                    if (Diplomata.preferences.languagesList[i] == currentLanguageTemp) {
                        selected = i;
                    }
                }

                DGUI.Horizontal(() => {
                    jsonPrettyPrintTemp = GUILayout.Toggle(jsonPrettyPrintTemp, "JSON pretty print");
                    selected = EditorGUILayout.Popup("Current Language", selected, Diplomata.preferences.languagesList);
                });

                for (int i = 0; i < Diplomata.preferences.languagesList.Length; i++) {
                    if (selected == i) {
                        currentLanguageTemp = Diplomata.preferences.languagesList[i];
                    }
                }

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

                for (int i = 0; i < attributesTemp.Length; i++) {
                    DGUI.Horizontal(() => {

                        attributesTemp[i] = EditorGUILayout.TextField(attributesTemp[i]);

                        if (GUILayout.Button("X", GUILayout.Width(20))) {
                            attributesTemp = ArrayHandler.Remove(attributesTemp, attributesTemp[i]);
                        }

                    });
                }

                if (GUILayout.Button("Add attribute")) {
                    attributesTemp = ArrayHandler.Add(attributesTemp, "");
                }

            }, GUILayout.Width(Screen.width / 2));
        }

        public void DrawLanguages() {
            DGUI.Vertical(() => {

                GUILayout.Label("Languages:");

                for (int i = 0; i < languagesTemp.Length; i++) {
                    DGUI.Horizontal(() => {

                        languagesTemp[i].name = EditorGUILayout.TextField(languagesTemp[i].name);
                        languagesTemp[i].subtitle = GUILayout.Toggle(languagesTemp[i].subtitle, "Sub");
                        languagesTemp[i].dubbing = GUILayout.Toggle(languagesTemp[i].dubbing, "Dub");

                        if (GUILayout.Button("X", GUILayout.Width(20))) {
                            languagesTemp = ArrayHandler.Remove(languagesTemp, languagesTemp[i]);
                        }
                    });
                }

                if (GUILayout.Button("Add language")) {
                    languagesTemp = ArrayHandler.Add(languagesTemp, new Language(""));
                }

            });
        }

        public void Save() {
            Diplomata.preferences.attributes = ArrayHandler.Copy(attributesTemp);
            Diplomata.preferences.languages = ArrayHandler.Copy(languagesTemp);
            Diplomata.preferences.jsonPrettyPrint = jsonPrettyPrintTemp;

            DiplomataLib.Preferences.defaultResourcesFolder = string.Copy(defaultResourcesFolderTemp);
            Diplomata.preferences.currentLanguage = string.Copy(currentLanguageTemp);

            Diplomata.preferences.SetLanguageList();
            
            JSONHandler.Update(Diplomata.preferences, "preferences", "Diplomata/");
            Close();
        }
    }

}