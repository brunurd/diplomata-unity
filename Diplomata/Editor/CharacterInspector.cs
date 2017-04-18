using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    [CustomEditor(typeof(DiplomataCharacter))]
    [CanEditMultipleObjects]
    public class CharacterInspector : Editor {
        
        public DiplomataCharacter diplomataCharacter;
        
        public void OnEnable() {
            Refresh();
        }

        public void Refresh() {
            Diplomata.Instantiate();
            diplomataCharacter = target as DiplomataCharacter;
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

                        var selectedBefore = selected;

                        selected = EditorGUILayout.Popup(selected, Diplomata.preferences.characterList);

                        for (var i = 0; i < Diplomata.characters.Count; i++) {
                            if (selected == i) {
                                diplomataCharacter.character = Diplomata.characters[i];
                                Diplomata.characters[selectedBefore].onScene = false;
                                diplomataCharacter.character.onScene = true;
                                break;
                            }
                        }

                        if (GUILayout.Button("Refresh", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                            Refresh();
                        }
                    });

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Edit Character", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        CharacterEditor.Edit(diplomataCharacter.character);
                    }

                    if (GUILayout.Button("Edit Messages", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        CharacterMessagesManager.OpenContextMenu(diplomataCharacter.character);
                    }

                    EditorGUILayout.Separator();

                    GUILayout.Label("Influence: " + diplomataCharacter.character.influence);

                    EditorGUILayout.Separator();

                    DGUI.Horizontal(() => {
                        if (GUILayout.Button("Create Character", GUILayout.Height(DGUI.BUTTON_HEIGHT_BIG))) {
                            CharacterEditor.OpenCreate();
                        }

                        EditorGUILayout.HelpBox("Create does not interfe in this character.", MessageType.Info);
                    });
                }

                else {
                    if (GUILayout.Button("Create Character", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        CharacterEditor.OpenCreate();
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