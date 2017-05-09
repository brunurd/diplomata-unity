using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class ContextEditor : EditorWindow {

        public static Character character;
        public static Context context;
        private Vector2 scrollPos = new Vector2(0, 0);
        private static Diplomata diplomataEditor;

        public enum State {
            None,
            Edit,
            Close
        }

        private static State state;

        public static void Init(State state = State.None) {
            DGUI.focusOnStart = true;
            ContextEditor.state = state;

            ContextEditor window = (ContextEditor)GetWindow(typeof(ContextEditor), false, "Context Editor", true);
            window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 170);
            
            if (state == State.Close || character == null) {
                window.Close();
            }

            else {
                window.Show();
            }
        }

        public void OnEnable() {
            diplomataEditor = (Diplomata) AssetHandler.Read("Diplomata.asset", "Diplomata/");
        }

        public static void Edit(Character currentCharacter, Context currentContext) {
            character = currentCharacter;
            context = currentContext;

            diplomataEditor = (Diplomata)AssetHandler.Read("Diplomata.asset", "Diplomata/");
            diplomataEditor.SetWorkingContextEditId(context.id);
            Init(State.Edit);
        }
        
        public static void Reset(string characterName) {
            if (character != null) {
                if (character.name == characterName) {
                    character = null;
                    context = null;

                    diplomataEditor = (Diplomata)AssetHandler.Read("Diplomata.asset", "Diplomata/");
                    diplomataEditor.SetWorkingContextEditId(-1);

                    Init(State.Close);
                }
            }
        }

        public void OnGUI() {
            DGUI.Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginVertical(DGUI.windowStyle);

            switch (state) {
                case State.None:
                    if (diplomataEditor.GetWorkingCharacter() != string.Empty) {
                        character = Character.Find(diplomataEditor.characters, diplomataEditor.GetWorkingCharacter());

                        if (diplomataEditor.GetWorkingContextEditId() > -1) {
                            context = Context.Find(character, diplomataEditor.GetWorkingContextEditId());
                            DrawEditWindow();
                        }
                    }
                    break;

                case State.Edit:
                    DrawEditWindow();
                    break;
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        
        public void DrawEditWindow() {
            var name = DictHandler.ContainsKey(context.name, diplomataEditor.preferences.currentLanguage);
            var description = DictHandler.ContainsKey(context.description, diplomataEditor.preferences.currentLanguage);

            if (name != null && description != null) {
                GUILayout.Label("Name: ");

                GUI.SetNextControlName("name");
                    name.value = EditorGUILayout.TextField(name.value);

                DGUI.Focus("name");

                EditorGUILayout.Separator();

                DGUI.textContent.text = description.value;
                var height = DGUI.textAreaStyle.CalcHeight(DGUI.textContent, Screen.width - (2 * DGUI.MARGIN));

                GUILayout.Label("Description: ");
                description.value = EditorGUILayout.TextArea(description.value, DGUI.textAreaStyle, GUILayout.Height(height));

                EditorGUILayout.Separator();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Update", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    UpdateContext();
                }

                if (GUILayout.Button("Cancel", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    UpdateContext();
                }
                GUILayout.EndHorizontal();
            }
        }

        public void UpdateContext() {
            diplomataEditor.Save(character);
            Close();
        }

        public void OnDisable() {
            if (character != null) {
                diplomataEditor.Save(character);
            }
        }
    }

}