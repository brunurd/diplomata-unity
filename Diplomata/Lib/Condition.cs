using UnityEngine.Events;

namespace DiplomataLib {

    [System.Serializable]
    public class Condition {

        public string displayName = "None.";
        public Type type;
        public string afterOfMessageName;
        public int comparedInfluence;
        public string characterInfluencedName;

        [System.NonSerialized]
        public UnityEvent custom;

        public enum Type {
            None,
            AfterOf,
            InfluenceEqualTo,
            InfluenceGreaterThan,
            InfluenceLessThan
        }

        public Condition() { }

        public void DisplayNone() {
            displayName = "None.";
        }

        public void ApplyAfterOf(string messageName) {
            afterOfMessageName = messageName;
            DisplayAfterOf();
        }

        public void DisplayAfterOf() {
            displayName = "After of <i>" + afterOfMessageName + "</i>.";
        }

        public void ApplyCompareInfluence(string characterName, int influence) {
            comparedInfluence = influence;
            characterInfluencedName = characterName;

            DisplayCompareInfluence();
        }

        public void DisplayCompareInfluence() {
            switch (type) {
                case Type.InfluenceEqualTo:
                    displayName = "Influence <i>equal</i> to \n<i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>.";
                    break;
                case Type.InfluenceGreaterThan:
                    displayName = "Influence <i>greater</i> then \n<i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>.";
                    break;
                case Type.InfluenceLessThan:
                    displayName = "Influence <i>less</i> then \n<i>" + comparedInfluence + "</i> in <i>" + characterInfluencedName + "</i>.";
                    break;
            }
        }
    }

}
