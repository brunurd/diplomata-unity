using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    [CustomEditor(typeof(DiplomataCharacter))]
    [CanEditMultipleObjects]
    public class CharacterInspector : Editor {

        private const byte MARGIN = 15;
        private const byte BUTTON_HEIGHT = 30;
        private const byte BIG_BUTTON_HEIGHT = 63;

        public DiplomataCharacter diplomataCharacter;
        public static string[] characterList;
        
        public void OnEnable() {
            Diplomata.Instantiate();
            diplomataCharacter = target as DiplomataCharacter;
            characterList = Diplomata.ListToArray(Diplomata.preferences.characterList);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            GUILayout.Space(MARGIN);

            if (diplomataCharacter.character != null && Diplomata.characters.Count > 0) {

                GUILayout.BeginHorizontal();

                GUILayout.Label("Character: ");

                var selected = 0;

                for (var i = 0; i < Diplomata.characters.Count; i++) {
                    if (Diplomata.characters[i].name == diplomataCharacter.character.name) {
                        selected = i;
                        break;
                    }
                }

                selected = EditorGUILayout.Popup(selected, characterList);

                for (var i = 0; i < Diplomata.characters.Count; i++) {
                    if (selected == i) {
                        diplomataCharacter.character = Diplomata.characters[i];
                        break;
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(MARGIN / 2);

                if (GUILayout.Button("Edit Character", GUILayout.Height(BUTTON_HEIGHT))) {
                    CharacterEdit.character = diplomataCharacter.character;
                    CharacterEdit.Init();
                }

                if (GUILayout.Button("Edit Messages", GUILayout.Height(BUTTON_HEIGHT))) {
                    //
                }

                EditorGUILayout.Separator();

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Create Character", GUILayout.Height(40))) {
                    CharacterEdit.character = null;
                    CharacterEdit.Init();
                }

                EditorGUILayout.HelpBox("Create does not interfe in this character.", MessageType.Info);

                GUILayout.EndHorizontal();
            }

            else {
                if (GUILayout.Button("Create Character", GUILayout.Height(BUTTON_HEIGHT))) {
                    CharacterEdit.character = null;
                    CharacterEdit.Init();
                }
            }

            GUILayout.Space(MARGIN);

            serializedObject.ApplyModifiedProperties();
        }
    }

}