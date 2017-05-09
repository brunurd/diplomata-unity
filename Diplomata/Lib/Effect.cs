using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    public class Effect {
        
        public Type type;
        public EndOfContext endOfContext;
        public GoTo goTo;
        public AnimatorAttributeSetter animatorAttributeSetter = new AnimatorAttributeSetter();

        [System.NonSerialized]
        public Events onStart = new Events();

        [System.NonSerialized]
        public Events onComplete = new Events();

        public enum Type {
            None,
            EndOfContext,
            GoTo,
            SetAnimatorAttribute,
            GetItem
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

        public string DisplaySetAnimatorAttribute() {
            if (animatorAttributeSetter != null) {

                switch (animatorAttributeSetter.type) {
                    case AnimatorControllerParameterType.Bool:
                        return "Set animator attribute <i>" + animatorAttributeSetter.name + "</i> to <i>" + animatorAttributeSetter.setBool + "</i>";

                    case AnimatorControllerParameterType.Float:
                        return "Set animator attribute <i>" + animatorAttributeSetter.name + "</i> to <i>" + animatorAttributeSetter.setFloat + "</i>";

                    case AnimatorControllerParameterType.Int:
                        return "Set animator attribute <i>" + animatorAttributeSetter.name + "</i> to <i>" + animatorAttributeSetter.setInt + "</i>";

                    case AnimatorControllerParameterType.Trigger:
                        return "Pull the trigger <i>" + animatorAttributeSetter.name + "</i> of animator";

                    default:
                        return "Animator attribute setter type not found.";
                }

            }

            else {
                return "Animator attribute setter not found.";
            }
        }
    }

}
