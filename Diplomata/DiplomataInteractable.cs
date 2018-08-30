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
    /// Interactable getter.
    /// </summary>
    /// <value>The talkable model.</value>
    public Interactable Interactable
    {
      get
      {
        return (Interactable) talkable;
      }
      set
      {
        talkable = (Interactable) value;
      }
    }

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
        talkable = Interactable.Find(DiplomataData.interactables, talkable.name);
      }
    }
  }
}
