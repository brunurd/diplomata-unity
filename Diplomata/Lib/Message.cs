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
        public string[] audioClipName;
        public bool isAChoice;
        public bool alreadySpoked;
        public string emitter;
        public int columnId;
        
        [System.NonSerialized]
        public AudioClip[] audioClip;
        
        public Message() { }

        public Message(int id, string emitter, int columnId) {
            foreach (string str in Diplomata.preferences.attributes) {
                attributes = ArrayHandler.Add(attributes, new DictAttr(str));
            }

            foreach (Language lang in Diplomata.preferences.languages) {
                title = ArrayHandler.Add(title, new DictLang(lang.name, ""));
                content = ArrayHandler.Add(content, new DictLang(lang.name, ""));
                audioClipName = ArrayHandler.Add(audioClipName, "");
                audioClip = ArrayHandler.Add(audioClip, new AudioClip());
            }

            Update(id, emitter, columnId);
        }

        public void Update(int id, string emitter, int columnId) {
            this.id = id;
            this.emitter = emitter;
            this.columnId = columnId;
        }
    }

}