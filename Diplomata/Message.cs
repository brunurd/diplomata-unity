using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace Diplomata {

    [Serializable]
    public struct DictLang {
        public string key;
        public string value;

        public DictLang(string k, string v) {
            key = k;
            value = v;
        }
    }

    [Serializable]
    public struct Message {
        public static int countId;
        public int id;
        public string emitter;
        public int colunm;
        public int row;
        public string mainLanguage;
        public List<DictLang> title;
        public List<DictLang> content;
        public List<DictAttr> attributes;
        public List<string> next;
        public UnityEvent conditions;
        public UnityEvent callback;
        public Character character;

        public Message(Character charct, int c, int r) {
            id = countId;
            countId += 1;
            colunm = c;
            row = r;
            emitter = "";
            mainLanguage = "";
            attributes = new List<DictAttr>();
            title = new List<DictLang>();
            content = new List<DictLang>();
            next = new List<string>();
            conditions = new UnityEvent();
            callback = new UnityEvent();
            character = charct;
        }
    }

}