namespace DiplomataLib {

    public enum MessageEditorState {
        None,
        Normal,
        Conditions,
        Effects
    }
    
    [System.Serializable]
    public class Context {
        public int id;
        public string characterName;
        public bool idFilter = false;
        public bool conditionsFilter = true;
        public bool titleFilter = true;
        public bool contentFilter = true;
        public bool effectsFilter = true;
        public CurrentMessage currentMessage = new CurrentMessage(-1, -1);
        public MessageEditorState messageEditorState = MessageEditorState.None;
        public ushort columnWidth = 200;
        public ushort fontSize = 11;
        public DictLang[] name;
        public DictLang[] description;
        public Column[] columns;

        [System.NonSerialized]
        public bool happened;

        public struct CurrentMessage {
            public int columnId;
            public int rowId;

            public CurrentMessage(int columnId, int rowId) {
                this.columnId = columnId;
                this.rowId = rowId;
            }

            public void Set(int columnId, int rowId) {
                this.columnId = columnId;
                this.rowId = rowId;
            }
        }

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
            if (character != null) {
                foreach (Context context in character.contexts) {
                    if (context.id == id) {
                        return context;
                    }
                }
            }

            return null;
        }

        public static Context Find(Character character, string name, string language) {
            if (character != null) {

                foreach (Context context in character.contexts) {
                    DictLang contextName = DictHandler.ContainsKey(context.name, language);

                    if (name == contextName.value) {
                        return context;
                    }
                }
            }

            return null;
        }

        public static Context[] ResetIDs(Character character, Context[] array) {
            Context[] temp = new Context[0];

            for (int i = 0; i < array.Length + 1; i++) {
                Context ctx = Find(character, i);

                if (ctx != null) {
                    temp = ArrayHandler.Add(temp, ctx);
                }
            }

            for (int j = 0; j < temp.Length; j++) {
                if (temp[j].id == j + 1) {
                    temp[j].id = j;
                }
            }

            return temp;
        }
    }

}
