using System.Collections.Generic;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEngine;

namespace LavaLeak.Diplomata
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
    }
  }
}
