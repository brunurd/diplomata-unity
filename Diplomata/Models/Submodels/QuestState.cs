using System;
using UnityEngine;

namespace Diplomata.Models.Submodels
{
  /// <summary>
  /// The state class
  /// </summary>
  [Serializable]
  public class QuestState
  {
    [SerializeField] private string uniqueId;
    public string Name;

    /// <summary>
    /// The state constructor.
    /// </summary>
    /// <param name="name">The name of the state.</param>
    public QuestState(string name)
    {
      uniqueId = Guid.NewGuid().ToString();
      Name = name;
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
    /// Get a list of the names of the quests states of a array.
    /// </summary>
    /// <param name="questsStates">The quests states array.</param>
    /// <returns>A array of names as strings.</returns>
    public static string[] GetNames(QuestState[] questsStates)
    {
      string[] questsReturn = questsStates == null ? new string[0] : new string[questsStates.Length];
      if (questsStates != null)
      {
        for (var i = 0; i < questsStates.Length; i++)
        {
          questsReturn[i] = questsStates[i].Name;
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
