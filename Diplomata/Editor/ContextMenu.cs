using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class ContextMenu {

        private static Vector2 scrollPos = new Vector2(0, 0);
        private static float lateContextMenuHeight;

        public static void Draw() {
            var character = CharacterMessagesManager.character;

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
                            style.alignment = TextAnchor.MiddleLeft;
                            
                            foreach (Context context in character.contexts) {

                                var buttonHeight = DGUI.BUTTON_HEIGHT + buttonAutoMargin + (2 * DGUI.MARGIN);
                                var height = DGUI.Box(context.name, third, yPos, third, buttonHeight);

                                yPos += height;

                                GUILayout.Space(height + DGUI.MARGIN);

                                DGUI.Horizontal(() => {

                                    if (GUILayout.Button("Edit", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.ExpandWidth(true))) {
                                        CharacterMessagesManager.OpenMessagesMenu(character, context);
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
                            
                            yPos += DGUI.BUTTON_HEIGHT_BIG + buttonAutoMargin + DGUI.MARGIN;
                        });

                        GUILayout.Space(third);
                    });

                }, scrollPos, lateContextMenuHeight);

                /*
                 * SAVE AS SCREENPLAY BUTTON 
                
                var saveAsScreenplayButtonWidth = 200;
                var saveAsScreenplayButtonXPos = Screen.width - saveAsScreenplayButtonWidth - DGUI.MARGIN;
                var saveAsScreenplayButtonYPos = Screen.height - (DGUI.BUTTON_HEIGHT_BIG * 1.5f) - DGUI.MARGIN;
                
                if (DGUI.hasSlider) {
                    saveAsScreenplayButtonXPos -= 15;
                    saveAsScreenplayButtonYPos = (Screen.height + scrollPos.x) - (DGUI.BUTTON_HEIGHT_BIG * 1.5f) - DGUI.MARGIN;
                }

                if (GUI.Button(new Rect(
                        saveAsScreenplayButtonXPos,
                        saveAsScreenplayButtonYPos,
                        saveAsScreenplayButtonWidth,
                        DGUI.BUTTON_HEIGHT_BIG
                ), "Save as Screenplay")) {
                    Screenplay.Save(character);
                }
                
                */
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
