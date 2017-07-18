using UnityEngine;

namespace DiplomataLib {

    [System.Serializable]
    public class AnimatorAttributeSetter {
        public AnimatorControllerParameterType type = AnimatorControllerParameterType.Float;
        public string name;
        public float setFloat;
        public int setInt;
        public bool setBool;
        public int setTrigger;
        public string animatorPath = string.Empty;

        [System.NonSerialized]
        public RuntimeAnimatorController animator;
    }

}
