using UnityEngine;
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
        public bool isListening;
        private Message currentMessage;
        private List<Message> currentChoices = new List<Message>();
        private Events events;

        public void Awake() {
            Manager.Instantiate();
            SetAttributes();

            bool canAdd = true;

            foreach (Character character in Manager.characters) {
                if (character != null) {
                    if (character.gameObject.Equals(gameObject)) {
                        canAdd = false;
                        break;
                    }
                }
            }

            if (canAdd) {
                Manager.characters.Add(this);
            }
        }

        public void Start() {
            events = GetComponent<Events>();
            events.SetCharacter(this);
            
            Manager.Instantiate();

            talking = false;
            isTalking = false;
            waitingPlayer = false;
            isListening = false;

            if (startOnPlay) {
                StartTalk();
            }
        }

        public void SetAttributes() {
            if (attributes.Count == 0) {
                foreach (string attr in Preferences.attributes) {
                    attributes.Add(new DictAttr(attr, 0));
                }
            }
        }

        public void StartTalk() {
            talking = true;
            currentMessage = new Message();
            currentChoices = new List<Message>();
            startNext = new List<string>();

            foreach (Message msg in messages) {
                if (msg.colunm == 0) {
                    foreach (DictLang title in msg.title) {
                        if (title.key == GameProgress.currentSubtitledLanguage) {
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
            isListening = false;

            foreach (Message msg in messages) {
                if (talking) {
                    foreach (string str in nextArray) {
                        if (str == "__END") {
                            talking = false;
                            break;
                        }
                        else {
                            foreach (DictLang title in msg.title) {
                                if (title.key == GameProgress.currentSubtitledLanguage) {
                                    if (title.value == str) {
                                        next.Add(msg);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (talking) {
                for (int i = 0; i < next.Count; i++) {
                    conditions = true;
                    next[i].conditions.Invoke();

                    if (conditions) {
                        if (emitter == null) {
                            emitter = next[i].emitter;
                        }

                        if (emitter != "Player") {
                            isTalking = true;
                            currentMessage = next[i];
                            break;
                        }

                        if (emitter == "Player" && next[i].emitter == "Player") {
                            waitingPlayer = true;
                            currentChoices.Add(next[i]);
                        }
                    }

                    else if (i == next.Count - 1) {
                        talking = false;
                    }
                }
            }
        }

        public string ShowMessageContent() {
            string newContent = currentMessage.emitter + ":\n";

            if (talking) {
                foreach (DictLang content in currentMessage.content) {
                    if (content.key == GameProgress.currentSubtitledLanguage) {
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
                    if (title.key == GameProgress.currentSubtitledLanguage) {
                        returnChoices.Add(title.value);
                    }
                }
            }

            return returnChoices;
        }

        public void ChooseMessage(string title) {
            events.lastChoice = title;
            foreach (Message msg in currentChoices) {
                foreach (DictLang titleTemp in msg.title) {
                    if (titleTemp.key == GameProgress.currentSubtitledLanguage && titleTemp.value == title) {
                        waitingPlayer = false;
                        isListening = true;
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
                    if (attrMsg.key == attrChar.key) {
                        if (attrMsg.value < attrChar.value) {
                            min.Add(attrMsg.value);
                            break;
                        }
                        if (attrMsg.value >= attrChar.value) {
                            min.Add(attrChar.value);
                            break;
                        }
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
