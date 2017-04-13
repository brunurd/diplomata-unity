using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    [CustomEditor(typeof(DiplomataCharacter))]
    [CanEditMultipleObjects]
    public class CharacterInspector : Editor {
        
        public DiplomataCharacter diplomataCharacter;
        public static string[] characterList;
        
        public void OnEnable() {
            Diplomata.Instantiate();
            diplomataCharacter = target as DiplomataCharacter;
            characterList = Diplomata.ListToArray(Diplomata.preferences.characterList);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            DGUI.WindowWrap(() => {

                if (diplomataCharacter.character != null && Diplomata.characters.Count > 0) {

                    DGUI.Horizontal(() => {

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

                    });

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Edit Character", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        CharacterEdit.Edit(diplomataCharacter.character);
                    }

                    if (GUILayout.Button("Edit Messages", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        MessagesManager.ContextMenu(diplomataCharacter.character);
                    }

                    EditorGUILayout.Separator();

                    DGUI.Horizontal(() => {
                        if (GUILayout.Button("Create Character", GUILayout.Height(DGUI.BUTTON_HEIGHT_BIG))) {
                            CharacterEdit.Create();
                        }

                        EditorGUILayout.HelpBox("Create does not interfe in this character.", MessageType.Info);
                    });
                }

                else {
                    if (GUILayout.Button("Create Character", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        CharacterEdit.Create();
                    }
                }

            });

            serializedObject.ApplyModifiedProperties();
        }

        public void OnInspectorUpdate() {
            Repaint();
        }
    }

}