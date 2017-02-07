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
        public static string[] languages;
        public int languageIndex;

        static public void Init(Character character) {
            MessageManager.character = character;
            MessageManager window = (MessageManager)GetWindow(typeof(MessageManager), false, "Messages", true);
            window.Show();
            //SetLanguages();
            ResetColunms();
        }

        [MenuItem("Diplomata/Message(s) Manager")]
        static public void Init() {
            MessageManager window = (MessageManager)GetWindow(typeof(MessageManager), false, "Messages", true);
            window.Show();
            //SetLanguages();
            ResetColunms();
        }

        public static void SetLanguages() {
            string[] subLanguages = Manager.preferences.subLanguages;
            string[] dubLanguges = Manager.preferences.dubLanguages;

            for (int i = 0; i < subLanguages.Length; i++) {
                languages[i] = subLanguages[i];
            }

            for (int i = 0; i < dubLanguges.Length; i++) {
                bool hasEqual = false;
                for (int j = 0; j < languages.Length; j++) {
                    if (dubLanguges[i] == languages[j]) {
                        hasEqual = true;
                    }
                }
                if (!hasEqual) {
                    languages[languages.Length] = dubLanguges[i];
                }
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
                //colunms[msg.colunm].Add(new Node(msg.colunm, msg.row, msg.emitter, msg.title, msg, character));
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
            GUI.Label(new Rect(310, 10, 100, headerSize), "Language: ");
            //EditorGUI.Popup(new Rect(100, 10, 200, 16), languageIndex, languages);
        }

        public void OnGUI() {
            EditorGUILayout.BeginScrollView(new Vector2(0,0));
            DrawBG();
            DrawHeader();
            if (character != null) {/*
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
                }*/

            }
            EditorGUILayout.EndScrollView();
        }
    }

#endif

}