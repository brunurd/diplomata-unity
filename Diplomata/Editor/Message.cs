using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

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
            window.minSize = new Vector2(340, 595);
            window.Show();
        }

        public void OnGUI() {

            int margin = 15;
            characterObject.Update();

            GUILayout.Space(margin);
            GUILayout.Label("SPEECH " + message.colunm + " - Message " + message.row);

            GUILayout.Space(margin);
            EditorGUILayout.PropertyField(conditions, true);

            GUILayout.Space(margin);
            GUILayout.Label("Emitter:");
            GUILayout.BeginHorizontal(GUILayout.Width(200));

            if (emitterPlayer = GUILayout.Toggle(emitterPlayer, "Player")) {
                emitterCharacter = false;
            }
            /*
            if (emitterCharacter = GUILayout.Toggle(emitterCharacter, message.character.name)) {
                emitterPlayer = false;
            }*/

            GUILayout.EndHorizontal();

            if (emitterPlayer) {
                message.emitter = "Player";
                emitterCharacter = false;
            }

            if (emitterCharacter) {
                //message.emitter = message.character.name;
                emitterPlayer = false;
            }

            GUILayout.Space(margin);
            GUILayout.Label("Title in '" + MessageManager.languagesArray[MessageManager.languageIndex] + "':");
            //message.title[MessageManager.languageIndex].value = GUILayout.TextField(message.title[MessageManager.languageIndex].value);

            GUILayout.Space(margin);
            GUILayout.Label("Content in '" + MessageManager.languagesArray[MessageManager.languageIndex] + "':");
            //message.content[MessageManager.languageIndex].value = GUILayout.TextArea(message.content[MessageManager.languageIndex].value, GUILayout.Height(50));

            if (message.emitter == "Player") {
                GUILayout.Space(margin);
                /*
                foreach (DictAttr attr in message.attributes) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(attr.key);
                    attr.value = (byte)EditorGUILayout.Slider(attr.value, 0, 100);
                    GUILayout.EndHorizontal();
                }*/
            }

            GUILayout.Space(margin);
            EditorGUILayout.PropertyField(callback, true);

            GUILayout.Space(margin);
            if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 60, 100, 30), "Save changes")) {
                //Diplomata.instance.currentCharacterIndex = Diplomata.instance.characters.IndexOf(message.character);
                MessageManager.Init();
                this.Close();
            }

            characterObject.ApplyModifiedProperties();
        }

        public static void GetEventsField() {
            //characterObject = new SerializedObject(message.character);
            messageObject = characterObject.FindProperty("messages");
            //conditions = messageObject.GetArrayElementAtIndex(message.character.messages.IndexOf(message)).FindPropertyRelative("conditions");
            //callback = messageObject.GetArrayElementAtIndex(message.character.messages.IndexOf(message)).FindPropertyRelative("callback");
        }

    }

}