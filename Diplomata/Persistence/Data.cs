using System;
using System.Collections.Generic;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;

namespace LavaLeak.Diplomata.Persistence
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
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    abstract public void SetData(Persistent persistentData);

    /// <summary>
    /// Return a array of persistent objects from a data object.
    /// </summary>
    /// <param name="array">The array of data objects.</param>
    /// <returns>A array of persistent objects.</returns>
    internal static T[] GetArrayData<T>(Data[] array)
    {
      var persistent = new T[0];
      foreach (var item in array)
      {
        persistent = ArrayHelper.Add(persistent, (T) Convert.ChangeType(item.GetData(), typeof(T)));
      }
      return persistent;
    }

    /// <summary>
    /// Set in a array of objects the data of a array of persistent data objects.
    /// </summary>
    /// <param name="dataArray">A array of data objects.</param>
    /// <param name="array">The array of persistent data objects.</param>
    internal static T[] SetArrayData<T>(Data[] dataArray, Persistent[] array)
    {
      var data = new T[0];
      foreach (var item in array)
      {
        foreach (var dataItem in dataArray)
        {
          dataItem.SetData(item);
          data = ArrayHelper.Add(data, (T) Convert.ChangeType(dataItem, typeof(T)));
        }
      }
      return data;
    }
  }
}
