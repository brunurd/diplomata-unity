using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class CharacterEdit : EditorWindow {

        private const byte MARGIN = 7;
        private const byte BUTTON_HEIGHT = 30;
        private static readonly Vector2 EDIT_WIN_SIZE = new Vector2(400, 330);
        private static readonly Vector2 CREATE_WIN_SIZE = new Vector2(400, 100);

        public static Character character;
        private string characterName = "";
        
        public static void Init() {
            CharacterEdit window = (CharacterEdit)GetWindow(typeof(CharacterEdit), false, "Character Edit", true);

            if (character == null) {
                window.maxSize = CREATE_WIN_SIZE;
            }

            else {
                window.minSize = EDIT_WIN_SIZE;
            }

            window.Show();
        }

        public void OnGUI() {
            GUILayout.BeginHorizontal();
            GUILayout.Space(MARGIN);

            GUILayout.BeginVertical();
            GUILayout.Space(MARGIN);

            if (character == null) {
                DrawCreateWindow();
            }

            else {
                DrawEditWindow();
            }

            GUILayout.Space(MARGIN);
            GUILayout.EndVertical();

            GUILayout.Space(MARGIN);
            GUILayout.EndHorizontal();
        }

        public void DrawCreateWindow() {
            GUILayout.Label("Name: ");
            characterName = GUILayout.TextField(characterName);

            GUILayout.Space(MARGIN);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create", GUILayout.Height(BUTTON_HEIGHT))) {
                Diplomata.characters.Add(new Character(characterName));
                CharacterInspector.characterList = Diplomata.ListToArray(Diplomata.preferences.characterList);
                Close();
            }

            if (GUILayout.Button("Cancel", GUILayout.Height(BUTTON_HEIGHT))) {
                Close();
            }

            GUILayout.EndHorizontal();
        }

        public void DrawEditWindow() {
            GUILayout.Space(MARGIN);
            GUILayout.Label("Name: ");
            character.name = GUILayout.TextField(character.name);

            GUILayout.Space(MARGIN);
            GUILayout.Label("Description: ");
            character.description = GUILayout.TextArea(character.description, GUILayout.Height(50));

            GUILayout.Space(MARGIN);
            character.startOnPlay = GUILayout.Toggle(character.startOnPlay, "Start on play");

            GUILayout.Space(MARGIN);
            GUILayout.Label("Character attributes (influenceable by): ");

            for (int i = 0; i < character.attributes.Count; i++) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(character.attributes[i].key);
                character.attributes[i].value = (byte) EditorGUILayout.Slider(character.attributes[i].value, 0, 100);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(MARGIN);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save", GUILayout.Height(BUTTON_HEIGHT))) {
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
            }

            if (GUILayout.Button("Close", GUILayout.Height(BUTTON_HEIGHT))) {
                if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to close? All changed data will be lost.", "Yes", "No")) {
                    Close();
                }
            }

            GUILayout.EndHorizontal();
        }
    }

}
