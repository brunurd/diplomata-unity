using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class CharacterMessagesManager : EditorWindow {

        public static Character character;
        public static Context context;
        
        public enum State {
            None,
            Context,
            Messages,
            Close
        }

        public static State state;
        
        public static void Init(State state = State.None) {
            CharacterMessagesManager.state = state;
            CharacterMessagesManager window = (CharacterMessagesManager)GetWindow(typeof(CharacterMessagesManager), false, "Messages", true);
            window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 300);
            window.maximized = true;

            if (state == State.Close) {
                window.Close();
            }

            else {
                window.Show();
            }
        }

        public static void OpenContextMenu(Character currentCharacter) {
            character = currentCharacter;
            Diplomata.preferences.SetWorkingCharacter(currentCharacter.name);
            Diplomata.preferences.workingContext = string.Empty;
            Init(State.Context);
        }

        public static void OpenMessagesMenu(Context currentContext) {
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
            switch (state) {
                case State.None:
                    if (Diplomata.preferences.workingCharacter != string.Empty) {
                        character = Diplomata.FindCharacter(Diplomata.preferences.workingCharacter);

                        if (Diplomata.preferences.workingContext != string.Empty) {
                            DrawMessagesMenu();
                        }

                        else {
                            ContextMenu.DrawContextMenu(character);
                        }
                    }
                    break;
                case State.Context:
                    ContextMenu.DrawContextMenu(character);
                    break;
                case State.Messages:
                    DrawMessagesMenu();
                    break;
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
