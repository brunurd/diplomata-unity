using System;
using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Helpers;
using UnityEngine;

namespace LavaLeak.Diplomata.Models.Submodels
{
  /// <summary>
  /// The state class
  /// </summary>
  [Serializable]
  public class QuestState
  {
    [SerializeField] 
    private string uniqueId;

    //TODO(Celso): Need to add this to use the language system
    
    public LanguageDictionary[] ShortDescription;
    public LanguageDictionary[] LongDescription;

    /// <summary>
    /// The state constructor.
    /// </summary>
    public QuestState()
    {
      uniqueId = Guid.NewGuid().ToString();
      ShortDescription = new LanguageDictionary[0];
      LongDescription = new LanguageDictionary[0];
    }

    /// <summary>
    /// Method to get id because to serialize the id need to be a field not a property.
    /// </summary>
    /// <returns>The id (a guid string).</returns>
    public string GetId()
    {
      return uniqueId;
    }

    /// <summary>
    /// Get the short description as string the setted language.
    /// </summary>
    /// <param name="language">The language desired.</param>
    /// <returns>The string value of the short description.</returns>
    public string GetShortDescription(string language = "")
    {
      language = string.IsNullOrEmpty(language) ? DiplomataManager.Data.options.currentLanguage : language;
      var shortDescription = DictionariesHelper.ContainsKey(ShortDescription, language);
      return shortDescription != null ? shortDescription.value : string.Empty;
    }

    /// <summary>
    /// Get a list of the short description of the quests states of a array.
    /// </summary>
    /// <param name="questsStates">The quests states array.</param>
    /// <returns>A array of short description as strings.</returns>
    public static string[] GetShortDescriptions(QuestState[] questsStates, string language = "")
    {
      var questsReturn = questsStates == null ? new string[0] : new string[questsStates.Length];
      if (questsStates != null)
      {
        for (var i = 0; i < questsStates.Length; i++)
        {
          questsReturn[i] = questsStates[i].GetShortDescription(language);
        }
      }

      return questsReturn;
    }
    
    /// <summary>
    /// Get the long description as string the setted language.
    /// </summary>
    /// <param name="language">The language desired.</param>
    /// <returns>The string value of the long description.</returns>
    public string GetLongDescription(string language = "")
    {
      language = string.IsNullOrEmpty(language) ? DiplomataManager.Data.options.currentLanguage : language;
      var longDescription = DictionariesHelper.ContainsKey(LongDescription, language);
      return longDescription != null ? longDescription.value : string.Empty;
    }

    /// <summary>
    /// Get a list of the long descriptions of the quests states of a array.
    /// </summary>
    /// <param name="questsStates">The quests states array.</param>
    /// <returns>A array of long description as strings.</returns>
    public static string[] GetLongDescriptions(QuestState[] questsStates, string language = "")
    {
      var questsReturn = questsStates == null ? new string[0] : new string[questsStates.Length];
      if (questsStates != null)
      {
        for (int i = 0; i < questsStates.Length; i++)
        {
          questsReturn[i] = questsStates[i].GetLongDescription(language);
        }
      }

      return questsReturn;
    }
    
    /// <summary>
    /// Get a list of the ids of the quests states of a array.
    /// </summary>
    /// <param name="questsStates">The quests states array.</param>
    /// <returns>A array of ids as strings.</returns>
    public static string[] GetIDs(QuestState[] questsStates)
    {
      string[] questsReturn = questsStates == null ? new string[0] : new string[questsStates.Length];
      if (questsStates != null)
      {
        for (var i = 0; i < questsStates.Length; i++)
        {
          questsReturn[i] = questsStates[i].GetId();
        }
      }

      return questsReturn;
    }
  }
}
