using System;
using System.Collections.Generic;

namespace Diplomata.Models
{
  [Serializable]
  public class Interactable : Talkable
  {
    public Interactable() : base() {}

    public Interactable(string name) : base(name) {}

    public static Interactable Find(List<Interactable> interactables, string name)
    {
      foreach (Interactable interactable in interactables)
      {
        if (interactable.name == name)
        {
          return interactable;
        }
      }

      return null;
    }
  }
}
