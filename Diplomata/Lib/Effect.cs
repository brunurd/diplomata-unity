using UnityEngine.Events;

namespace DiplomataLib {

    [System.Serializable]
    public class Effect {

        public string displayName;
        public Type type;
        public string contextName;
        public string nextMessage;

        [System.NonSerialized]
        public UnityEvent custom;

        public enum Type {
            None,
            EndOfContext,
            GoTo
        }

        public Effect() { }

        public Effect(string contextName) {
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

        public void ApplyGoTo(string nextMessage) {
            this.nextMessage = nextMessage;
            displayName = "Go to " + nextMessage;
        }
    }

}
