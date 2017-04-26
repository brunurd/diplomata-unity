using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class ContextEditor : EditorWindow {

        public static Character character;
        public static Context context;
        private Vector2 scrollPos = new Vector2(0, 0);
        private static EditorData editorData;

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
            editorData = (EditorData)AssetHandler.Read<EditorData>("editorData.asset", "Diplomata/");
        }

        public static void Edit(Character currentCharacter, Context currentContext) {
            character = currentCharacter;
            context = currentContext;
            editorData = (EditorData)AssetHandler.Read<EditorData>("editorData.asset", "Diplomata/");
            editorData.SetWorkingContextEditId(context.id);

            Init(State.Edit);
        }
        
        public static void Reset(string characterName) {
            if (character != null) {
                if (character.name == characterName) {
                    character = null;
                    context = null;
                    editorData.SetWorkingContextEditId(-1);

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
                    if (editorData.workingCharacter != string.Empty) {
                        character = Character.Find(editorData.workingCharacter);

                        if (editorData.workingContextEditId > -1) {
                            context = Context.Find(character, editorData.workingContextEditId);
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
            var name = DictHandler.ContainsKey(context.name, Diplomata.preferences.currentLanguage);
            var description = DictHandler.ContainsKey(context.description, Diplomata.preferences.currentLanguage);

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
            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
            Close();
        }

        public void OnDisable() {
            if (character != null) {
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
            }
        }
    }

}