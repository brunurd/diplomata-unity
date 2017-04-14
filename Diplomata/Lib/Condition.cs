using UnityEngine.Events;

namespace DiplomataLib {

    [System.Serializable]
    public class Condition {

        public string displayName;

        public enum Type {
            AfterOf,
            Custom,
            InfluenceEqualTo,
            InfluenceGreaterThan,
            InfluenceLessThan,
            None
        }

        public Type type;

        public string afterOfMessageName;
        public int comparedInfluence;
        public string characterInfluencedName;
        public UnityAction custom;

        public Condition() {
            type = Type.None;
            ApplyNone();
        }

        public void ApplyNone() {
            displayName = "None";
        }

        public void ApplyAfterOf(string messageName) {
            afterOfMessageName = messageName;
            displayName = "After of " + afterOfMessageName;
        }

        public void ApplyCompareInfluence(string characterName, int influence) {
            comparedInfluence = influence;
            characterInfluencedName = characterName;

            switch (type) {
                case Type.InfluenceEqualTo:
                    displayName = "Influence equal to " + comparedInfluence + " in " + characterInfluencedName;
                    break;
                case Type.InfluenceGreaterThan:
                    displayName = "Influence greater then " + comparedInfluence + " in " + characterInfluencedName;
                    break;
                case Type.InfluenceLessThan:
                    displayName = "Influence less then " + comparedInfluence + " in " + characterInfluencedName;
                    break;
            }
        }

        public void ApplyCustom(UnityAction function) {
            custom = function;
            displayName = custom.Method.Name;
        }
    }

}
