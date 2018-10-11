using System;
using System.Collections.Generic;
using LavaLeak.Diplomata.Helpers;
using UnityEngine;

namespace LavaLeak.Diplomata.Models
{
  /// <summary>
  /// Interactable class, a talkable model for objects in the environment.
  /// </summary>
  [Serializable]
  public class Interactable : Talkable
  {
    /// <summary>
    /// Constructor with a name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Interactable(string name, Options options) : base(name, options) {}

    /// <summary>
    /// Find a interactable by name.
    /// </summary>
    /// <param name="list">A list of interactables.</param>
    /// <param name="name">The name of the interactable.</param>
    /// <returns>The interactable if found, or null.</returns>
    public static Interactable Find(List<Interactable> list, string name)
    {
      return (Interactable) Helpers.Find.In(list.ToArray()).Where("name", name).Result;
    }
  }
}
