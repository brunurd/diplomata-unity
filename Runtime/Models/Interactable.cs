using System;
using System.Collections.Generic;
using LavaLeak.Diplomata.Helpers;
using UnityEngine;

namespace LavaLeak.Diplomata.Models
{
  /// <summary>
  /// Interactable class, a talkable model for objects in the environment.
  /// </summary>
  public class Interactable : Talkable
  {
    public Interactable(){}

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
      for (var i = 0; i < list.Count; i++)
      {
        if (list[i].name == name)
          return list[i];
        
        if (list[i].Id == name)
          return list[i];
      }

//      var interactable = (Interactable) Helpers.Find.In(list.ToArray()).Where("name", name).Result;
//
//      if (interactable == null)
//        interactable = (Interactable) Helpers.Find.In(list.ToArray()).Where("Id", name).Result;

      return null;//interactable;
    }
  }
}
