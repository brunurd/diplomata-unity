using System.Collections.Generic;
using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class DiplomataCharacter : MonoBehaviour {
        
        public Character character;
        public bool conditions;
        public bool talking;
        public bool isTalking;
        public bool waitingPlayer;
        public bool isListening;
        //private Message currentMessage;
        public List<string> startNext = new List<string>();
        private List<Message> currentChoices = new List<Message>();
        
        public void Start() {
            talking = false;
            isTalking = false;
            waitingPlayer = false;
            isListening = false;

            if (character != null) {
                if (character.startOnPlay) {
                    StartTalk();
                }
            }
        }

        public Message FindMessage(string messageTitle, string language = "English") {
            foreach (Context context in character.contexts) {
                foreach (Column column in context.columns) {
                    for (int i = 0; i < column.messages.Length; i++) {
                        DictLang title = DictHandler.ContainsKey(Message.Find(column.messages, i).title, language);
                        if (title.value == messageTitle && title != null) {
                            return column.messages[i];
                        }
                    }
                }
            }
            return null;
        }

        public void StartTalk() {
            talking = true;
            //currentMessage = new Message();
            currentChoices = new List<Message>();
            startNext = new List<string>();
            
            /*
            foreach (Message msg in character.messages) {
                if (msg.colunm == 0) {
                    foreach (KeyValuePair<string, string> title in msg.title) {
                        if (title.Key == Diplomata.gameProgress.currentSubtitledLanguage) {
                            startNext.Add(title.Value);
                        }
                    }
                }
            }
            */

            Next(startNext);
        }

        public void Next(List<string> nextArray) {
            //List<Message> next = new List<Message>();
            currentChoices = new List<Message>();
            //string emitter = null;
            isTalking = false;
            waitingPlayer = false;
            isListening = false;

            /*
            foreach (Message msg in character.messages) {
                if (talking) {
                    foreach (string str in nextArray) {
                        if (str == "__END") {
                            talking = false;
                            break;
                        }
                        else {
                            foreach (KeyValuePair<string, string> title in msg.title) {
                                if (title.Key == Diplomata.gameProgress.currentSubtitledLanguage) {
                                    if (title.Value == str) {
                                        next.Add(msg);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            */
            /*
            if (talking) {
                for (int i = 0; i < next.Count; i++) {
                    conditions = true;
                    //next[i].conditions.Invoke();

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
            }*/
        }

        public string ShowMessageContent() {
            string newContent = "";// currentMessage.emitter + ":\n";

            if (talking) {
                /*foreach (KeyValuePair<string, string> content in currentMessage.content) {
                    if (content.Key == Diplomata.gameProgress.currentSubtitledLanguage) {
                        newContent += content.Value;
                    }
                }*/
            }

            return newContent;
        }

        public void NextMessage() {
            //currentMessage.callback.Invoke();
            //Next(currentMessage.next);
        }

        public List<string> MessageChoices() {
            List<string> returnChoices = new List<string>();

            foreach (Message msg in currentChoices) {
                /*foreach (KeyValuePair<string, string> title in msg.title) {
                    if (title.Key == Diplomata.gameProgress.currentSubtitledLanguage) {
                        returnChoices.Add(title.Value);
                    }
                }*/
            }

            return returnChoices;
        }

        public void ChooseMessage(string title) {
            foreach (Message msg in currentChoices) {
                /*foreach (KeyValuePair<string, string> titleTemp in msg.title) {
                    if (titleTemp.Key == Diplomata.gameProgress.currentSubtitledLanguage && titleTemp.Value == title) {
                        waitingPlayer = false;
                        isListening = true;
                        currentMessage = msg;
                        SetInfluence();
                        break;
                    }
                }*/
            }
        }

        public void EndTalk() {
            Debug.Log(" Talking with" + name + " finished.");
            talking = false;
        }

        public void SetInfluence() {
            byte max = 0;
            List<byte> min = new List<byte>();

            /*foreach (KeyValuePair<string, byte> attrMsg in currentMessage.attributes) {
                foreach (DictAttr attrChar in character.attributes) {
                    if (attrMsg.Key == attrChar.key) {
                        if (attrMsg.Value < attrChar.value) {
                            min.Add(attrMsg.Value);
                            break;
                        }
                        if (attrMsg.Value >= attrChar.value) {
                            min.Add(attrChar.value);
                            break;
                        }
                    }
                }
            }*/

            foreach (byte val in min) {
                if (val > max) {
                    max = val;
                }
            }

            int tempInfluence = (max + character.influence) / 2;
            character.influence = (byte)tempInfluence;
        }

        private void OnEnable() {
            if (character != null) {
                character.onScene = true;
            }
        }

        private void OnDisable() {
            if (character != null) {
                character.onScene = false;
            }
        }

        private void OnDestroy() {
            if (character != null) {
                character.onScene = false;
            }
        }
    }

}