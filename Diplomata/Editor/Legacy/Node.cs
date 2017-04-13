using UnityEditor;
using UnityEngine;
using DiplomataLib;

namespace DiplomataEditor_Legacy {
    
    public class Node {
        private const int padding = 10;
        private const int width = 150;
        private const int height = 125;
        private readonly Color bodyColor = new Color(0.8705f, 0.8705f, 0.8705f);
        public int colunm;
        public int row;
        private int x;
        private int y;
        private Color headerColor;
        public string emitter;
        public string title;
        public Message message;
        public bool isAdd;
        public DiplomataLib.Character character;
        public int colunmLimit;

        public Node(int col, int row, Character character) {
            SetPosition(col, row);
            this.character = character;
            isAdd = true;
        }

        public Node(int col, int row, string emitter, string title, Message message, Character character) {
            SetPosition(col, row);
            this.emitter = emitter;
            this.title = title;
            this.message = message;
            isAdd = false;
            this.character = character;

            if (emitter == "Player") {
                headerColor = new Color(0.3333f, 0.0823f, 0.7411f);
            }

            else {
                headerColor = new Color(0.1882f, 0.6509f, 0.8039f);
            }
        }

        public void SetPosition(int col, int row) {
            colunm = col;
            this.row = row;
            x = (padding + ((col * 2) * padding) + (col * width));
            y = (MessageManager.headerSize + (row + 1) * padding) + (row * height);
        }

        public void Draw() {
            EditorGUI.DrawRect(new Rect(x, y, width, 25), headerColor);
            MessageManager.style.normal.textColor = Color.white;
            GUI.Label(new Rect(x + 5, y + 5, width - 10, 15), emitter, MessageManager.style);
            GUI.Label(new Rect(x + width - 20, y + 5, 20, 15), ""+row, MessageManager.style);
            EditorGUI.DrawRect(new Rect(x, y + 25, width, 100), bodyColor);
            GUI.Label(new Rect(x + 5, y + 40, width - 10, 25), title);

            /*
            if (GUI.Button(new Rect((x + width) - 110, y + 75, 50, 20), "Up")) {
                if (row > 0) {
                    row -= 1;
                    message.row = row;
                }
                MessageManager.ResetColunms();
            }

            if (GUI.Button(new Rect((x + width) - 55, y + 75, 50, 20), "Down")) {
                if (row < colunmLimit) {
                    row += 1;
                    message.row = row;
                }
                MessageManager.ResetColunms();
            }

            if (GUI.Button(new Rect(x + 5, (y + height) - 25, 20, 20), "X")) {
                character.messages.Remove(this.message);
                MessageManager.ResetColunms();
            }
            */

            if (GUI.Button(new Rect(x + 30, (y + height) - 25, width - 35, 20), "Edit")) {
                MessageManager.close = true;
                MessageEditor.Init(this.message);
            }
        }

        public void DrawAdd() {
            if (GUI.Button(new Rect(x, y, width, 40), "Add Message")) {
                AddNode();
            }
        }

        public void AddNode() {
            MessageManager.close = true;
            //character.messages.Add(new Message(character, colunm, row));
            //message = character.messages[character.messages.Count - 1];
            MessageEditor.Init(message);
            MessageManager.ResetColunms();
        }
    }

}