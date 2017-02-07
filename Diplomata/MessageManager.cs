using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Diplomata {

#if (UNITY_EDITOR)

    public class MessageManager : EditorWindow {

        public static Character character;
        private readonly Color bgColor1 = new Color(0.085f, 0.085f, 0.085f);
        private readonly Color bgColor2 = new Color(0.125f, 0.125f, 0.125f);
        private GUIStyle bgStyle;
        public static List<Node> colunms;
        public static int headerSize = 32;

        static public void Init(Character character) {
            MessageManager.character = character;
            MessageManager window = (MessageManager)GetWindow(typeof(MessageManager), false, "Messages", true);
            window.Show();
            ResetColunms();
        }

        [MenuItem("Diplomata/Message(s) Manager")]
        static public void Init() {
            MessageManager window = (MessageManager)GetWindow(typeof(MessageManager), false, "Messages", true);
            window.Show();
            ResetColunms();
        }

        public static void ResetColunms() {
            colunms = new List<Node>();

        }

        public void DrawBG() {
            bool turn = false;
            for (int i = 0; i < 17000; i += 170) {
                if (turn) {
                    EditorGUI.DrawRect(new Rect(i, headerSize, 170, Screen.height), bgColor2);
                    turn = false;
                }
                else {
                    EditorGUI.DrawRect(new Rect(i, headerSize, 170, Screen.height), bgColor1);
                    turn = true;
                }
            }
        }

        public void DrawHeader() {
            GUI.Label(new Rect(10, 10, 100, headerSize), "Character: ");
            character = EditorGUI.ObjectField(new Rect(100, 10, 200, 16), character, typeof(Character), true) as Character;
        }

        public void OnGUI() {
            EditorGUILayout.BeginScrollView(new Vector2(0,0));
            DrawBG();
            DrawHeader();
            EditorGUILayout.EndScrollView();
        }
    }

#endif

}