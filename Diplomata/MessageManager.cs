#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Diplomata {

    public class MessageManager : EditorWindow {

        public readonly static int headerSize = 62;
        private readonly static int colunmLimit = 100;
        private readonly static int rowLimit = 100;
        private readonly static int windowContentWidth = colunmLimit * 170;
        private readonly static int windowContentHeight = headerSize + (rowLimit * 135);

        public static Character character;
        private readonly Color bgColor1 = new Color(0.085f, 0.085f, 0.085f);
        private readonly Color bgColor2 = new Color(0.125f, 0.125f, 0.125f);
        private GUIStyle bgStyle;
        public static List<List<Node>> colunms;
        public static List<string> languages;
        public static string[] languagesArray;
        public static int languageIndex;
        public static GUIStyle style;
        public static bool close;
        public Vector2 scrollPos;

        static public void Init() {
            close = false;
            style = new GUIStyle();
            character = Manager.instance.characters[Manager.instance.currentCharacterIndex];
            Manager.UpdatePreferences();
            SetLanguages();
            ResetColunms();
            MessageManager window = (MessageManager)GetWindow(typeof(MessageManager), false, "Messages", true);
            window.minSize = new Vector2(730, 550);
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


            foreach (Message msg in character.messages) {
                msg.SetNext();

                if (colunms.Count == msg.colunm + 1) {
                    msg.next.Add("__END");
                }
            }

            // Last add node in every colunm
            for (int i = 0; i <= colunmsMax; i++) {
                if (colunms[i].Count < rowLimit) {
                    colunms[i].Add(new Node(i, colunms[i].Count, character));
                }
            }

            // Add node in last colunm
            if (character.messages.Count > 0) {
                if (colunms.Count < colunmLimit) {
                    colunms.Add(new List<Node>());
                    colunms[colunms.Count - 1].Add(new Node(colunmsMax + 1, 0, character));
                }
            }

            // Add node in first colunm
            else {
                colunms.Add(new List<Node>());
                colunms[colunms.Count - 1].Add(new Node(0, 0, character));
            }
        }

        public void DrawBG() {
            bool turn = false;
            Color textColor;
            for (int i = 0; i < windowContentWidth; i += 170) {
                if (turn) {
                    EditorGUI.DrawRect(new Rect(i, headerSize - 30, 170, windowContentHeight), bgColor2);
                    textColor = new Color(0.325f, 0.325f, 0.325f);
                    turn = false;
                }
                else {
                    EditorGUI.DrawRect(new Rect(i, headerSize - 30, 170, windowContentHeight), bgColor1);
                    textColor = new Color(0.285f, 0.285f, 0.285f);
                    turn = true;
                }

                style.normal.textColor = textColor;
                GUI.Label(new Rect(i + 10, headerSize - 18, 150, 20), "SPEECH " + i / 170, style);
            }
        }

        public void DrawHeader() {
            GUI.Label(new Rect(5, 7, 80, headerSize - 30), Manager.logo);
            GUI.Label(new Rect(110, 10, 70, headerSize - 30), "Character: ");
            character = EditorGUI.ObjectField(new Rect(180, 10, 200, 16), character, typeof(Character), true) as Character;
            GUI.Label(new Rect(410, 10, 70, headerSize - 30), "Language: ");
            languageIndex = EditorGUI.Popup(new Rect(480, 10, 60, 16), languageIndex, languagesArray);

            if (GUI.Button(new Rect(Screen.width - 150, 5, 140, headerSize - 40),"Save as screenplay")) {
                Debug.Log("Saved.");
            }
        }

        public void OnGUI() {
            Character characterTemp = character;
            int indexTemp = languageIndex;

            DrawHeader();

            scrollPos = GUI.BeginScrollView(
                new Rect(0, headerSize - 30, Screen.width, Screen.height - headerSize + 8), 
                scrollPos, 
                new Rect(0, headerSize - 30, windowContentWidth, windowContentHeight));
            
            DrawBG();
            
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

                    try {
                        for (int j = 0; j < colunms[i].Count; j++) {
                            if (colunms[i][j] != null) {
                                if (!colunms[i][j].isAdd) {
                                    colunms[i][j].Draw();
                                }
                                else {
                                    colunms[i][j].DrawAdd();
                                }
                            }
                        }
                    }
                    catch(Exception e) {
                        Debug.LogWarning(e.Message);
                    }

                }
            }

            GUI.EndScrollView();

            if (close) {
                Close();
            }
        }
    }

}

#endif
