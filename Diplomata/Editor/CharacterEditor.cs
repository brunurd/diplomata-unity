using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class CharacterEditor : EditorWindow {
        
        public static Character character;
        private string characterName = "";

        public enum State {
            None,
            Create,
            Edit,
            Close
        }

        private static State state;
        
        public static void Init(State state = State.None) {
            CharacterEditor.state = state;
            DGUI.focusOnStart = true;

            CharacterEditor window = (CharacterEditor)GetWindow(typeof(CharacterEditor), false, "Character", true);

            if (state == State.Create) {
                window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 100);
                window.maxSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 101);
            }

            else {
                window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 390);
            }

            if (state == State.Close) {
                window.Close();
            }

            else {
                window.Show();
            }
        }

        public static void OpenCreate() {
            character = null;
            Diplomata.preferences.SetWorkingCharacter(string.Empty);
            Init(State.Create);
        }

        public static void Edit(Character currentCharacter) {
            character = currentCharacter;
            Diplomata.preferences.SetWorkingCharacter(currentCharacter.name);
            Init(State.Edit);
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
                            character = Character.Find(Diplomata.preferences.workingCharacter);
                            DrawEditWindow();
                        }
                        else {
                            DrawCreateWindow();
                        }
                        break;

                    case State.Create:
                        DrawCreateWindow();
                        break;

                    case State.Edit:
                        DrawEditWindow();
                        break;
                }

            });
        }

        public void DrawCreateWindow() {
            GUILayout.Label("Name: ");

            DGUI.Focus(() => {
                characterName = EditorGUILayout.TextField(characterName);
            });

            EditorGUILayout.Separator();
                    
            DGUI.Horizontal(() => {
                if (GUILayout.Button("Create", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    Create();
                }

                if (GUILayout.Button("Cancel", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    Close();
                }
            });
            

            if (focusedWindow.ToString() == " (DiplomataEditor.CharacterEditor)") {
                if (Event.current.keyCode == KeyCode.Return) {
                    Create();
                }
            }
        }

        public void Create() {
            if (characterName != "") {
                Diplomata.characters.Add(new Character(characterName));
            }

            else {
                Debug.LogError("Character name was empty.");
            }
            
            Close();
        }

        public void DrawEditWindow() {
            GUILayout.Label("Name: ");
            character.name = EditorGUILayout.TextField(character.name);

            DGUI.Separator();
            
            var description = DictHandler.ContainsKey(character.description, Diplomata.preferences.currentLanguage);

            if (description == null) {
                character.description = ArrayHandler.Add(character.description, new DictLang(Diplomata.preferences.currentLanguage, ""));
                description = DictHandler.ContainsKey(character.description, Diplomata.preferences.currentLanguage);
            }
            
            DGUI.textContent.text = description.value;
            DGUI.textAreaStyle.padding = DGUI.boxPadding;
            var height = DGUI.textAreaStyle.CalcHeight(DGUI.textContent, Screen.width - (2 * DGUI.MARGIN));

            GUILayout.Label("Description: ");
            description.value = EditorGUILayout.TextArea(description.value, DGUI.textAreaStyle, GUILayout.Height(height + 15));
            
            EditorGUILayout.Separator();

            DGUI.Horizontal(() => {

                character.startOnPlay = GUILayout.Toggle(character.startOnPlay, " Start on play");

                var player = false;

                if (Diplomata.preferences.playerCharacterName == character.name) {
                    player = true;
                }

                player = GUILayout.Toggle(player, " Is player");

                if (player) {
                    Diplomata.preferences.playerCharacterName = character.name;
                    CharacterMessagesManager.UpdateCharacterList();
                }

            });
            
            if (character.name != Diplomata.preferences.playerCharacterName) {
                DGUI.Separator();

                GUILayout.Label("Character attributes (influenceable by): ");

                for (int i = 0; i < character.attributes.Length; i++) {
                    character.attributes[i].value = (byte)EditorGUILayout.Slider(character.attributes[i].key, character.attributes[i].value, 0, 100);
                }

                DGUI.Separator();
            }

            else {
                EditorGUILayout.Separator();
            }

            DGUI.Horizontal(() => {

                if (GUILayout.Button("Save", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    Save();
                    Close();
                }

                if (GUILayout.Button("Close", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    Save();
                    Close();
                }

            });
        }

        public void Save() {
            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
            JSONHandler.Update(Diplomata.preferences, "preferences", "Diplomata/");
        }

        public void OnDisable() {
            if (state == State.Edit && character != null) {
                Save();
            }
        }
    }

}
