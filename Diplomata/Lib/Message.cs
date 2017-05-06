using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    public class Message {
        
        public int id;
        public bool isAChoice;
        public bool disposable;
        public string emitter;
        public int columnId;
        public string imagePath = string.Empty;
        public Color color;
        public Condition[] conditions;
        public DictLang[] title;
        public DictLang[] content;
        public DictLang[] screenplayNotes;
        public DictAttr[] attributes;
        public Effect[] effects;
        public AnimatorAttributeSetter[] animatorAttributesSetters;
        public DictLang[] audioClipPath;

        [System.NonSerialized]
        public bool alreadySpoked;

        [System.NonSerialized]
        public AudioClip audioClip;

        [System.NonSerialized]
        public Texture2D image;

        [System.NonSerialized]
        public Sprite sprite;

        public Message() { }

        public Message(int id, string emitter, int columnId) {
            conditions = new Condition[0];
            title = new DictLang[0];
            content = new DictLang[0];
            attributes = new DictAttr[0];
            screenplayNotes = new DictLang[0];
            effects = new Effect[0];
            audioClipPath = new DictLang[0];
            animatorAttributesSetters = new AnimatorAttributeSetter[0];

            foreach (string str in Diplomata.preferences.attributes) {
                attributes = ArrayHandler.Add(attributes, new DictAttr(str));
            }

            foreach (Language lang in Diplomata.preferences.languages) {
                title = ArrayHandler.Add(title, new DictLang(lang.name, "placeholder" + columnId.ToString() + id.ToString()));
                content = ArrayHandler.Add(content, new DictLang(lang.name, "[ Message content here ]"));
                screenplayNotes = ArrayHandler.Add(screenplayNotes, new DictLang(lang.name, ""));
                audioClipPath = ArrayHandler.Add(audioClipPath, new DictLang(lang.name, string.Empty));
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

        public Sprite GetSprite(Vector2 pivot) {
            if (sprite == null) {
                image = (Texture2D) Resources.Load(imagePath);
                sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), pivot);
            }

            return sprite;
        }

        public static Message Find(Message[] messages, int rowId) {
            foreach (Message message in messages) {
                if (message.id == rowId) {
                    return message;
                }
            }
            
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