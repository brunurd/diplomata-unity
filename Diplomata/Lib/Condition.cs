using UnityEngine.Events;

namespace DiplomataLib {

    [System.Serializable]
    public class Condition {

        public string displayName = "None.";
        public Type type;
        public string afterOfMessageName = "<none>";
        public int comparedInfluence;
        public string characterInfluencedName = "<none>";

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
            displayName = "After of " + afterOfMessageName + ".";
        }

        public void ApplyCompareInfluence(string characterName, int influence) {
            comparedInfluence = influence;
            characterInfluencedName = characterName;

            DisplayCompareInfluence();
        }

        public void DisplayCompareInfluence() {
            switch (type) {
                case Type.InfluenceEqualTo:
                    displayName = "Influence equal to " + comparedInfluence + " in " + characterInfluencedName + ".";
                    break;
                case Type.InfluenceGreaterThan:
                    displayName = "Influence greater then " + comparedInfluence + " in " + characterInfluencedName + ".";
                    break;
                case Type.InfluenceLessThan:
                    displayName = "Influence less then " + comparedInfluence + " in " + characterInfluencedName + ".";
                    break;
            }
        }
    }

}
