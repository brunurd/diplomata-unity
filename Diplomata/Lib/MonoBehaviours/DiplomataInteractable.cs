using System.Collections.Generic;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata
{
  public class DiplomataInteractable : DiplomataTalkable
  {
    private void Start()
    {
      choices = new List<Message>();
      controlIndexes = new Dictionary<string, int>();

      controlIndexes.Add("context", 0);
      controlIndexes.Add("column", 0);
      controlIndexes.Add("message", 0);

      if (talkable != null && Application.isPlaying)
      {
        talkable = Interactable.Find(DiplomataData.interactables, talkable.name);
      }
    }
  }
}
