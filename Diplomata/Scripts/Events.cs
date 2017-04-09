using System;
using UnityEngine;

namespace Diplomata {

    public class Events : MonoBehaviour {

        [HideInInspector]
        [SerializeField]
        private Character character;
        [HideInInspector]
        public string lastChoice;

        public void SetCharacter(Character character) {
            this.character = character;
        }

        public void Print(string message) {
            Debug.Log(message);
        }

        public bool Compare(Func<bool> func) {
            if (this.character != null) {
                try {
                    if (func()) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                catch (Exception e) {
                    Debug.LogError(e.Message);
                    return false;
                }
            }
            else {
                return false;
            }
        }

        public void InfluenceGreaterThen(string value) {
            this.character.conditions = Compare(() => { if (this.character.influence > double.Parse(value)) return true; else return false; });
        }

        public void InfluenceLessThen(string value) {
            this.character.conditions = Compare(() => { if (this.character.influence < double.Parse(value)) return true; else return false; });
        }

        public void InfluenceEqualTo(string value) {
            this.character.conditions = Compare(() => { if (double.Parse(value) == this.character.influence) return true; else return false; });
        }

        public void AfterChoice(string title) {
            if (lastChoice == title) {
                character.conditions = true;
            }
            else {
                character.conditions = false;
            }
        }
    }

}