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
    public static T[] GetArrayData<T>(Data[] array)
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
    public static T[] SetArrayData<T>(Data[] dataArray, Persistent[] array) where T : Data
    {
      var data = new T[array.Length];
      for (var i = 0; i < data.Length; i++)
      {
        if (dataArray != null)
        {
          if (i < dataArray.Length)
          {
            dataArray[i].SetData(array[i]);
            data[i] = (T) Convert.ChangeType(dataArray[i], typeof(T));
            continue;
          }
        }
        data[i] = Activator.CreateInstance<T>();
        data[i].SetData(array[i]);
      }

      return data;
    }
  }
}
