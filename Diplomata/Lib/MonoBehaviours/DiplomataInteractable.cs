using Diplomata.Models;
using UnityEngine;

namespace Diplomata
{

  public class DiplomataInteractable : MonoBehaviour
  {

    public string interactableLabel;

    public void SetLabel()
    {
      Character.playerInteractingWith = interactableLabel;
    }
  }

}
