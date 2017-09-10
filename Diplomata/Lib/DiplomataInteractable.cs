using UnityEngine;

namespace DiplomataLib {

    public class DiplomataInteractable : MonoBehaviour {

        public string interactableLabel;

        public void SetLabel() {
            Character.playerInteractingWith = interactableLabel;
        }
    }

}