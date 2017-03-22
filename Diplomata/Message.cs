using UnityEngine.Events;
using System.Collections.Generic;
using System;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;

namespace Diplomata {

#if (UNITY_EDITOR)
    
    public class MessageEditor : EditorWindow {

        public static Message message;
        public static int resetTimer;
        public static bool emitterPlayer;
        public static bool emitterCharacter;
        public static SerializedObject characterObject;
        public static SerializedProperty messageObject;
        public static SerializedProperty conditions;
        public static SerializedProperty callback;
        
        public static void Init(Message message) {
            MessageEditor.message = message;

            if (message.emitter == "Player") {
                emitterPlayer = true;
                emitterCharacter = false;
            }
            else {
                emitterPlayer = false;
                emitterCharacter = true;
            }

            GetEventsField();

            MessageEditor window = (MessageEditor)GetWindow(typeof(MessageEditor), false, "Edit Message", true);
            window.minSize = new Vector2(340,595);
            window.Show();
        }

        public void OnGUI() {

            int margin = 15;
            characterObject.Update();

            GUILayout.Space(margin);
            GUILayout.Label("SPEECH " + message.colunm + " - Message " + message.row);

            GUILayout.Space(margin);
            EditorGUILayout.PropertyField(conditions,true);
            
            GUILayout.Space(margin);
            GUILayout.Label("Emitter:");
            GUILayout.BeginHorizontal(GUILayout.Width(200));

            if (emitterPlayer = GUILayout.Toggle(emitterPlayer, "Player")) {
                emitterCharacter = false;
            }

            if (emitterCharacter = GUILayout.Toggle(emitterCharacter, message.character.name)) {
                emitterPlayer = false;
            }

            GUILayout.EndHorizontal();
            
            if (emitterPlayer) {
                message.emitter = "Player";
                emitterCharacter = false;
            }

            if (emitterCharacter) {
                message.emitter = message.character.name;
                emitterPlayer = false;
            }

            GUILayout.Space(margin);
            GUILayout.Label("Title in '" + MessageManager.languagesArray[MessageManager.languageIndex] + "':");
            message.title[MessageManager.languageIndex].value = GUILayout.TextField(message.title[MessageManager.languageIndex].value);

            GUILayout.Space(margin);
            GUILayout.Label("Content in '" + MessageManager.languagesArray[MessageManager.languageIndex] + "':");
            message.content[MessageManager.languageIndex].value = GUILayout.TextArea(message.content[MessageManager.languageIndex].value, GUILayout.Height(50));
            
            if (message.emitter == "Player") {
                GUILayout.Space(margin);
                foreach (DictAttr attr in message.attributes) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(attr.key);
                    attr.value = (byte)EditorGUILayout.Slider(attr.value, 0, 100);
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.Space(margin);
            EditorGUILayout.PropertyField(callback, true);
            
            GUILayout.Space(margin);
            if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 60, 100, 30), "Save changes")) {
                Manager.instance.currentCharacterIndex = Manager.instance.characters.IndexOf(message.character);
                MessageManager.Init();
                this.Close();
            }

            characterObject.ApplyModifiedProperties();
        }

        public static void GetEventsField() {
            characterObject = new SerializedObject(message.character);
            messageObject = characterObject.FindProperty("messages");
            conditions = messageObject.GetArrayElementAtIndex(message.character.messages.IndexOf(message)).FindPropertyRelative("conditions");
            callback = messageObject.GetArrayElementAtIndex(message.character.messages.IndexOf(message)).FindPropertyRelative("callback");
        }

    }

#endif

    [Serializable]
    public class DictLang {
        public string key;
        public string value;

        public DictLang(string k, string v) {
            key = k;
            value = v;
        }
    }

    [Serializable]
    public class Message {
        public int colunm;
        public int row;
        public string emitter;
        public List<DictLang> title;
        public List<DictLang> content;
        public List<DictAttr> attributes;
        public UnityEvent conditions;
        public UnityEvent callback;
        public Character character;
        public List<string> next;

        public Message(Character charct, int c, int r) {
            colunm = c;
            row = r;
            emitter = charct.name;
            character = charct;
            conditions = new UnityEvent();
            callback = new UnityEvent();

            attributes = new List<DictAttr>();
            foreach (string str in Manager.preferences.attributes) {
                attributes.Add(new DictAttr(str, 50));
            }

            title = new List<DictLang>();
            foreach (string str in Manager.preferences.subLanguages) {
                title.Add(new DictLang(str, ""));
            }

            content = new List<DictLang>();
            foreach (string str in Manager.preferences.subLanguages) {
                content.Add(new DictLang(str, ""));
            }

            SetNext();
        }

        public Message() {
        }

        public void SetNext() {
            next = new List<string>();
            foreach (Message msg in character.messages) {
                if (msg.colunm == colunm + 1) {
                    foreach (DictLang titleTemp in msg.title) {
                        if (titleTemp.key == Options.language) {
                            next.Add(titleTemp.value);
                        }
                    }
                }
            }
        }
    }

}