using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class CharacterListMenu : EditorWindow {

        public Vector2 scrollPos = new Vector2(0, 0);

        [MenuItem("Diplomata/Character List")]
        static public void Init() {
            Diplomata.Instantiate();

            CharacterListMenu window = (CharacterListMenu)GetWindow(typeof(CharacterListMenu), false, "Character List");
            window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH + 80, 300);

            window.Show();
        }

        public void OnGUI() {
            scrollPos = DGUI.ScrollWindow(() => {

                if (Diplomata.preferences.characterList.Length <= 0) {
                    EditorGUILayout.HelpBox("No characters yet.", MessageType.Info);
                }

                for (int i = 0; i < Diplomata.preferences.characterList.Length; i++) {
                    var half = Screen.width - (2 * DGUI.MARGIN) - 6;
                    var name = Diplomata.preferences.characterList[i];

                    if (DGUI.hasSlider) {
                        half -= 15;
                    }

                    half /= 2;

                    DGUI.Horizontal(() => {

                        DGUI.Horizontal(() => {

                            DGUI.labelStyle.alignment = TextAnchor.MiddleLeft;
                            GUILayout.Label(name, DGUI.labelStyle);

                            DGUI.labelStyle.alignment = TextAnchor.MiddleRight;
                            DGUI.labelStyle.richText = true;
                            if (Diplomata.preferences.playerCharacterName == name) {
                                GUILayout.Label("<b>[Player]</b>", DGUI.labelStyle);
                            }

                        }, half);

                        DGUI.Horizontal(() => {

                            if (GUILayout.Button("Edit", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                                CharacterEditor.Edit(Character.Find(name));
                            }

                            if (GUILayout.Button("Edit Messages", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                                CharacterMessagesManager.OpenContextMenu(Character.Find(name));
                                Close();
                            }

                            if (GUILayout.Button("Delete", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                                if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No")) {
                                    var isPlayer = false;

                                    if (name == Diplomata.preferences.playerCharacterName) {
                                        isPlayer = true;
                                    }

                                    JSONHandler.Delete(name, "Diplomata/Characters/");

                                    Character.UpdateList();

                                    if (isPlayer && Diplomata.preferences.characterList.Length > 0) {
                                        Diplomata.preferences.playerCharacterName = Diplomata.preferences.characterList[0];
                                    }

                                    JSONHandler.Update(Diplomata.preferences, "preferences", "Diplomata/");

                                    CharacterEditor.Reset(name);
                                    CharacterMessagesManager.Reset(name);
                                    ContextEditor.Reset(name);
                                }
                            }

                        }, half);

                    });

                    if (i < Diplomata.preferences.characterList.Length - 1) {
                        DGUI.Separator();
                    }
                }

                EditorGUILayout.Separator();

                if (GUILayout.Button("Create", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    CharacterEditor.OpenCreate();
                }

            }, scrollPos, ((DGUI.BUTTON_HEIGHT_SMALL + 10) * Diplomata.preferences.characterList.Length) + DGUI.BUTTON_HEIGHT + 10 + (3 * DGUI.MARGIN));
        }

        public void OnInspectorUpdate() {
            Repaint();
        }
    }

}
