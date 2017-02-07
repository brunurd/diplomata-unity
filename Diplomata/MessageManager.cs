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
        public static List<List<Node>> colunms;
        public static int headerSize = 32;
        public static List<string> languages;
        public static string[] languagesArray;
        public static int languageIndex;

        static public void Init(Character character) {
            MessageManager.character = character;
            Manager.UpdatePreferences();
            SetLanguages();
            ResetColunms();
            MessageManager window = (MessageManager)GetWindow(typeof(MessageManager), false, "Messages", true);
            window.Show();
        }

        [MenuItem("Diplomata/Message(s) Manager")]
        static public void Init() {
            Manager.UpdatePreferences();
            SetLanguages();
            MessageManager window = (MessageManager)GetWindow(typeof(MessageManager), false, "Messages", true);
            window.Show();
        }

        public static void SetLanguages() {
            languages = new List<string>();

            for (int i = 0; i < Manager.preferences.subLanguages.Length; i++) {
                languages.Add(Manager.preferences.subLanguages[i]);
            }

            for (int i = 0; i < Manager.preferences.dubLanguages.Length; i++) {
                bool hasEqual = false;
                for (int j = 0; j < languages.Count; j++) {
                    if (Manager.preferences.dubLanguages[i] == languages[j]) {
                        hasEqual = true;
                    }
                }
                if (!hasEqual) {
                    languages.Add(Manager.preferences.dubLanguages[i]);
                }
            }

            languagesArray = new string[languages.Count];

            for (int i = 0; i < languages.Count; i++) {
                languagesArray[i] = languages[i];
            }
        }

        public static void ResetColunms() {
            colunms = new List<List<Node>>();
            int colunmsMax = 0;

            foreach (Message msg in character.messages) {
                if (msg.colunm > colunmsMax) {
                    colunmsMax = msg.colunm;
                }
            }

            for (int i = 0; i <= colunmsMax; i++) {
                colunms.Add(new List<Node>());
            }

            foreach (Message msg in character.messages) {
                string title = "";
                foreach (DictLang t in msg.title) {
                    if (t.key == languagesArray[languageIndex]) {
                        title = t.value;
                    }
                }
                colunms[msg.colunm].Add(new Node(msg.colunm, msg.row, msg.emitter, title, msg, character));
            }

            if (colunms.Count > 1) {
                colunms.Add(new List<Node>());
                colunms[colunms.Count - 1].Add(new Node(colunmsMax + 1, 0, character));
            }
            else {
                colunms.Add(new List<Node>());
                colunms[colunms.Count - 1].Add(new Node(colunmsMax, 0, character));
            }
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
            GUI.Label(new Rect(330, 10, 70, headerSize), "Language: ");
            languageIndex = EditorGUI.Popup(new Rect(400, 10, 60, 16), languageIndex, MessageManager.languagesArray);
        }

        public void OnGUI() {
            Character characterTemp = character;
            int indexTemp = languageIndex;
            EditorGUILayout.BeginScrollView(new Vector2(0,0));
            DrawBG();
            DrawHeader();

            if (languageIndex != indexTemp) {
                Manager.UpdatePreferences();
                SetLanguages();
                ResetColunms();
            }

            if (character != characterTemp) {
                ResetColunms();
            }

            if (character != null) {
                for (int i = 0; i < colunms.Count; i++) {
                    for (int j = 0; j < colunms[i].Count; j++) {
                        if (colunms[i][j] != null) {
                            if (colunms[i][j].isAdd) {
                                colunms[i][j].DrawAdd();
                            }
                            else {
                                colunms[i][j].Draw();
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }

#endif

}
