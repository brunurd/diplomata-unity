using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class ContextListMenu {

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
                            
                            DGUI.labelStyle.fontSize = 24;
                            DGUI.labelStyle.alignment = TextAnchor.MiddleCenter;

                            GUILayout.Label(character.name, DGUI.labelStyle, GUILayout.ExpandWidth(true), GUILayout.Height(titleLabelHeight));

                            DGUI.labelStyle.fontSize = 11;
                            DGUI.labelStyle.alignment = TextAnchor.MiddleLeft;
                            
                            foreach (Context context in character.contexts) {

                                var buttonHeight = DGUI.BUTTON_HEIGHT + buttonAutoMargin + DGUI.MARGIN;

                                var name = DictHandler.ContainsKey(context.name, Diplomata.preferences.currentLanguage);

                                if (name == null) {
                                    context.name = ArrayHandler.Add(context.name, new DictLang(Diplomata.preferences.currentLanguage, "Name [Change clicking on Edit]"));
                                    name = DictHandler.ContainsKey(context.name, Diplomata.preferences.currentLanguage);
                                }

                                var description = DictHandler.ContainsKey(context.description, Diplomata.preferences.currentLanguage);

                                if (description == null) {
                                    context.description = ArrayHandler.Add(context.description, new DictLang(Diplomata.preferences.currentLanguage, "Description [Change clicking on Edit]"));
                                    description = DictHandler.ContainsKey(context.description, Diplomata.preferences.currentLanguage);
                                }
                                
                                var content = "<size=13><i>" + name.value + "</i></size>\n\n" + description.value;
                                var height = DGUI.Box(content, third, yPos, third, buttonHeight);

                                yPos += height;

                                GUILayout.Space(height);

                                DGUI.Horizontal(() => {
                                    if (GUILayout.Button("Edit", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.ExpandWidth(true))) {
                                        ContextEditor.Edit(character, context);
                                    }

                                    if (GUILayout.Button("Edit Messages", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.ExpandWidth(true))) {
                                        CharacterMessagesManager.OpenMessagesMenu(character, context);
                                    }

                                    if (GUILayout.Button("Delete", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.ExpandWidth(true))) {
                                        if (EditorUtility.DisplayDialog("Are you sure?", "All data inside this context will be lost forever.", "Yes", "No")) {
                                            ContextEditor.Reset(character.name);
                                            character.contexts = ArrayHandler.Remove(character.contexts, context);
                                            Context.ResetIDs(character.contexts);
                                            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                                        }
                                    }

                                });

                                GUILayout.Space(2 * DGUI.MARGIN);

                                yPos += buttonHeight + DGUI.MARGIN;
                            }

                            DGUI.Horizontal(() => {
                                if (GUILayout.Button("Add Context", GUILayout.Height(DGUI.BUTTON_HEIGHT_BIG))) {
                                    CreateContext();
                                }

                                if (GUILayout.Button("Save as Screenplay", GUILayout.Height(DGUI.BUTTON_HEIGHT_BIG))) {
                                    if (EditorUtility.DisplayDialog("This character only?", "This character only or all characters?", "Only this character", "All characters")) {
                                        Screenplay.Save(character);
                                    }
                                    else {
                                        Screenplay.SaveAll();
                                    }
                                }
                            });
                            
                            yPos += DGUI.BUTTON_HEIGHT_BIG + buttonAutoMargin + DGUI.MARGIN;
                        });

                        GUILayout.Space(third);
                    });

                }, scrollPos, lateContextMenuHeight);
            }

            else {
                DGUI.WindowWrap(() => {
                    EditorGUILayout.HelpBox("This characters doesn't exist anymore.", MessageType.Info);
                });
            }

            lateContextMenuHeight = yPos;
        }

        public static void CreateContext() {
            var character = CharacterMessagesManager.character;

            character.contexts = ArrayHandler.Add(character.contexts, new Context(character.contexts.Length, character.name));
            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
        }

    }

}
