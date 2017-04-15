namespace DiplomataLib {

    [System.Serializable]
    public class Column {
        public int id;
        public string emitter;
        public Message[] messages;

        public Column() { }

        public Column(int id) {
            this.id = id;
            emitter = Diplomata.preferences.playerCharacterName;
        }
    }

}
