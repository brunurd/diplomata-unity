namespace DiplomataLib {

    [System.Serializable]
    public class Context {
        public string name;
        public string characterName;
        public bool happened;
        public Message[] messages;

        public Context() { }

        public Context(string name, string characterName) {
            this.name = name;
            this.characterName = characterName;
        }
    }

}
