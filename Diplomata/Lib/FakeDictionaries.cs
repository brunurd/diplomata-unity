namespace DiplomataLib {

    [System.Serializable]
    public class Dict {
        public string key;
    }

    [System.Serializable]
    public class DictAttr : Dict {
        public byte value;

        public DictAttr() { }

        public DictAttr(string key) {
            this.key = key;
        }
    }

    [System.Serializable]
    public class DictLang : Dict {
        public string value;

        public DictLang() { }

        public DictLang(string key, string value) {
            this.key = key;
            this.value = value;
        }
    }

    public class DictHandler {

        public static DictAttr ContainsKey(DictAttr[] array, string key) {
            for (int i = 0; i < array.Length; i++) {
                if (array[i].key == key) {
                    return array[i];
                }
            }

            return null;
        }

        public static DictLang ContainsKey(DictLang[] array, string key) {
            for (int i = 0; i < array.Length; i++) {
                if (array[i].key == key) {
                    return array[i];
                }
            }

            return null;
        }
    }

}