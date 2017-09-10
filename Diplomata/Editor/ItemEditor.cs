using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class ItemEditor : EditorWindow {

        public static Item item;
        private Vector2 scrollPos = new Vector2(0, 0);
        private static Diplomata diplomataEditor;
        private static State state;

        public enum State {
            None,
            Edit,
            Close
        } 

        public static void Init(State state = State.None) {
            ItemEditor window = (ItemEditor)GetWindow(typeof(ItemEditor), false, "Item", true);
            window.minSize = new Vector2(DGUI.WINDOW_MIN_WIDTH, 288);

            ItemEditor.state = state;

            if (state == State.Close) {
                window.Close();
            }

            else {
                window.Show();
            }
        }

        public void OnEnable() {
            diplomataEditor = (Diplomata)AssetHandler.Read("Diplomata.asset", "Diplomata/");
        }

        public static void OpenEdit(Item item) {
            diplomataEditor = (Diplomata)AssetHandler.Read("Diplomata.asset", "Diplomata/");
            ItemEditor.item = item;
            diplomataEditor.SetWorkingItemId(item.id);
            Init(State.Edit);
        }

        public void OnGUI() {
            DGUI.Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginVertical(DGUI.windowStyle);

            switch(state) {
                case State.None:

                    if (diplomataEditor.GetWorkingItemId() != -1) {
                        item = Item.Find(diplomataEditor.inventory.items, diplomataEditor.GetWorkingItemId());
                        DrawEditWindow();
                    }

                    break;

                case State.Edit:
                    DrawEditWindow();
                    break;
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public void DrawEditWindow() {

            var name = DictHandler.ContainsKey(item.name, diplomataEditor.preferences.currentLanguage);
            
            GUILayout.Label("Name: ");
            name.value = EditorGUILayout.TextField(name.value);

            EditorGUILayout.Separator();
            
            var description = DictHandler.ContainsKey(item.description, diplomataEditor.preferences.currentLanguage);

            DGUI.textContent.text = description.value;
            var height = DGUI.textAreaStyle.CalcHeight(DGUI.textContent, Screen.width - (2 * DGUI.MARGIN));

            GUILayout.Label("Description: ");
            description.value = EditorGUILayout.TextArea(description.value, DGUI.textAreaStyle, GUILayout.Height(height));

            EditorGUILayout.Separator();

            item.image = (Texture2D) Resources.Load(item.imagePath);

            if (item.image == null && item.imagePath != string.Empty) {
                Debug.LogWarning("Cannot find the file \"" + item.imagePath + "\" in Resources folder.");
            }

            item.highlightImage = (Texture2D)Resources.Load(item.highlightImagePath);

            if (item.highlightImage == null && item.highlightImagePath != string.Empty) {
                Debug.LogWarning("Cannot find the file \"" + item.highlightImagePath + "\" in Resources folder.");
            }

            GUILayout.Label("Image: ");
            EditorGUI.BeginChangeCheck();

            item.image = (Texture2D) EditorGUILayout.ObjectField(item.image, typeof(Texture2D), false);

            if (EditorGUI.EndChangeCheck()) {
                item.imagePath = FilenameExtract(item.image);
            }

            GUILayout.Label("Highlight Image: ");
            EditorGUI.BeginChangeCheck();

            item.highlightImage = (Texture2D)EditorGUILayout.ObjectField(item.highlightImage, typeof(Texture2D), false);

            if (EditorGUI.EndChangeCheck()) {
                item.highlightImagePath = FilenameExtract(item.highlightImage);
            }

            EditorGUILayout.Separator();

            EditorGUILayout.HelpBox("\nMake sure to store this texture in Resources folder.\n", MessageType.Info);

            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                Save();
                Close();
            }

            if (GUILayout.Button("Close", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                Save();
                Close();
            }

            GUILayout.EndHorizontal();
        }

        public string FilenameExtract(Texture2D image) {
            if (image != null) {
                var str = AssetDatabase.GetAssetPath(image).Replace("Resources/", "¬");
                var strings = str.Split('¬');
                str = strings[1].Replace(".png", "");
                str = str.Replace(".jpg", "");
                str = str.Replace(".jpeg", "");
                str = str.Replace(".psd", "");
                str = str.Replace(".tga", "");
                str = str.Replace(".tiff", "");
                str = str.Replace(".gif", "");
                str = str.Replace(".bmp", "");
                return str;
            }

            else {
                return string.Empty;
            }
        }

        public void Save() {
            diplomataEditor.SaveInventory();
        }

        public void OnDisable() {
            if (state == State.Edit && item != null) {
                Save();
            }
        }
    }

}