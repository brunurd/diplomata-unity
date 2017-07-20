using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class ItemListMenu : EditorWindow {

        public Vector2 scrollPos = new Vector2(0, 0);
        private Diplomata diplomataEditor;

        [MenuItem("Diplomata/Inventory")]
        static public void Init() {
            Diplomata.Instantiate();

            ItemListMenu window = (ItemListMenu)GetWindow(typeof(ItemListMenu), false, "Inventory");
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
            
            if (diplomataEditor.inventory.items.Length <= 0) {
                EditorGUILayout.HelpBox("No items yet.", MessageType.Info);
            }

            for (int i = 0; i < diplomataEditor.inventory.items.Length; i++) {

                var item = diplomataEditor.inventory.items[i];

                GUILayout.BeginHorizontal();
                GUILayout.BeginHorizontal();

                var name = DictHandler.ContainsKey(item.name, diplomataEditor.preferences.currentLanguage);
                
                DGUI.labelStyle.alignment = TextAnchor.MiddleLeft;
                GUILayout.Label(name.value, DGUI.labelStyle);

                DGUI.labelStyle.alignment = TextAnchor.MiddleRight;
                DGUI.labelStyle.normal.textColor = DGUI.grey;
                GUILayout.Label("id: " + item.id, DGUI.labelStyle);

                DGUI.labelStyle.normal.textColor = Color.black;

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 2));

                if (GUILayout.Button("Edit", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                    ItemEditor.OpenEdit(item);
                }

                if (GUILayout.Button("Delete", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                    if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No")) {

                        if (item.id == diplomataEditor.GetWorkingItemId()) {
                            ItemEditor.Init(ItemEditor.State.Close);
                            diplomataEditor.SetWorkingItemId(-1);
                        }

                        diplomataEditor.inventory.items = ArrayHandler.Remove(diplomataEditor.inventory.items, item);
                        diplomataEditor.SaveInventory();
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();

                if (i < diplomataEditor.inventory.items.Length - 1) {
                    DGUI.Separator();
                }
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Create", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                diplomataEditor.inventory.items = ArrayHandler.Add(diplomataEditor.inventory.items, new Item(diplomataEditor.inventory.items.Length));
                diplomataEditor.SaveInventory();
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public void OnInspectorUpdate() {
            Repaint();
        }
    }

}