using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {
    
    public class Preferences : EditorWindow {

        public static string[] attributesTemp = new string[0];
        public static Language[] languagesTemp = new Language[0];
        public static bool jsonPrettyPrintTemp = false;
        public static string currentLanguageTemp;
        private Vector2 scrollPos = new Vector2(0, 0);
        private static Diplomata diplomataEditor;
        
        [MenuItem("Diplomata/Preferences")]
        static public void Init() {
            Diplomata.Instantiate();
            diplomataEditor = (Diplomata)AssetHandler.Read("Diplomata.asset", "Diplomata/");

            attributesTemp = ArrayHandler.Copy(diplomataEditor.preferences.attributes);
            languagesTemp = ArrayHandler.Copy(diplomataEditor.preferences.languages);
            jsonPrettyPrintTemp = diplomataEditor.preferences.jsonPrettyPrint;
            currentLanguageTemp = string.Copy(diplomataEditor.preferences.currentLanguage);

            Preferences window = (Preferences)GetWindow(typeof(Preferences), false, "Preferences");
            window.minSize = new Vector2(600, 325);
            window.Show();
        }

        public void OnGUI() {
            DGUI.Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginVertical(DGUI.windowStyle);
            
            GUILayout.BeginHorizontal();
            DrawAttributes();
            DrawLanguages();
            GUILayout.EndHorizontal();

            DGUI.Separator();
                
            GUILayout.BeginHorizontal();
            jsonPrettyPrintTemp = GUILayout.Toggle(jsonPrettyPrintTemp, "JSON pretty print");
            currentLanguageTemp = DGUI.Popup("Current Language", currentLanguageTemp, diplomataEditor.preferences.languagesList);
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();
                
            EditorGUILayout.HelpBox("\nClose or enter in play mode will restore the data of this window.\n", MessageType.Info);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Save Preferences", GUILayout.Height(30))) {
                Save();
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public void DrawAttributes() {
            GUILayout.BeginVertical(GUILayout.Width(Screen.width / 2));

            GUILayout.Label("Attributes:");

            for (int i = 0; i < attributesTemp.Length; i++) {
                GUILayout.BeginHorizontal();

                attributesTemp[i] = EditorGUILayout.TextField(attributesTemp[i]);

                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    attributesTemp = ArrayHandler.Remove(attributesTemp, attributesTemp[i]);
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Add attribute")) {
                attributesTemp = ArrayHandler.Add(attributesTemp, "");
            }

            GUILayout.EndVertical();
        }

        public void DrawLanguages() {
            GUILayout.BeginVertical();

            GUILayout.Label("Languages:");

            for (int i = 0; i < languagesTemp.Length; i++) {
                GUILayout.BeginHorizontal();

                languagesTemp[i].name = EditorGUILayout.TextField(languagesTemp[i].name);
                languagesTemp[i].subtitle = GUILayout.Toggle(languagesTemp[i].subtitle, "Sub");
                languagesTemp[i].dubbing = GUILayout.Toggle(languagesTemp[i].dubbing, "Dub");

                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    languagesTemp = ArrayHandler.Remove(languagesTemp, languagesTemp[i]);
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Add language")) {
                languagesTemp = ArrayHandler.Add(languagesTemp, new Language(""));
            }

            GUILayout.EndVertical();
        }

        public void Save() {
            diplomataEditor.preferences.attributes = ArrayHandler.Copy(attributesTemp);
            diplomataEditor.preferences.languages = ArrayHandler.Copy(languagesTemp);
            diplomataEditor.preferences.jsonPrettyPrint = jsonPrettyPrintTemp;
            diplomataEditor.preferences.currentLanguage = string.Copy(currentLanguageTemp);
            diplomataEditor.preferences.SetLanguageList();

            diplomataEditor.SavePreferences();
            Close();
        }
    }

}