namespace DiplomataLib {

    [System.Serializable]
    public class Condition {

        public string displayName = "None";
        public Type type;
        public DictLang[] afterOfMessageName = new DictLang[0];
        public int afterOfMessageId = -1;
        public int afterOfMessageColumnId = -1;
        public int comparedInfluence;
        public string characterInfluencedName;

        [System.NonSerialized]
        public Events custom = new Events();

        [System.NonSerialized]
        public bool proceed = true;

        public enum Type {
            None,
            AfterOf,
            InfluenceEqualTo,
            InfluenceGreaterThan,
            InfluenceLessThan
        }

        public Condition() { }

        public void DisplayNone() {
            displayName = "None";
        }

        public void DisplayAfterOf() {
            displayName = "After of <i>" + afterOfMessageName[0].value + "</i>";
        }
        
        public void DisplayCompareInfluence() {
            switch (type) {
                case Type.InfluenceEqualTo:
                    displayName = "Influence <i>equal</i> to \n<i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>";
                    break;
                case Type.InfluenceGreaterThan:
                    displayName = "Influence <i>greater</i> then \n<i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>";
                    break;
                case Type.InfluenceLessThan:
                    displayName = "Influence <i>less</i> then \n<i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>";
                    break;
            }
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
