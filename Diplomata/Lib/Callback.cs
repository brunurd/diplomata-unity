using UnityEngine.Events;

namespace DiplomataLib {

    [System.Serializable]
    public class Callback {

        public string displayName;

        public enum Type {
            Custom,
            EndOfContext,
            GoTo,
            None
        }

        public Type type;

        public string contextName;
        public string nextMessage;
        public UnityAction custom;

        public Callback() { }

        public Callback(string contextName) {
            type = Type.None;
            this.contextName = contextName;
            ApplyNone();
        }

        public void ApplyNone() {
            displayName = "None";
        }

        public void ApplyEndOfContext(string contextName) {
            this.contextName = contextName;
            displayName = "End of context";
        }

        public void ApplyCustom(UnityAction function) {
            custom = function;
            displayName = custom.Method.Name;
        }

        public void ApplyGoTo(string nextMessage) {
            this.nextMessage = nextMessage;
            displayName = "Go to " + nextMessage;
        }
    }

}
