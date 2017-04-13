using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class MessagesManager : EditorWindow {

        public static Character character;
        public static Context context;

        private Vector2 scrollPos = new Vector2(0, 0);

        public enum State {
            None,
            Context,
            Messages,
            Close
        }

        private static State state;

        public static void Init(State state = State.None) {
            MessagesManager.state = state;
            MessagesManager window = (MessagesManager)GetWindow(typeof(MessagesManager), false, "Messages", true);
            window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 300);
            window.maximized = true;

            if (state == State.Close) {
                window.Close();
            }

            else {
                window.Show();
            }
        }

        public static void ContextMenu(Character currentCharacter) {
            character = currentCharacter;
            Diplomata.preferences.SetWorkingCharacter(currentCharacter.name);
            Diplomata.preferences.workingContext = string.Empty;
            Init(State.Context);
        }

        public static void MessagesMenu(Context currentContext) {
            context = currentContext;
            Diplomata.preferences.workingContext = currentContext.name;
            Init(State.Messages);
        }

        public static void Reset(string characterName) {
            if (character != null) {
                if (character.name == characterName) {
                    Diplomata.preferences.SetWorkingCharacter(string.Empty);
                    character = null;
                    Init(State.Close);
                }
            }
        }

        public void OnGUI() {
            DGUI.WindowWrap(() => {

                switch (state) {
                    case State.None:
                        if (Diplomata.preferences.workingCharacter != string.Empty) {
                            character = Diplomata.FindCharacter(Diplomata.preferences.workingCharacter);

                            if (Diplomata.preferences.workingContext != string.Empty) {
                                DrawMessagesMenu();
                            }

                            else {
                                DrawContextMenu();
                            }
                        }
                        break;
                    case State.Context:
                        DrawContextMenu();
                        break;
                    case State.Messages:
                        DrawMessagesMenu();
                        break;
                }

            });
        }

        public void DrawContextMenu() {
            if (character != null) {
                scrollPos = DGUI.ScrollWindow(() => {

                    DGUI.Horizontal(() => {
                        var third = Screen.width / 3;
                        GUILayout.Space(third);

                        DGUI.Vertical(() => {
                            
                            foreach (Context context in character.contexts) {
                                // --

                                GUIStyle style = GUI.skin.box;

                                style.wordWrap = true;
                                EditorStyles.label.wordWrap = true;

                                //--

                                GUI.Box(new Rect(third, 0, third, 100), "");

                                DGUI.Area(() => {
                                    GUILayout.Label(context.name);
                                }, third, 0, third, 100);

                                GUILayout.Space(100);
                            }

                            if (GUILayout.Button("Add Context", GUILayout.Height(DGUI.BUTTON_HEIGHT_BIG))) {
                                AddContext.character = character;
                                AddContext.Init();
                            }

                        });

                        GUILayout.Space(third);
                    });

                }, scrollPos, DGUI.BUTTON_HEIGHT_BIG);
            }

            else {
                EditorGUILayout.HelpBox("This characters doesn't exist anymore.", MessageType.Info);
            }
        }

        public void DrawMessagesMenu() {
            //
        }

        public void OnInspectorUpdate() {
            Repaint();
        }

        public void OnDisable() {
            if (state == State.Context || state == State.Messages) {
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
            }
        }
    }

}
