using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class CustomFlagListMenu : EditorWindow {

        public Vector2 scrollPos = new Vector2(0, 0);
        private Diplomata diplomataEditor;
        private string[] booleanArray = new string[] { "True", "False" };

        [MenuItem("Diplomata/Custom Flags")]
        static public void Init() {
            Diplomata.Instantiate();

            CustomFlagListMenu window = (CustomFlagListMenu)GetWindow(typeof(CustomFlagListMenu), false, "Custom Flags");
            window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH + 150, 300);

            window.Show();
        }

        public void OnEnable() {
            diplomataEditor = (Diplomata)AssetHandler.Read("Diplomata.asset", "Diplomata/");
        }

        public void OnGUI() {
            DGUI.Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginVertical(DGUI.windowStyle);

            if (diplomataEditor.customFlags.flags.Length <= 0) {
                EditorGUILayout.HelpBox("No flags yet.", MessageType.Info);
            }

            var width = Screen.width - (2 * DGUI.MARGIN);

            for (int i = 0; i < diplomataEditor.customFlags.flags.Length; i++) {

                var flag = diplomataEditor.customFlags.flags[i];

                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();

                DGUI.labelStyle.normal.textColor = DGUI.grey;
                DGUI.labelStyle.alignment = TextAnchor.MiddleRight;
                GUILayout.Label("flag : " + i, DGUI.labelStyle);
                DGUI.labelStyle.normal.textColor = Color.black;

                GUILayout.EndHorizontal();

                GUILayout.Space(5.0f);

                GUILayout.BeginHorizontal();

                DGUI.textContent.text = flag.name;

                DGUI.textAreaStyle.padding = DGUI.padding;

                var height = DGUI.textAreaStyle.CalcHeight(DGUI.textContent, width);
                
                flag.name = EditorGUILayout.TextArea(flag.name, DGUI.textAreaStyle, GUILayout.ExpandWidth(true), GUILayout.Height(height));

                GUILayout.EndHorizontal();

                GUILayout.Space(5.0f);

                GUILayout.BeginHorizontal();

                string selected = flag.value.ToString();

                EditorGUI.BeginChangeCheck();

                selected = DGUI.Popup("Start in ", selected, booleanArray);

                if (EditorGUI.EndChangeCheck()) {

                    if (selected == "True") {
                        flag.value = true;
                    }

                    else {
                        flag.value = false;
                    }

                    diplomataEditor.SaveCustomFlags();
                }

                GUILayout.Space(10.0f);
                
                GUILayout.Label("Move: ", DGUI.labelStyle);

                if (GUILayout.Button("Up", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                    if (i > 0) {
                        diplomataEditor.customFlags.flags = ArrayHandler.Swap(diplomataEditor.customFlags.flags, i, i - 1);
                        diplomataEditor.SaveCustomFlags();
                    }
                }

                if (GUILayout.Button("Down", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                    if (i < diplomataEditor.customFlags.flags.Length - 1) {
                        diplomataEditor.customFlags.flags = ArrayHandler.Swap(diplomataEditor.customFlags.flags, i, i + 1);
                        diplomataEditor.SaveCustomFlags();
                    }
                }

                GUILayout.Space(5.0f);

                if (GUILayout.Button("Add Next", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {

                    diplomataEditor.customFlags.flags = ArrayHandler.Add(diplomataEditor.customFlags.flags, new Flag("", false));

                    for (int j = 1; j < (diplomataEditor.customFlags.flags.Length - 1) - i; j++) {
                        diplomataEditor.customFlags.flags = ArrayHandler.Swap(diplomataEditor.customFlags.flags, diplomataEditor.customFlags.flags.Length - 1, i + j);
                    }

                    diplomataEditor.SaveCustomFlags();
                }

                if (GUILayout.Button("Delete", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                    if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No")) {

                        diplomataEditor.customFlags.flags = ArrayHandler.Remove(diplomataEditor.customFlags.flags, flag);
                        diplomataEditor.SaveCustomFlags();
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

                if (i < diplomataEditor.customFlags.flags.Length - 1) {
                    DGUI.Separator();
                }
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Create", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                diplomataEditor.customFlags.flags = ArrayHandler.Add(diplomataEditor.customFlags.flags, new Flag("", false));
                diplomataEditor.SaveCustomFlags();
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            DGUI.labelStyle.alignment = TextAnchor.MiddleLeft;
        }

        public void OnDisable() {
            diplomataEditor.SaveCustomFlags();
        }
    }

}
