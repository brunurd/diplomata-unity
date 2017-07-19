using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class ContextListMenu {

        private static Vector2 scrollPos = new Vector2(0, 0);

        public static void Draw() {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;
            var character = CharacterMessagesManager.character;
            var listWidth = Screen.width / 3;

            if (character != null) {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                GUILayout.BeginHorizontal();
                GUILayout.Space(listWidth);

                GUILayout.BeginVertical(DGUI.windowStyle, GUILayout.Width(listWidth));
                
                DGUI.labelStyle.fontSize = 24;
                DGUI.labelStyle.alignment = TextAnchor.MiddleCenter;

                GUILayout.Label(character.name, DGUI.labelStyle, GUILayout.Height(50));
                
                for (int i = 0; i < character.contexts.Length; i++) {

                    Context context = Context.Find(character, i);
                    
                    Rect boxRect = EditorGUILayout.BeginVertical(DGUI.boxStyle);
                    GUI.Box(boxRect, GUIContent.none);

                    var name = DictHandler.ContainsKey(context.name, diplomataEditor.preferences.currentLanguage);

                    if (name == null) {
                        context.name = ArrayHandler.Add(context.name, new DictLang(diplomataEditor.preferences.currentLanguage, "Name [Change clicking on Edit]"));
                        name = DictHandler.ContainsKey(context.name, diplomataEditor.preferences.currentLanguage);
                    }

                    var description = DictHandler.ContainsKey(context.description, diplomataEditor.preferences.currentLanguage);

                    if (description == null) {
                        context.description = ArrayHandler.Add(context.description, new DictLang(diplomataEditor.preferences.currentLanguage, "Description [Change clicking on Edit]"));
                        description = DictHandler.ContainsKey(context.description, diplomataEditor.preferences.currentLanguage);
                    }

                    DGUI.labelStyle.fontSize = 11;
                    DGUI.labelStyle.alignment = TextAnchor.UpperCenter;

                    DGUI.textContent.text = "<size=13><i>" + name.value + "</i></size>\n\n" + description.value + "\n";
                    var height = DGUI.labelStyle.CalcHeight(DGUI.textContent, listWidth);
                    
                    GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(listWidth), GUILayout.Height(height));

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Edit", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        ContextEditor.Edit(character, context);
                    }

                    if (GUILayout.Button("Edit Messages", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        CharacterMessagesManager.OpenMessagesMenu(character, context);
                    }

                    if (GUILayout.Button("Delete", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        if (EditorUtility.DisplayDialog("Are you sure?", "All data inside this context will be lost forever.", "Yes", "No")) {
                            ContextEditor.Reset(character.name);
                            character.contexts = ArrayHandler.Remove(character.contexts, context);
                            character.contexts = Context.ResetIDs(character.contexts);
                            diplomataEditor.Save(character);
                        }
                    }

                    if (GUILayout.Button("Move Up", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        if (context.id > 0) {

                            Context.Find(character, context.id - 1).id += 1;
                            context.id -= 1;
                            
                            diplomataEditor.Save(character);
                        }
                    }

                    if (GUILayout.Button("Move Down", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        if (context.id < character.contexts.Length - 1) {

                            Context.Find(character, context.id + 1).id -= 1;
                            context.id += 1;
                            
                            diplomataEditor.Save(character);
                        }
                    }

                    GUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Separator();
                }

                GUILayout.BeginHorizontal();
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
                GUILayout.EndHorizontal();
                
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }

            else {
                GUILayout.BeginVertical(DGUI.windowStyle);
                EditorGUILayout.HelpBox("This characters doesn't exist anymore.", MessageType.Info);
                GUILayout.EndVertical();
            }
        }

        public static void CreateContext() {
            var character = CharacterMessagesManager.character;

            character.contexts = ArrayHandler.Add(character.contexts, new Context(character.contexts.Length, character.name));
            CharacterMessagesManager.diplomataEditor.Save(character);
        }

    }

}
