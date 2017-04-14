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

            CharacterEditor window = (CharacterEditor)GetWindow(typeof(CharacterEditor), false, "Character Edit", true);

            if (state == State.Create) {
                window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 100);
                window.maxSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 101);
            }

            else {
                window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 305);
            }

            if (state == State.Close) {
                window.Close();
            }

            else {
                window.Show();
            }
        }

        public static void Create() {
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
                            character = Diplomata.FindCharacter(Diplomata.preferences.workingCharacter);
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
                    if (characterName != "") {
                        Diplomata.characters.Add(new Character(characterName));
                        CharacterInspector.characterList = ArrayHandler.ListToArray(Diplomata.preferences.characterList);
                    }
                    Close();
                }

                if (GUILayout.Button("Cancel", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    Close();
                }
            });
        }

        public void DrawEditWindow() {
            GUILayout.Label("Name: ");
            character.name = EditorGUILayout.TextField(character.name);

            EditorGUILayout.Separator();

            GUILayout.Label("Description: ");
            character.description = EditorGUILayout.TextField(character.description);

            EditorGUILayout.Separator();

            character.startOnPlay = GUILayout.Toggle(character.startOnPlay, "Start on play");

            EditorGUILayout.Separator();

            GUILayout.Label("Character attributes (influenceable by): ");

            for (int i = 0; i < character.attributes.Length; i++) {
                    character.attributes[i].value = (byte)EditorGUILayout.Slider(character.attributes[i].key, character.attributes[i].value, 0, 100);
            }

            EditorGUILayout.Separator();

            DGUI.Horizontal(() => {

                if (GUILayout.Button("Save", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                }

                if (GUILayout.Button("Close", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                    Close();
                }

            });
        }

        public void OnDisable() {
            if (state == State.Edit && character != null) {
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
            }
        }
    }

}
