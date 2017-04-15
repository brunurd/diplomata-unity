using UnityEngine;
using UnityEditor;

namespace DiplomataEditor {

    public class MessagesEditor {

        public static Color baseColor = new Color(0,0,0);
        private static Color colorOperator = new Color(0.1f, 0.1f, 0.1f);

        public static void Draw() {

            if (EditorGUIUtility.isProSkin) {
                baseColor = DGUI.proBGColor;
            }

            else {
                baseColor = DGUI.BGColor;
            }

            Header();
            Main();
            Sidebar();
        }

        public static void Header() {
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;

            EditorGUI.DrawRect(new Rect(0, 0, Screen.width, 35), baseColor);

            DGUI.Horizontal(() => {

                GUILayout.Label(character.name);

            }, GUILayout.Height(35), GUILayout.ExpandWidth(true));
        }

        public static void Main() {
            //
        }

        public static void Sidebar() {
            //
        }

    }

}