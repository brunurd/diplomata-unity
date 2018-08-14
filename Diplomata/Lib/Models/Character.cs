using System;
using System.Collections.Generic;
using Diplomata.Preferences;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Character
  {
    public string name;
    public LanguageDictionary[] description;
    public AttributeDictionary[] attributes;
    public Context[] contexts;
    public byte influence = 50;

    public static string playerInteractingWith;

    [NonSerialized]
    public bool onScene;

    public Character() {}

    public Character(string name)
    {
      this.name = name;
      contexts = new Context[0];
      description = new LanguageDictionary[0];

      foreach (Language lang in DiplomataManager.options.languages)
      {
        description = ArrayHelper.Add(description, new LanguageDictionary(lang.name, ""));
      }

      SetAttributes();
    }

    public void SetAttributes()
    {
      attributes = new AttributeDictionary[0];

      foreach (string attrName in DiplomataManager.options.attributes)
      {
        attributes = ArrayHelper.Add(attributes, new AttributeDictionary(attrName));
      }
    }

    public static void UpdateList()
    {
      var charactersFiles = Resources.LoadAll("Diplomata/Characters/");

      DiplomataManager.characters = new List<Character>();
      DiplomataManager.options.characterList = new string[0];

      foreach (UnityEngine.Object obj in charactersFiles)
      {
        var json = (TextAsset) obj;
        var character = JsonUtility.FromJson<Character>(json.text);

        DiplomataManager.characters.Add(character);
        DiplomataManager.options.characterList = ArrayHelper.Add(DiplomataManager.options.characterList, obj.name);
      }

      SetOnScene();
    }

    public static void SetOnScene()
    {
      var charactersOnScene = UnityEngine.Object.FindObjectsOfType<DiplomataCharacter>();

      foreach (Character character in DiplomataManager.characters)
      {
        foreach (DiplomataCharacter diplomataCharacter in charactersOnScene)
        {
          if (diplomataCharacter.character != null)
          {
            if (character.name == diplomataCharacter.character.name)
            {
              character.onScene = true;
            }
          }
        }
      }
    }

    public static Character Find(List<Character> characters, string name)
    {
      foreach (Character character in characters)
      {
        if (character.name == name)
        {
          return character;
        }
      }

      return null;
    }
  }
}
