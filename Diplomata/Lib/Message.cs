using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace DiplomataLib {

    [System.Serializable]
    public class Message {

        [SerializeField]
        private string uniqueId;

        public int id;
        public bool isAChoice;
        public bool disposable;
        public int columnId;
        public string imagePath = string.Empty;
        public Condition[] conditions;
        public DictLang[] content;
        public DictLang[] screenplayNotes;
        public DictAttr[] attributes;
        public Effect[] effects;
        public AnimatorAttributeSetter[] animatorAttributesSetters;
        public DictLang[] audioClipPath;
        public string labelId;

        [System.NonSerialized]
        public bool alreadySpoked;

        [System.NonSerialized]
        public AudioClip audioClip;

        [System.NonSerialized]
        public Texture2D image;

        [System.NonSerialized]
        public Sprite sprite;

        public Message() { }

        public Message(Message msg, int id) {
            this.id = id;
            isAChoice = msg.isAChoice;
            disposable = msg.disposable;
            columnId = msg.columnId;
            imagePath = msg.imagePath;
            labelId = msg.labelId;
            
            conditions = ArrayHandler.Copy(msg.conditions);
            content = ArrayHandler.Copy(msg.content);
            screenplayNotes = ArrayHandler.Copy(msg.screenplayNotes);
            attributes = ArrayHandler.Copy(msg.attributes);
            effects = ArrayHandler.Copy(msg.effects);
            animatorAttributesSetters = ArrayHandler.Copy(msg.animatorAttributesSetters);
            audioClipPath = ArrayHandler.Copy(msg.audioClipPath);

            uniqueId = SetUniqueId();
        }

        public Message(int id, string emitter, int columnId, string labelId) {
            conditions = new Condition[0];
            content = new DictLang[0];
            attributes = new DictAttr[0];
            screenplayNotes = new DictLang[0];
            effects = new Effect[0];
            audioClipPath = new DictLang[0];
            animatorAttributesSetters = new AnimatorAttributeSetter[0];
            this.labelId = labelId;

            foreach (string str in Diplomata.preferences.attributes) {
                attributes = ArrayHandler.Add(attributes, new DictAttr(str));
            }

            foreach (Language lang in Diplomata.preferences.languages) {
                content = ArrayHandler.Add(content, new DictLang(lang.name, "[ Message content here ]"));
                screenplayNotes = ArrayHandler.Add(screenplayNotes, new DictLang(lang.name, ""));
                audioClipPath = ArrayHandler.Add(audioClipPath, new DictLang(lang.name, string.Empty));
            }

            this.id = id;
            this.columnId = columnId;

            uniqueId = SetUniqueId();
        }

        private string SetUniqueId() {
            MD5 md5Hash = MD5.Create();

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(
                "Diplomata" + id + columnId +
                Random.Range(-2147483648, 2147483647)
            ));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public string GetUniqueId() {
            return uniqueId;
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

        public static Message Find(Message[] messages, string uniqueId) {
            foreach (Message message in messages) {
                if (message.uniqueId == uniqueId) {
                    return message;
                }
            }

            return null;
        }

        public static Message[] ResetIDs(Message[] array) {

            Message[] temp = new Message[0];

            for (int i = 0; i < array.Length + 1; i++) {
                Message msg = Find(array, i);

                if (msg != null) {
                    temp = ArrayHandler.Add(temp, msg);
                }
            }

            for (int j = 0; j < temp.Length; j++) {
                if (temp[j].id == j + 1) {
                    temp[j].id = j;
                }
            }

            return temp;
        }

        public Effect AddCustomEffect() {
            effects = ArrayHandler.Add(effects, new Effect());
            return effects[effects.Length - 1];
        }
    }

}