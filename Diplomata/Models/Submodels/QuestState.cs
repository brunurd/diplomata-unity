using System;
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
    
    public string ShortDescription;
    public string LongDescription;

    /// <summary>
    /// The state constructor.
    /// </summary>
    /// <param name="name">The name of the state.</param>
    public QuestState(string shortDescription, string longDescription)
    {
      uniqueId = Guid.NewGuid().ToString();
      ShortDescription = shortDescription;
      LongDescription = longDescription;
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
    /// Get a list of the short description of the quests states of a array.
    /// </summary>
    /// <param name="questsStates">The quests states array.</param>
    /// <returns>A array of short description as strings.</returns>
    public static string[] GetShortDescriptions(QuestState[] questsStates)
    {
      string[] questsReturn = questsStates == null ? new string[0] : new string[questsStates.Length];
      if (questsStates != null)
      {
        for (var i = 0; i < questsStates.Length; i++)
        {
          questsReturn[i] = questsStates[i].ShortDescription;
        }
      }

      return questsReturn;
    }

    /// <summary>
    /// Get a list of the long descriptions of the quests states of a array.
    /// </summary>
    /// <param name="questsStates">The quests states array.</param>
    /// <returns>A array of long description as strings.</returns>
    public static string[] GetLongDescriptions(QuestState[] questsStates)
    {
      string[] questsReturn = questsStates == null ? new string[0] : new string[questsStates.Length];
      if (questsStates != null)
      {
        for (int i = 0; i < questsStates.Length; i++)
        {
          questsReturn[i] = questsStates[i].LongDescription;
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
