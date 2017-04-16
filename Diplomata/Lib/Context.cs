namespace DiplomataLib {

    public enum MessageEditorState {
        None,
        Normal,
        Conditions,
        Callbacks
    }

    [System.Serializable]
    public class CurrentMessage {
        public int columnId;
        public int rowId;

        public CurrentMessage() { }

        public CurrentMessage(int columnId, int rowId) {
            this.columnId = columnId;
            this.rowId = rowId;
        }

        public void Set(int columnId, int rowId) {
            this.columnId = columnId;
            this.rowId = rowId;
        }
    }

    [System.Serializable]
    public class Context {
        public string name;
        public string characterName;
        public bool happened;
        public Column[] columns;
        public string currentLanguage;
        public bool conditionsFilter = true;
        public bool titleFilter = true;
        public bool contentFilter = true;
        public bool callbacksFilter = true;
        public CurrentMessage currentMessage = new CurrentMessage(-1, -1);
        public MessageEditorState messageEditorState = MessageEditorState.None;
        public float columnWidth = 200;

        public Context() { }

        public Context(string name, string characterName) {
            this.name = name;
            this.characterName = characterName;
            columns = new Column[0];
        }

        public static Context Find(Character character, string contextName) {

            foreach (Context context in character.contexts) {
                if (context.name == contextName) {
                    return context;
                }
            }

            return null;
        }
    }

}
