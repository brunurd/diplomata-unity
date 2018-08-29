using System;
using Diplomata.Models;
using Diplomata.Persistence;
using UnityEngine;

namespace Diplomata.Persistence
{
  /// <summary>
  /// The abstract class to get the persistent models data.
  /// </summary>
  [Serializable]
  abstract public class Persistent
  {
    /// <summary>
    /// Override to return the object as json.
    /// </summary>
    /// <returns>A json string.</returns>
    public override string ToString()
    {
      return JsonUtility.ToJson(this);
    }
  }
}
