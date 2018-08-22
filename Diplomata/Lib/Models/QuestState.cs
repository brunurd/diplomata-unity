using System;
using UnityEngine;

namespace Diplomata.Models
{
  /// <summary>
  /// The state class
  /// </summary>
  [Serializable]
  public class QuestState
  {
    [SerializeField] private string id;
    public string Name;

    /// <summary>
    /// The state constructor.
    /// </summary>
    /// <param name="name">The name of the state.</param>
    public QuestState(string name)
    {
      id = Guid.NewGuid().ToString();
      Name = name;
    }

    /// <summary>
    /// Method to get id because to serialize the id need to be a field not a property.
    /// </summary>
    /// <returns>The id (a guid string).</returns>
    public string GetId()
    {
      return id;
    }
  }
}
