using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    public class Message {
        public int id;
        public Condition[] conditions;
        public DictLang[] title;
        public DictLang[] content;
        public DictAttr[] attributes;
        public Callback[] callbacks;
        public string[] audioClipNames;
        public bool isAChoice;
        public bool disposable;
        public bool alreadySpoked;
        public string emitter;
        public int columnId;
        public Color color;
        
        [System.NonSerialized]
        public AudioClip[] audioClip;
        
        public Message() { }

        public Message(int id, string emitter, int columnId) {
            conditions = new Condition[0];
            title = new DictLang[0];
            content = new DictLang[0];
            attributes = new DictAttr[0];
            callbacks = new Callback[0];
            audioClipNames = new string[0];
            audioClip = new AudioClip[0];

            foreach (string str in Diplomata.preferences.attributes) {
                attributes = ArrayHandler.Add(attributes, new DictAttr(str));
            }

            foreach (Language lang in Diplomata.preferences.languages) {
                title = ArrayHandler.Add(title, new DictLang(lang.name, ""));
                content = ArrayHandler.Add(content, new DictLang(lang.name, ""));
                audioClipNames = ArrayHandler.Add(audioClipNames, "");
                audioClip = ArrayHandler.Add(audioClip, new AudioClip());
            }

            #if UNITY_EDITOR
            
            if (UnityEditor.EditorGUIUtility.isProSkin) {
                color = new Color(0.2196f, 0.2196f, 0.2196f);
            }

            else {
                color = new Color(0.9764f, 0.9764f, 0.9764f);
            }

            #endif

            Update(id, emitter, columnId);
        }

        public void Update(int id, string emitter, int columnId) {
            this.id = id;
            this.emitter = emitter;
            this.columnId = columnId;
        }

        public static Message Find(Message[] messages, int columnId, int rowId) {
            foreach (Message message in messages) {
                if (message.columnId == columnId && message.id == rowId) {
                    return message;
                }
            }

            return null;
        }
    }

}