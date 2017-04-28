namespace DiplomataLib {
    
    [System.Serializable]
    public class Effect {
        
        public Type type;
        public EndOfContext endOfContext;
        public GoTo goTo;
        
        [System.NonSerialized]
        public Events custom = new Events();

        public enum Type {
            None,
            EndOfContext,
            GoTo
        }

        [System.Serializable]
        public struct GoTo {
            public int columnId;
            public int messageId;

            public GoTo(int columnId, int messageId) {
                this.columnId = columnId;
                this.messageId = messageId;
            }

            public void Set(int columnId, int messageId) {
                this.columnId = columnId;
                this.messageId = messageId;
            }

            public Message GetMessage(Context context) {
                return Message.Find(Column.Find(context, columnId).messages, messageId);
            }
        }

        [System.Serializable]
        public struct EndOfContext {
            public string characterName;
            public int contextId;

            public EndOfContext(string characterName, int contextId) {
                this.characterName = characterName;
                this.contextId = contextId;
            }

            public void Set(string characterName, int contextId) {
                this.characterName = characterName;
                this.contextId = contextId;
            }

            public Context GetContext(System.Collections.Generic.List<Character> characters) {
                return Context.Find(Character.Find(characters, characterName), contextId);
            }
        }

        public Effect() { }
        
        public string DisplayNone() {
            return "None";
        }

        public string DisplayEndOfContext(string contextName) {
            return "End of the context\n<i>" + contextName + "</i>";
        }

        public string DisplayGoTo(string messageTitle) {
            return "Go to <i>"  + messageTitle + "</i>";
        }
    }

}
