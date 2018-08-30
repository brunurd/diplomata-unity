using System;
using System.Collections.Generic;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Persistence;
using UnityEngine;

namespace Diplomata.Models
{
  /// <summary>
  /// Character class, a talkable model for game characters.
  /// </summary>
  [Serializable]
  public class Character : Talkable
  {
    public AttributeDictionary[] attributes;
    public byte influence = 50;

    /// <summary>
    /// Character constructor with name.
    /// </summary>
    /// <param name="name">The character name.</param>
    /// <returns>The new Character.</returns>
    public Character(string name) : base(name)
    {
      SetAttributes();
    }

    /// <summary>
    /// Set the global attributes to the character local attributes.
    /// </summary>
    public void SetAttributes()
    {
      attributes = new AttributeDictionary[0];

      foreach (string attrName in DiplomataData.options.attributes)
      {
        attributes = ArrayHelper.Add(attributes, new AttributeDictionary(attrName));
      }
    }

    /// <summary>
    /// Update the list of interactables in the DiplomataData.
    /// </summary>
    public static void UpdateList()
    {
      var charactersFiles = Resources.LoadAll("Diplomata/Characters/");

      DiplomataData.characters = new List<Character>();
      DiplomataData.options.characterList = new string[0];

      foreach (UnityEngine.Object obj in charactersFiles)
      {
        var json = (TextAsset) obj;
        var character = JsonUtility.FromJson<Character>(json.text);

        DiplomataData.characters.Add(character);
        DiplomataData.options.characterList = ArrayHelper.Add(DiplomataData.options.characterList, obj.name);
      }

      SetOnScene();
    }

    /// <summary>
    /// Set if the character is on the current scene.
    /// </summary>
    public static void SetOnScene()
    {
      var charactersOnScene = UnityEngine.Object.FindObjectsOfType<DiplomataCharacter>();

      foreach (Character character in DiplomataData.characters)
      {
        foreach (DiplomataCharacter diplomataCharacter in charactersOnScene)
        {
          if (diplomataCharacter.talkable != null)
          {
            if (character.name == diplomataCharacter.talkable.name)
            {
              character.onScene = true;
            }
          }
        }
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
      return (Character) Helpers.Find.In(list.ToArray()).Where("name", name).Result;
    }
  }
}
