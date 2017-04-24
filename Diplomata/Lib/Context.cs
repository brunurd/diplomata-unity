namespace DiplomataLib {

    public enum MessageEditorState {
        None,
        Normal,
        Conditions,
        Effects
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
        public int id;
        public DictLang[] name;
        public DictLang[] description;
        public string characterName;
        public bool happened;
        public Column[] columns;
        public bool idFilter = false;
        public bool conditionsFilter = true;
        public bool titleFilter = true;
        public bool contentFilter = true;
        public bool callbacksFilter = true;
        public CurrentMessage currentMessage = new CurrentMessage(-1, -1);
        public MessageEditorState messageEditorState = MessageEditorState.None;
        public ushort columnWidth = 200;

        public Context() { }

        public Context(int id, string characterName) {
            this.id = id;
            this.characterName = characterName;
            columns = new Column[0];
            name = new DictLang[0];
            description = new DictLang[0];

            foreach (Language lang in Diplomata.preferences.languages) {
                name = ArrayHandler.Add(name, new DictLang(lang.name, "Name [Change clicking on Edit]"));
                description = ArrayHandler.Add(description, new DictLang(lang.name, "Description [Change clicking on Edit]"));
            }
        }

        public static Context Find(Character character, int id) {

            foreach (Context context in character.contexts) {
                if (context.id == id) {
                    return context;
                }
            }

            return null;
        }

        public static Context[] ResetIDs(Context[] array) {

            for (int i = 0; i < array.Length; i++) {
                if (array[i].id == i + 1) {
                    array[i].id = i;
                }
            }

            return array;
        }
    }

}
