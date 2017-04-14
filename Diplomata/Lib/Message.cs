using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    public class Message {
        public Condition[] conditions;
        public DictLang[] title;
        public DictLang[] content;
        public DictAttr[] attributes;
        public Callback[] callbacks;
        public string[] audioClipName;
        public bool isAChoice;
        public bool alreadySpoked;

        [System.NonSerialized]
        public AudioClip[] audioClip;
        
        public Message() {
            foreach (string str in Diplomata.preferences.attributes) {
                attributes = ArrayHandler.Add(attributes, new DictAttr(str));
            }

            foreach (Language lang in Diplomata.preferences.languages) {
                title = ArrayHandler.Add(title, new DictLang(lang.name, ""));
                content = ArrayHandler.Add(content, new DictLang(lang.name, ""));
                audioClipName = ArrayHandler.Add(audioClipName, "");
                audioClip = ArrayHandler.Add(audioClip, new AudioClip());
            }
        }
    }

}