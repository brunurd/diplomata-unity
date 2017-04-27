namespace DiplomataLib {

    [System.Serializable]
    public class Effect {

        public string displayName = "None";
        public Type type;
        public DictLang[] endContextName = new DictLang[0];
        public DictLang[] nextMessage = new DictLang[0];
        public int nextMessageId = -1;
        public int nextMessageColumnId = -1;
        public int endContextId = -1;

        [System.NonSerialized]
        public Events custom = new Events();

        public enum Type {
            None,
            EndOfContext,
            GoTo
        }

        public Effect() { }
        
        public void DisplayNone() {
            displayName = "None";
        }

        public void DisplayEndOfContext() {
            displayName = "End of the context\n<i>" + endContextName[0].value + "</i>";
        }

        public void DisplayGoTo() {
            displayName = "Go to <i>" + nextMessage[0].value +"</i>";
        }
    }

}
