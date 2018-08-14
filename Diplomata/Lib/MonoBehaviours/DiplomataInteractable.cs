using Diplomata.Models;
using UnityEngine;

namespace Diplomata
{
  public class DiplomataInteractable : DiplomataTalkable
  {
    private void Awake()
    {
      onStart = () =>
      {
        if (talkable != null && Application.isPlaying)
        {
          talkable = (Interactable) Interactable.Find(DiplomataData.interactables, talkable.name);
        }
      };
    }
  }
}
