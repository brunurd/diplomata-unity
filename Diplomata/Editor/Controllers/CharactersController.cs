using System.Collections.Generic;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Controllers
{
  public static class CharactersController
  {
    public static List<Character> GetCharacters(Options options)
    {
      JSONHelper.CreateFolder("Diplomata/Characters/");
      GlobalFlagsController.GetGlobalFlags(options.jsonPrettyPrint);
      return UpdateList(options);
    }

    public static List<Character> UpdateList(Options options)
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

    public static void AddCharacter(string name, Options options, List<Character> characters)
    {
      Character character = new Character(name);
      CheckRepeatedCharacter(character, options, characters);
    }

    private static void CheckRepeatedCharacter(Character character, Options options, List<Character> characters)
    {
      bool canAdd = true;

      foreach (string characterName in options.characterList)
      {
        if (characterName == character.name)
        {
          canAdd = false;
          break;
        }
      }

      if (canAdd)
      {
        characters.Add(character);
        if (characters.Count == 1) options.playerCharacterName = character.name;

        options.characterList = ArrayHelper.Add(options.characterList, character.name);
        OptionsController.Save(options, options.jsonPrettyPrint);

        JSONHelper.Create(character, character.name, options.jsonPrettyPrint, "Diplomata/Characters/");
      }

      else
      {
        Debug.LogError("This name already exists!");
      }
    }
  }
}
