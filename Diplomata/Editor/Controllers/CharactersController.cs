using System.Collections.Generic;
using Diplomata.Editor.Helpers;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata.Editor.Controllers
{
  public static class CharactersController
  {
    public static List<Character> GetCharacters(Options options)
    {
      JSONHelper.CreateFolder("Diplomata/Characters/");
      return UpdateList(options);
    }

    private static List<Character> UpdateList(Options options)
    {
      var charactersFiles = Resources.LoadAll("Diplomata/Characters/");
      var characters = new List<Character>();
      options.characterList = new string[0];

      foreach (Object obj in charactersFiles)
      {
        var json = (TextAsset) obj;
        var character = JsonUtility.FromJson<Character>(json.text);

        characters.Add(character);
        options.characterList = ArrayHelper.Add(options.characterList, obj.name);
      }

      return characters;
    }

    public static void Save(Character character, bool prettyPrint = false)
    {
      JSONHelper.Update(character, character.name, prettyPrint, "Diplomata/Characters/");
    }
  }
}
