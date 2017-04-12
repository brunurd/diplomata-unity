using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace DiplomataLib {

    [Serializable]
    public class DictLang {
        public string key;
        public string value;

        public DictLang() { }

        public DictLang(string key, string value) {
            this.key = key;
            this.value = value;
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
        //public UnityEvent conditions;
        //public UnityEvent callback;
        //public Character character;
        public List<string> next;

        public Message(Character charct, int c, int r) {
            colunm = c;
            row = r;
            emitter = charct.name;
            /*
            character = charct;
            conditions = new UnityEvent();
            callback = new UnityEvent();
            attributes = new List<DictAttr>();
            
            foreach (string str in Diplomata.preferences.attributes) {
                attributes.Add(new DictAttr(str, 50));
            }

            title = new List<DictLang>();

            foreach (string str in Diplomata.preferences.subLanguages) {
                title.Add(new DictLang(str, ""));
            }

            content = new List<DictLang>();
            
            foreach (string str in Diplomata.preferences.subLanguages) {
                content.Add(new DictLang(str, ""));
            }*/

            SetNext();
        }

        public Message() {
        }

        public void SetNext() {
            next = new List<string>();/*
            foreach (Message msg in character.messages) {
                if (msg.colunm == colunm + 1) {
                    foreach (DictLang titleTemp in msg.title) {
                        if (titleTemp.key == Diplomata.gameProgress.currentSubtitledLanguage) {
                            next.Add(titleTemp.value);
                        }
                    }
                }
            }*/
        }
    }

}