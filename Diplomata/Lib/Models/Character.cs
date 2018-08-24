using System;
using System.Collections.Generic;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Persistence;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Character : Talkable
  {
    public AttributeDictionary[] attributes;
    public byte influence = 50;

    public Character(string name) : base(name)
    {
      SetAttributes();
    }

    public void SetAttributes()
    {
      attributes = new AttributeDictionary[0];

      foreach (string attrName in DiplomataData.options.attributes)
      {
        attributes = ArrayHelper.Add(attributes, new AttributeDictionary(attrName));
      }
    }

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

    /// <summary>
    /// Get the object data and save to the persistent character reference.
    /// </summary>
    /// <param name="character">A character persistent object reference.</param>
    public void GetData(ref CharacterPersistent character)
    {
      character.name = name;
      character.influence = influence;

      // foreach (var context in contexts)
      // {
      //   var newContext = new ContextPersistent();
      //   context.GetData(ref newContext);
      //   character.contexts = ArrayHelper.Add(character.contexts, newContext);
      // }
    }

    /// <summary>
    /// Get the objects array data and save to the persistent character reference array.
    /// </summary>
    /// <param name="persistentCharacters">The persistent characters array.</param>
    /// <param name="characters">The characters array.</param>
    public static void GetArrayData(ref CharacterPersistent[] persistentCharacters, Character[] characters)
    {
      foreach (var character in characters)
      {
        var newCharacter = new CharacterPersistent();
        character.GetData(ref newCharacter);
        persistentCharacters = ArrayHelper.Add(persistentCharacters, newCharacter);
      }
    }
  }
}
