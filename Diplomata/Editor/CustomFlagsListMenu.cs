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
            window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH + 80, 300);

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

            for (int i = 0; i < diplomataEditor.customFlags.flags.Length; i++) {

                var flag = diplomataEditor.customFlags.flags[i];

                GUILayout.BeginHorizontal();

                GUILayout.BeginHorizontal();

                flag.name = EditorGUILayout.TextField(flag.name, GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL));

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 3));

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

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 3));


                if (GUILayout.Button("Delete", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                    if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No")) {

                        diplomataEditor.customFlags.flags = ArrayHandler.Remove(diplomataEditor.customFlags.flags, flag);
                        diplomataEditor.SaveCustomFlags();
                    }
                }

                GUILayout.EndHorizontal();
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
        }

        public void OnDisable() {
            diplomataEditor.SaveCustomFlags();
        }
    }

}
