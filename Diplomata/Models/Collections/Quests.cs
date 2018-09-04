using System;
using LavaLeak.Diplomata.Models;
using UnityEngine;

namespace LavaLeak.Diplomata.Models.Collections
{
  /// <summary>
  /// Class to serialize as json.
  /// </summary>
  [Serializable]
  public class Quests
  {
    [SerializeField] private Quest[] quests = new Quest[0];

    /// <summary>
    /// A clean constructor.
    /// </summary>
    public Quests() {}

    /// <summary>
    /// A constructor with a quests array.
    /// Made to work with <seealso cref="UnityEngine.JsonUtility.ToJson(object)">.
    /// </summary>
    /// <param name="quests">A array of quests objects.</param>
    public Quests(Quest[] quests)
    {
      this.quests = quests;
    }

    /// <summary>
    /// Get the quest array private field.
    /// Made to work with <seealso cref="UnityEngine.JsonUtility.FromJson(string, Type)">.
    /// </summary>
    /// <returns>A array of quests.</returns>
    public Quest[] GetQuests()
    {
      return quests;
    }
  }
}
