namespace DiplomataLib {
    
    [System.Serializable]
    public class Condition {
        
        public Type type;
        public int comparedInfluence;
        public string characterInfluencedName;
        public AfterOf afterOf;
        public Flag customFlag;
        public int itemId;
        public string interactWith;

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
            HasItem,
            ItemWasDiscarded,
            CustomFlagEqualTo,
            ItemIsEquipped,
            InteractsWith,
            DoesNotHaveTheItem
        }

        [System.Serializable]
        public struct AfterOf {
            public string uniqueId;

            public AfterOf(string uniqueId) {
                this.uniqueId = uniqueId;
            }

            public Message GetMessage(Context context) {
                foreach (Column col in context.columns) {
                    if (Message.Find(col.messages, uniqueId) != null) {
                        return Message.Find(col.messages, uniqueId);
                    }
                }

                return null;
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
                    return "Influence <i>equal</i> to <i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>";
                case Type.InfluenceGreaterThan:
                    return "Influence <i>greater</i> then <i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>";
                case Type.InfluenceLessThan:
                    return "Influence <i>less</i> then <i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>";
                default:
                    return string.Empty;
            }
        }

        public string DisplayHasItem(string itemName) {
            return "Has the item: <i>" + itemName + "</i>";
        }

        public string DisplayDoesNotHaveItem(string itemName) {
            return "Does not have the item: <i>" + itemName + "</i>";
        }

        public string DisplayItemWasDiscarded(string itemName) {
            return "item was discarded: <i>" + itemName + "</i>";
        }

        public string DisplayItemIsEquipped(string itemName) {
            return "item is equipped: <i>" + itemName + "</i>";
        }

        public string DisplayCustomFlagEqualTo() {
            return "<i>\"" + customFlag.name + "\"</i> is <i>" + customFlag.value + "</i>";
        }

        public string DisplayInteractsWith(string objectName) {
            return "Interacts with <i>\"" + objectName + "\"</i>";
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
