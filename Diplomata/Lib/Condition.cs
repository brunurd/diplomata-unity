namespace DiplomataLib {
    
    [System.Serializable]
    public class Condition {
        
        public Type type;
        public int comparedInfluence;
        public string characterInfluencedName;
        public AfterOf afterOf;
        public int itemId;

        [System.NonSerialized]
        public Events custom = new Events();

        [System.NonSerialized]
        public bool proceed = true;
        
        public enum Type {
            None,
            AfterOf,
            InfluenceEqualTo,
            InfluenceGreaterThan,
            InfluenceLessThan,
            HasItem
        }

        [System.Serializable]
        public struct AfterOf {
            public int columnId;
            public int messageId;

            public AfterOf(int columnId, int messageId) {
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

        public Condition() { }

        public string DisplayNone() {
            return "None";
        }

        public string DisplayAfterOf(string messageTitle) {
            return "After of <i>" + messageTitle + "</i>";
        }
        
        public string DisplayCompareInfluence() {
            switch (type) {
                case Type.InfluenceEqualTo:
                    return "Influence <i>equal</i> to \n<i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>";
                case Type.InfluenceGreaterThan:
                    return "Influence <i>greater</i> then \n<i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>";
                case Type.InfluenceLessThan:
                    return "Influence <i>less</i> then \n<i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>";
                default:
                    return string.Empty;
            }
        }

        public string DisplayHasItem(string itemName) {
            return "Has the item: " + itemName;
        }

        public static bool CanProceed(Condition[] conditions) {
            foreach (Condition condition in conditions) {
                if (!condition.proceed) {
                    return false;
                }
            }

            return true;
        }
    }

}
