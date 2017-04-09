using UnityEngine;
using UnityEditor;

namespace DiplomataEditor {

    [CustomEditor(typeof(Character))]
    [CanEditMultipleObjects]
    public class Character : Editor {

        private const byte MARGIN = 15;

        public static Character character;
        SerializedProperty attributes;
        SerializedProperty description;
        SerializedProperty startOnPlay;

        public void OnEnable() {
            attributes = serializedObject.FindProperty("attributes");
            description = serializedObject.FindProperty("description");
            startOnPlay = serializedObject.FindProperty("startOnPlay");
            character = target as Character;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            GUILayout.Space(MARGIN);
            GUILayout.Label("Description: ");
            description.stringValue = GUILayout.TextArea(description.stringValue, GUILayout.Height(50));

            GUILayout.Space(MARGIN);
            startOnPlay.boolValue = GUILayout.Toggle(startOnPlay.boolValue, "Start on play");

            GUILayout.Space(MARGIN);
            GUILayout.Label("Character attributes (influenceable by): ");
            for (int i = 0; i < attributes.arraySize; i++) {
                SerializedProperty key = attributes.GetArrayElementAtIndex(i).FindPropertyRelative("key");
                SerializedProperty value = attributes.GetArrayElementAtIndex(i).FindPropertyRelative("value");
                GUILayout.BeginHorizontal();
                GUILayout.Label(key.stringValue);
                value.intValue = (byte)EditorGUILayout.Slider(value.intValue, 0, 100);
                GUILayout.EndHorizontal();
            }

            if (character != null) {
                GUILayout.Space(MARGIN);

                if (GUILayout.Button("Edit messages", GUILayout.Height(40))) {

                    if (Manager.instance != null && Manager.instance.characters.IndexOf(character) != -1) {
                        Manager.instance.currentCharacterIndex = Manager.instance.characters.IndexOf(character);
                    }

                    else {
                        character.InstantiateManager();
                        Manager.instance.characters.Add(character);
                        Manager.instance.currentCharacterIndex = Manager.instance.characters.IndexOf(character);
                    }

                    MessageManager.Init();
                }
            }

            GUILayout.Space(MARGIN);

            serializedObject.ApplyModifiedProperties();
        }
    }

}