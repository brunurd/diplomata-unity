using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Diplomata {

    [Serializable]
    public class DictAttr {
        public string key;
        public byte value;

        public DictAttr(string k, byte v) {
            key = k;
            value = v;
        }

        public void SetValue(byte value) {
            this.value = value;
        }
    }

#if (UNITY_EDITOR)

    [CustomEditor(typeof(Character))]
    [CanEditMultipleObjects]
    [Serializable]
    public class CharacterEditor : Editor {
        
        public static Character character;
        SerializedProperty attributes;
        SerializedProperty description;
        SerializedProperty startOnPlay;

        public static void SetCharacter(Character character) {
            CharacterEditor.character = character;
        }

        public void OnEnable() {
            if (GameObject.Find(serializedObject.targetObject.name) != null) {
                GameObject obj = GameObject.Find(serializedObject.targetObject.name);
                character = obj.GetComponent<Character>();
            }
            attributes = serializedObject.FindProperty("attributes");
            description = serializedObject.FindProperty("description");
            startOnPlay = serializedObject.FindProperty("startOnPlay");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            int margin = 15;
            
            GUILayout.Space(margin);
            GUILayout.Label("Description: ");
            description.stringValue = GUILayout.TextArea(description.stringValue, GUILayout.Height(50));

            GUILayout.Space(margin);
            startOnPlay.boolValue = GUILayout.Toggle(startOnPlay.boolValue, "Start on play");

            GUILayout.Space(margin);
            GUILayout.Label("Character attributes: ");
            for (int i = 0; i < attributes.arraySize; i++) {
                SerializedProperty key = attributes.GetArrayElementAtIndex(i).FindPropertyRelative("key");
                SerializedProperty value = attributes.GetArrayElementAtIndex(i).FindPropertyRelative("value");
                GUILayout.BeginHorizontal();
                    GUILayout.Label(key.stringValue);
                    value.intValue = (byte)EditorGUILayout.Slider(value.intValue, 0, 100);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(margin);
            if (GUILayout.Button("Edit messages", GUILayout.Height(40))) {
                if (character != null) {
                    MessageManager.Init(character);
                }
                else {
                    MessageManager.Init();
                }
            }

            GUILayout.Space(margin);

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    [RequireComponent(typeof(Events))]
    [Serializable]
    [ExecuteInEditMode]
    public class Character : MonoBehaviour {
        public string description = "";
        public bool startOnPlay;
        public List<DictAttr> attributes = new List<DictAttr>();
        public List<Message> messages = new List<Message>();
        public List<string> startNext = new List<string>();
        public byte influence = 50;
        public bool conditions;
        public bool talking;
        public bool isTalking;
        public bool waitingPlayer;
        private Message currentMessage;
        private List<Message> currentChoices = new List<Message>();
        private Events events;
    
        public void Awake() {
            InstantiateManager();
            SetAttributes();
            #if (UNITY_EDITOR)
            CharacterEditor.SetCharacter(this);
            #endif
        }

        public void Start() {
            events = GetComponent<Events>();
            events.SetCharacter(this);
            #if (UNITY_EDITOR)
            CharacterEditor.SetCharacter(this);
            #endif

            InstantiateManager();

            talking = false;
            isTalking = false;
            waitingPlayer = false;

            if (startOnPlay) {
                StartTalk();
            }
        }

        public void InstantiateManager() {
            if (GameObject.Find("Diplomata") == null) {
                if (GameObject.Find("Diplomata(Clone)") == null) {
                    UnityEngine.Object diplomata = Resources.Load("Diplomata");
                    Instantiate(diplomata);
                }
            }
            Manager.UpdatePreferences();
        }

        public void SetAttributes() {
            if (attributes.Count == 0) {
                foreach (string attr in Manager.preferences.attributes) {
                    attributes.Add(new DictAttr(attr, 0));
                }
            }
        }

        public void StartTalk() {
            talking = true;
            currentMessage = new Message();
            currentChoices = new List<Message>();

            foreach (Message msg in messages) {
                if (msg.colunm == 0) {
                    foreach (DictLang title in msg.title) {
                        if (title.key == Options.language) {
                            startNext.Add(title.value);
                        }
                    }
                }
            }

            Next(startNext);
        }

        public void Next(List<string> nextArray) {
            List<Message> next = new List<Message>();
            currentChoices = new List<Message>();
            string emitter = null;
            isTalking = false;
            waitingPlayer = false;

            foreach (Message msg in messages) {
                if (talking) {
                    foreach (string str in nextArray) {
                        if (str == "__END") {
                            talking = false;
                            break;
                        }
                        else {
                            foreach (DictLang title in msg.title) {
                                if (title.key == Options.language) {
                                    next.Add(msg);
                                }
                            }
                        }
                    }
                }
            }

            if (talking) {
                foreach (Message msg in next) {
                    conditions = true;
                    msg.conditions.Invoke();

                    if (conditions) {
                        if (emitter == null) {
                            emitter = msg.emitter;
                        }

                        if (emitter != "Player") {
                            isTalking = true;
                            currentMessage = msg;
                            break;
                        }

                        if (emitter == "Player" && msg.emitter == "Player") {
                            waitingPlayer = true;
                            currentChoices.Add(msg);
                        }
                    }
                }
            }
        }

        public string ShowMessageContent() {
            string newContent = currentMessage.emitter + ": ";

            if (talking) {
                foreach (DictLang content in currentMessage.content) {
                    if (content.key == Options.language) {
                        newContent += content.value;
                    }
                }
            }

            return newContent;
        }

        public void NextMessage() {
            currentMessage.callback.Invoke();
            Next(currentMessage.next);
        }

        public List<string> MessageChoices() {
            List<string> returnChoices = new List<string>();

            foreach (Message msg in currentChoices) {
                foreach (DictLang title in msg.title) {
                    if (title.key == Options.language) {
                        returnChoices.Add(title.value);
                    }
                }
            }

            return returnChoices;
        }

        public void ChooseMessage(string title) {
            foreach (Message msg in currentChoices) {
                foreach (DictLang titleTemp in msg.title) {
                    if (titleTemp.key == Options.language && titleTemp.value == title) {
                        currentMessage = msg;
                        SetInfluence();
                        break;
                    }
                }
            }
        }

        public void EndTalk() {
            Debug.Log(" Talking with" + name + " finished.");
            talking = false;
        }

        public void SetInfluence() {
            byte max = 0;
            List<byte> min = new List<byte>();

            foreach (DictAttr attrMsg in currentMessage.attributes) {
                foreach (DictAttr attrChar in attributes) {
                    if (attrMsg.value < attrChar.value) {
                        min.Add(attrMsg.value);
                    }
                    else {
                        min.Add(attrChar.value);
                    }
                }
            }

            foreach (byte val in min) {
                if (val > max) {
                    max = val;
                }
            }

            int tempInfluence = (max + influence) / 2;
            influence = (byte)tempInfluence;
        }
        
    }
}