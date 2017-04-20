using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class CharacterMessagesManager : EditorWindow {

        public static Character character;
        public static Context context;
        private ushort timer = 0;

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
            window.minSize = new Vector2(960, 300);
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
            Diplomata.preferences.SetWorkingContextMessagesId(-1);
            Init(State.Context);
        }

        public static void OpenMessagesMenu(Character currentCharacter, Context currentContext) {
            character = currentCharacter;
            context = currentContext;
            Diplomata.preferences.SetWorkingCharacter(currentCharacter.name);
            Diplomata.preferences.SetWorkingContextMessagesId(currentContext.id);
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
                        character = Character.Find(Diplomata.preferences.workingCharacter);

                        if (Diplomata.preferences.workingContextMessagesId > -1) {
                            context = Context.Find(character, Diplomata.preferences.workingContextMessagesId);
                            MessagesEditor.Draw();
                        }

                        else {
                            ContextListMenu.Draw();
                        }
                    }
                    break;
                case State.Context:
                    ContextListMenu.Draw();
                    break;
                case State.Messages:
                    MessagesEditor.Draw();
                    break;
            }
        }

        public void OnInspectorUpdate() {
            AutoSave();
            Repaint();
        }

        private void AutoSave() {

            if (timer == 120) {
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                timer = 0;
            }

            timer++;

        }

        public void OnDisable() {
            if (character != null) {
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
            }
        }
    }

}
