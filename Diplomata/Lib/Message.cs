using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    public class Message {
        
        public int id;
        public Condition[] conditions;
        public DictLang[] title;
        public DictLang[] content;
        public DictLang[] screenplayNotes;
        public DictAttr[] attributes;
        public Effect[] effects;
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
            screenplayNotes = new DictLang[0];
            effects = new Effect[0];
            audioClipNames = new string[0];
            audioClip = new AudioClip[0];

            foreach (string str in Diplomata.preferences.attributes) {
                attributes = ArrayHandler.Add(attributes, new DictAttr(str));
            }

            foreach (Language lang in Diplomata.preferences.languages) {
                title = ArrayHandler.Add(title, new DictLang(lang.name, "placeholder" + columnId.ToString() + id.ToString()));
                content = ArrayHandler.Add(content, new DictLang(lang.name, ""));
                audioClipNames = ArrayHandler.Add(audioClipNames, "");
                screenplayNotes = ArrayHandler.Add(screenplayNotes, new DictLang(lang.name, ""));
                audioClip = ArrayHandler.Add(audioClip, new AudioClip());
            }

            #if UNITY_EDITOR
            
            if (UnityEditor.EditorGUIUtility.isProSkin) {
                color = new Color(0.2196f, 0.2196f, 0.2196f);
            }

            else {
                color = new Color(0.9764f, 0.9764f, 0.9764f);
            }

            #else
            color = new Color(0.9764f, 0.9764f, 0.9764f);
            #endif

            this.id = id;
            this.emitter = emitter;
            this.columnId = columnId;
        }

        public static Message Find(Message[] messages, int rowId) {
            foreach (Message message in messages) {
                if (message.id == rowId) {
                    return message;
                }
            }

            Debug.LogError("The message with the id " + rowId + " not found, this message doesn't exist or you mistake the id. returned null.");
            return null;
        }

        public static Message[] ResetIDs(Message[] array) {

            for (int i = 0; i < array.Length; i++) {
                if (array[i].id == i + 1) {
                    array[i].id = i;
                }
            }

            return array;
        }
    }

}