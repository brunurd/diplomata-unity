using System;
using Diplomata.Persistence;
using UnityEngine;

namespace Diplomata
{
  /// <summary>
  /// Class to return all persistent data from the DiplomataData object.
  /// </summary>
  [Serializable]
  sealed public class DiplomataPersistentData
  {
    [SerializeField] private OptionsPersistent options;

    /// <summary>
    /// The constructor, it return all the persistent data from DiplomataData into the new object.
    /// </summary>
    public DiplomataPersistentData()
    {
      options = new OptionsPersistent();
      options = (OptionsPersistent) DiplomataData.options.GetData();
    }
  }
}
