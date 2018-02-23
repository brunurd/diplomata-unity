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
            return "End of the context \"" + contextName + "\"";
        }

        public string DisplayGoTo(string messageTitle) {
            return "Go to \""  + messageTitle + "\"";
        }

        public string DisplaySetAnimatorAttribute() {
            if (animatorAttributeSetter != null) {

                switch (animatorAttributeSetter.type) {
                    case AnimatorControllerParameterType.Bool:
                        return "Set animator attribute " + animatorAttributeSetter.name + " to " + animatorAttributeSetter.setBool;

                    case AnimatorControllerParameterType.Float:
                        return "Set animator attribute " + animatorAttributeSetter.name + " to " + animatorAttributeSetter.setFloat;

                    case AnimatorControllerParameterType.Int:
                        return "Set animator attribute " + animatorAttributeSetter.name + " to " + animatorAttributeSetter.setInt;

                    case AnimatorControllerParameterType.Trigger:
                        return "Pull the trigger " + animatorAttributeSetter.name + " of animator";

                    default:
                        return "Animator attribute setter type not found.";
                }

            }

            else {
                return "Animator attribute setter not found.";
            }
        }

        public string DisplayGetItem(string itemName) {
            return "Get the item: \"" + itemName + "\"";
        }

        public string DisplayDiscardItem(string itemName) {
            return "Discard the item: \"" + itemName + "\"";
        }

        public string DisplayEquipItem(string itemName) {
            return "Equip the item: \"" + itemName + "\"";
        }

        public string DisplayCustomFlagEqualTo() {
            return "\"" + customFlag.name + "\" set to " + customFlag.value;
        }
    }

}
