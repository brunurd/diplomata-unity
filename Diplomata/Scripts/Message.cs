using UnityEngine.Events;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Diplomata {

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
            foreach (string str in Preferences.attributes) {
                attributes.Add(new DictAttr(str, 50));
            }

            title = new List<DictLang>();
            foreach (string str in Preferences.subLanguages) {
                title.Add(new DictLang(str, ""));
            }

            content = new List<DictLang>();
            foreach (string str in Preferences.subLanguages) {
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
                        if (titleTemp.key == GameProgress.currentSubtitledLanguage) {
                            next.Add(titleTemp.value);
                        }
                    }
                }
            }
        }
    }

}