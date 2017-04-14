namespace DiplomataLib {

    [System.Serializable]
    public class DictAttr {
        public string key;
        public byte value;

        public DictAttr() { }

        public DictAttr(string key) {
            this.key = key;
            value = 50;
        }
    }

    [System.Serializable]
    public class DictLang {
        public string key;
        public string value;

        public DictLang() { }

        public DictLang(string key, string value) {
            this.key = key;
            this.value = value;
        }
    }

}