using System.Collections.Generic;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata
{
  /// <summary>
  /// Interactable game objects component class.
  /// </summary>
  public class DiplomataInteractable : DiplomataTalkable
  {
    /// <summary>
    /// Set the main talkable fields.
    /// </summary>
    private void Start()
    {
      choices = new List<Message>();
      controlIndexes = new Dictionary<string, int>();

      controlIndexes.Add("context", 0);
      controlIndexes.Add("column", 0);
      controlIndexes.Add("message", 0);

      if (talkable != null && Application.isPlaying)
      {
        talkable = Interactable.Find(Data.interactables, talkable.name);
      }
    }
  }
}
