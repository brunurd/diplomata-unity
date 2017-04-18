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

            messages = new Message[0];
        }

        public static Column Find(Context context, int columnId) {

            foreach (Column column in context.columns) {
                if (column.id == columnId) {
                    return column;
                }
            }

            return null;
        }

        public static Column[] RemoveEmptyColumns(Column[] array) {
            
            for (int i = 0; i < array.Length; i++) {
                if (array[i].messages.Length == 0) {
                    array = ArrayHandler.Remove(array, array[i]);
                }
            }

            for (int i = 0; i < array.Length; i++) {
                if (array[i].id == i + 1) {
                    array[i].id = i;
                }
            }

            return array;
        }
    }

}
