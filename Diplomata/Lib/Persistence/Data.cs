using System;
using Diplomata.Helpers;
using Diplomata.Models;

namespace Diplomata.Persistence
{
  /// <summary>
  /// The abstract class to the main models data store.
  /// </summary>
  [Serializable]
  abstract public class Data
  {
    /// <summary>
    /// Return the data of the object to save in a persistent object.
    /// </summary>
    /// <returns>A persistent object.</returns>
    abstract public Persistent GetData();

    /// <summary>
    /// Return a array of persistent objects from a data object.
    /// </summary>
    /// <param name="array">The array of data objects.</param>
    /// <returns>A array of persistent objects.</returns>
    abstract public Persistent[] GetArrayData(Data[] array);

    /// <summary>
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    abstract public void SetData(Persistent persistentData);

    /// <summary>
    /// Set in a array of objects the data of a array of persistent data objects.
    /// </summary>
    /// <param name="data">A array of data objects.</param>
    /// <param name="persistentData">The array of persistent data objects.</param>
    abstract public void SetArrayData(ref Data[] data, Persistent[] persistentData);
  }
}
