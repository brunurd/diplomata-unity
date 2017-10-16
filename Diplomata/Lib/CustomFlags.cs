namespace DiplomataLib {

    [System.Serializable]
    public class Flag {
        public string name;
        public bool value;

        public Flag() { }

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
        
        public Flag Find(Flag[] flagArray, string name) {
            foreach(Flag flag in flagArray) {
                if (flag.name == name) {
                    return flag;
                }
            }

            return null;
        }
    }

}
