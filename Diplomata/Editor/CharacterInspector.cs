using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    [CustomEditor(typeof(DiplomataCharacter))]
    [CanEditMultipleObjects]
    public class CharacterInspector : Editor {
        
        public DiplomataCharacter diplomataCharacter;
        private static Diplomata diplomataEditor;
        
        public void OnEnable() {
            Refresh();
        }

        public void Refresh() {
            Diplomata.Instantiate();
            diplomataEditor = (Diplomata)AssetHandler.Read("Diplomata.asset", "Diplomata/");
            diplomataCharacter = target as DiplomataCharacter;
        }

        public override void OnInspectorGUI() {
            DGUI.Init();

            serializedObject.Update();
            
            GUILayout.BeginVertical(DGUI.windowStyle);

            if (diplomataCharacter.character != null && diplomataEditor.characters.Count > 0) {
                    
                GUILayout.BeginHorizontal();

                GUILayout.Label("Character: ");

                if (!Application.isPlaying) {
                    var selected = 0;

                    for (var i = 0; i < diplomataEditor.characters.Count; i++) {
                        if (diplomataEditor.characters[i].name == diplomataCharacter.character.name) {
                            selected = i;
                            break;
                        }
                    }

                    var selectedBefore = selected;

                    selected = EditorGUILayout.Popup(selected, diplomataEditor.preferences.characterList);

                    for (var i = 0; i < diplomataEditor.characters.Count; i++) {
                        if (selected == i) {
                            diplomataCharacter.character = diplomataEditor.characters[i];
                            diplomataEditor.characters[selectedBefore].onScene = false;
                            diplomataCharacter.character.onScene = true;
                            break;
                        }
                    }
                }

                else {
                    GUILayout.Label(diplomataCharacter.character.name);
                }

                GUILayout.EndHorizontal();

                if (GUILayout.Button("Refresh", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                    Refresh();
                }

                DGUI.Separator();

                var showInfluence = true;

                if (diplomataCharacter.character.name == diplomataEditor.preferences.playerCharacterName) {
                    EditorGUILayout.HelpBox("\nThis character is the player, he doesn't influence himself, use his messages only in the case he speaks with himself.\n", MessageType.Info);
                    showInfluence = false;
                }
                    
                if (GUILayout.Button("Edit Character", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    CharacterEditor.Edit(diplomataCharacter.character);
                }

                if (GUILayout.Button("Edit Messages", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    CharacterMessagesManager.OpenContextMenu(diplomataCharacter.character);
                }

                if (showInfluence) {
                    DGUI.labelStyle.alignment = TextAnchor.UpperCenter;
                    EditorGUILayout.Separator();
                    GUILayout.Label("Influence: <b>" + diplomataCharacter.character.influence.ToString() + "</b>", DGUI.labelStyle);
                }

                DGUI.Separator();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Character", GUILayout.Height(DGUI.BUTTON_HEIGHT_BIG))) {
                    CharacterEditor.OpenCreate();
                }

                EditorGUILayout.HelpBox("Create does not interfe in this character.", MessageType.Info);
                GUILayout.EndHorizontal();
            }

            else {
                if (GUILayout.Button("Create Character", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    CharacterEditor.OpenCreate();
                }
            }

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

}