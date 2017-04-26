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
            DisplayNone();
        }

        public void DisplayNone() {
            displayName = "None.";
        }

        public void ApplyEndOfContext(string contextName) {
            this.contextName = contextName;
            DisplayEndOfContext();
        }

        public void DisplayEndOfContext() {
            displayName = "End of context.";
        }

        public void ApplyGoTo(string nextMessage) {
            this.nextMessage = nextMessage;
            DisplayGoTo();
        }

        public void DisplayGoTo() {
            displayName = "Go to <i>" + nextMessage +"</i>.";
        }
    }

}
