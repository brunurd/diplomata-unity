using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class ContextMenu {

        private static Vector2 scrollPos = new Vector2(0, 0);
        private static float lateContextMenuHeight;

        public static void DrawContextMenu(Character character) {
            var buttonAutoMargin = 3;
            var titleLabelHeight = 50;
            float yPos = DGUI.MARGIN + titleLabelHeight;
            var third = Screen.width / 3;

            if (character != null) {
                scrollPos = DGUI.ScrollWindow(() => {

                    DGUI.Horizontal(() => {
                        GUILayout.Space(third);

                        DGUI.Vertical(() => {

                            GUIStyle style = GUI.skin.label;

                            style.fontSize = 24;
                            style.alignment = TextAnchor.MiddleCenter;

                            GUILayout.Label(character.name, style, GUILayout.ExpandWidth(true), GUILayout.Height(titleLabelHeight));

                            style.fontSize = 11;

                            foreach (Context context in character.contexts) {

                                var buttonHeight = DGUI.BUTTON_HEIGHT + buttonAutoMargin + (2 * DGUI.MARGIN);
                                var height = DGUI.Box(context.name, third, yPos, third, buttonHeight);

                                yPos += height;

                                GUILayout.Space(height + DGUI.MARGIN);

                                DGUI.Horizontal(() => {

                                    if (GUILayout.Button("Edit", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.ExpandWidth(true))) {
                                        CharacterMessagesManager.context = context;
                                        CharacterMessagesManager.state = CharacterMessagesManager.State.Messages;
                                    }

                                    if (GUILayout.Button("Delete", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.ExpandWidth(true))) {
                                        if (EditorUtility.DisplayDialog("Are you sure?", "All data inside this context will be lost forever.", "Yes", "No")) {
                                            character.contexts = ArrayHandler.Remove(character.contexts, context);
                                            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                                        }
                                    }

                                });

                                GUILayout.Space(2 * DGUI.MARGIN);

                                yPos += buttonHeight + DGUI.MARGIN;
                            }

                            if (GUILayout.Button("Add Context", GUILayout.Height(DGUI.BUTTON_HEIGHT_BIG))) {
                                AddContext.character = character;
                                AddContext.Init();
                            }

                        });

                        GUILayout.Space(third);
                    });

                }, scrollPos, lateContextMenuHeight + DGUI.BUTTON_HEIGHT_BIG + buttonAutoMargin + DGUI.MARGIN);
            }

            else {
                DGUI.WindowWrap(() => {
                    EditorGUILayout.HelpBox("This characters doesn't exist anymore.", MessageType.Info);
                });
            }

            lateContextMenuHeight = yPos;
        }

    }

}
