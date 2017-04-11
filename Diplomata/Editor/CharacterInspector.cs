using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    [CustomEditor(typeof(DiplomataCharacter))]
    [CanEditMultipleObjects]
    public class CharacterInspector : Editor {

        private const byte MARGIN = 15;

        public DiplomataCharacter diplomataCharacter;
        public static string[] characterList;
        private int selected = 0;

        public void OnEnable() {
            Diplomata.Instantiate();
            diplomataCharacter = target as DiplomataCharacter;
            characterList = Diplomata.ListToArray(Diplomata.preferences.characterList);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            GUILayout.Space(MARGIN);
            selected = EditorGUILayout.Popup("Character", selected, characterList);
            
            if (Diplomata.characters.Count > 0 && selected < Diplomata.characters.Count) {
                diplomataCharacter.character = Diplomata.characters[selected];
            }

            else {
                diplomataCharacter.character = null;
            }

            GUILayout.Space(MARGIN / 2);

            if (diplomataCharacter.character != null) {
                if (GUILayout.Button("Edit Character", GUILayout.Height(30))) {
                    CharacterEdit.character = diplomataCharacter.character;
                    CharacterEdit.Init();
                }
            }

            if (GUILayout.Button("Create Character", GUILayout.Height(30))) {
                CharacterEdit.character = null;
                CharacterEdit.Init();
            }

            GUILayout.Space(MARGIN);

            serializedObject.ApplyModifiedProperties();
        }
    }

}