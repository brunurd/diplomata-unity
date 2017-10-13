using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    public class Effect {
        
        public Type type;
        public EndOfContext endOfContext;
        public GoTo goTo;
        public AnimatorAttributeSetter animatorAttributeSetter = new AnimatorAttributeSetter();
        public Flag customFlag;
        public int itemId;

        [System.NonSerialized]
        public Events onStart = new Events();

        [System.NonSerialized]
        public Events onComplete = new Events();

        public enum Type {
            None,
            EndOfContext,
            GoTo,
            SetAnimatorAttribute,
            GetItem,
            DiscardItem,
            SetCustomFlag,
            EquipItem
        }

        [System.Serializable]
        public struct GoTo {
            public string uniqueId;

            public GoTo(string uniqueId) {
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

        public Effect(string characterName) {
            endOfContext.characterName = characterName;
        }

        public string DisplayNone() {
            return "None";
        }

        public string DisplayEndOfContext(string contextName) {
            return "End of the context <i>" + contextName + "</i>";
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

        public string DisplayGetItem(string itemName) {
            return "Get the item: <i>" + itemName + "</i>";
        }

        public string DisplayDiscardItem(string itemName) {
            return "Discard the item: <i>" + itemName + "</i>";
        }

        public string DisplayEquipItem(string itemName) {
            return "Equip the item: <i>" + itemName + "</i>";
        }

        public string DisplayCustomFlagEqualTo() {
            return "<i>\"" + customFlag.name + "\"</i> set to <i>" + customFlag.value + "</i>";
        }
    }

}
