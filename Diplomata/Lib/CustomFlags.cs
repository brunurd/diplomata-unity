namespace DiplomataLib {

    [System.Serializable]
    public class Flag {
        public string name;
        public bool value;

        public Flag(string name, bool value) {
            this.name = name;
            this.value = value;
        }

        public void Set(string name, bool value) {
            this.name = name;
            this.value = value;
        }
    }

    [System.Serializable]
    public class CustomFlags {

        public Flag[] flags = new Flag[0];
        
        public Flag Find(string name) {
            foreach(Flag flag in flags) {
                if (flag.name == name) {
                    return flag;
                }
            }

            return null;
        }
    }

}
