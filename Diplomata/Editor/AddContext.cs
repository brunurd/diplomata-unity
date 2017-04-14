using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class AddContext : EditorWindow {

        public static Character character;
        private string contextName = "";

        public static void Init(bool show = true) {
            if (character != null) {
                AddContext window = (AddContext)GetWindow(typeof(AddContext), false, "New Context", true);
                window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 100);
                window.maxSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 101);

                if (show) {
                    window.Show();
                }

                else {
                    window.Close();
                }
            }
        }

        public static void Reset(string characterName) {
            if (character.name == characterName) {
                Init(false);
            }
        }

        public void OnGUI() {
            DGUI.WindowWrap(() => {

                GUILayout.Label("Name: ");

                DGUI.Focus(() => {
                    contextName = EditorGUILayout.TextField(contextName);
                });

                EditorGUILayout.Separator();

                DGUI.Horizontal(() => {
                    if (GUILayout.Button("Create", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        character.contexts = ArrayHandler.Add(character.contexts, new Context(contextName, character.name));
                        JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                        Close();
                    }

                    if (GUILayout.Button("Cancel", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        Close();
                    }
                });

            });
        }
    }

}