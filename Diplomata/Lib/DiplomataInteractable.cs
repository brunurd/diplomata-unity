using UnityEngine;

namespace DiplomataLib {

    public class DiplomataInteractable : MonoBehaviour {

        public string interactableLabel;

        public void OnMouseDown() {
            Character.playerInteractingWith = interactableLabel;
        }
    }

}