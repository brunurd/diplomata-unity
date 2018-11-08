using System;
using System.Collections.Generic;
using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Helpers;
using UnityEngine;

namespace LavaLeak.Diplomata.Models
{
  /// <summary>
  /// Character class, a talkable model for game characters.
  /// </summary>
  public class Character : Talkable
  {
    public AttributeDictionary[] attributes;
    public byte influence = 50;

    public Character(){}

    /// <summary>
    /// Character constructor with name.
    /// </summary>
    /// <param name="name">The character name.</param>
    /// <returns>The new Character.</returns>
    public Character(string name, Options options) : base(name, options)
    {
      SetAttributes(options);
    }

    /// <summary>
    /// Set the global attributes to the character local attributes.
    /// </summary>
    public void SetAttributes(Options options)
    {
      attributes = new AttributeDictionary[0];

      foreach (var attrName in options.attributes)
      {
        attributes = ArrayHelper.Add(attributes, new AttributeDictionary(attrName));
      }
    }

    /// <summary>
    /// Find a character by name.
    /// </summary>
    /// <param name="list">A list of characters.</param>
    /// <param name="name">The name of the character.</param>
    /// <returns>The character if found, or null.</returns>
    public static Character Find(List<Character> list, string name)
    {
      for (var i = 0; i < list.Count; i++)
      {
        if (list[i].name == name)
          return list[i];
        
        if (list[i].Id == name)
          return list[i];
      }
      
//      var character = (Character) Helpers.Find.In(list.ToArray()).Where("name", name).Result;
//
//      if (character == null)
//        character = (Character) Helpers.Find.In(list.ToArray()).Where("Id", name).Result;

      return null;//character;
    }
  }
}
