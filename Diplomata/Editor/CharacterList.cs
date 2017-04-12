using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class CharacterList : EditorWindow {

        private const byte BUTTON_HEIGHT = 30;

        [MenuItem("Diplomata/Character List")]
        static public void Init() {
            Diplomata.Instantiate();

            CharacterList window = (CharacterList)GetWindow(typeof(CharacterList), false, "Character List");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        public void OnGUI() {
            if (Diplomata.preferences.characterList.Count <= 0) {
                EditorGUILayout.HelpBox("No characters yet.", MessageType.Info);
            }

            foreach (string name in Diplomata.preferences.characterList) {
                GUILayout.BeginHorizontal();

                GUILayout.Label(name, GUILayout.Width(Screen.width / 3));

                if (GUILayout.Button("Edit", GUILayout.Height(BUTTON_HEIGHT / 1.5f))) {
                    CharacterEdit.character = FindCharacter(name);
                    CharacterEdit.Init();
                }

                if (GUILayout.Button("Delete", GUILayout.Height(BUTTON_HEIGHT / 1.5f))) {
                    if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete? This data will be lost forever.", "Yes", "No")) {
                        JSONHandler.Delete(name, "Diplomata/Characters/");
                        Character.UpdateList();
                    }
                }

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Create", GUILayout.Height(BUTTON_HEIGHT))) {
                CharacterEdit.character = null;
                CharacterEdit.Init();
            }
        }

        public Character FindCharacter(string name) {
            foreach (Character character in Diplomata.characters) {
                if (character.name == name) {
                    return character;
                }
            }

            return null;
        }

        public void OnInspectorUpdate() {
            Repaint();
        }


    }

}
