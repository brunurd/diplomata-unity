namespace DiplomataLib {

    [System.Serializable]
    public class Context {
        public string name;
        public string characterName;
        public bool happened;
        public Column[] columns;

        public Context() { }

        public Context(string name, string characterName) {
            this.name = name;
            this.characterName = characterName;
            columns = new Column[] { new Column(0) };
        }
    }

}
