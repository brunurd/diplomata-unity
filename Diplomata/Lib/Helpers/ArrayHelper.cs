using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Diplomata.Helpers
{
  public class ArrayHelper
  {
    public static T[] Add<T>(T[] array, T element)
    {
      if (array == null)
      {
        array = new T[0];
      }

      var tempArray = new T[array.Length];

      for (var i = 0; i < tempArray.Length; i++)
      {
        tempArray[i] = array[i];
      }

      array = new T[tempArray.Length + 1];

      for (var i = 0; i < tempArray.Length; i++)
      {
        array[i] = tempArray[i];
      }

      array[array.Length - 1] = element;

      return array;
    }

    public static T[] Remove<T>(T[] array, T element)
    {
      var returnedArray = new T[array.Length - 1];
      var unfound = true;
      var j = 0;

      for (var i = 0; i < array.Length; i++)
      {
        if (array[i].Equals(element))
        {
          unfound = false;
        }
        else
        {
          returnedArray[j] = array[i];
          j++;
        }
      }

      if (unfound)
      {
        Debug.LogWarning("Object not found in this array.");
        return array;
      }

      else
      {
        return returnedArray;
      }
    }

    public static T[] Swap<T>(T[] array, int from, int to)
    {
      if (from < array.Length && to < array.Length && from >= 0 && to >= 0)
      {
        T[] returnedArray = new T[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
          if (i != from && i != to)
          {
            returnedArray[i] = array[i];
          }

          if (i == from)
          {
            returnedArray[to] = array[from];
          }

          if (i == to)
          {
            returnedArray[from] = array[to];
          }
        }

        return returnedArray;
      }

      else
      {
        Debug.LogWarning(from + " and " + to + " are indexes out of range (0 - " + (array.Length - 1) + ").");
        return array;
      }
    }

    private static T DeepCopy<T>(T other)
    {
      using(MemoryStream ms = new MemoryStream())
      {
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(ms, other);
        ms.Position = 0;
        return (T) formatter.Deserialize(ms);
      }
    }

    public static T[] Copy<T>(T[] copyOf)
    {
      T[] array = new T[copyOf.Length];

      for (int i = 0; i < copyOf.Length; i++)
      {
        array[i] = DeepCopy(copyOf[i]);
      }

      return array;
    }

    public static int GetIndex(string[] array, string element)
    {

      for (int i = 0; i < array.Length; i++)
      {
        if (array[i] == element)
        {
          return i;
        }
      }

      Debug.LogWarning("Cannot find this index. returned: -1");
      return -1;
    }

    public static bool Contains<T>(T[] array, T element)
    {
      bool response = false;
      for (int i = 0; i < array.Length; i++)
      {
        if (array[i].Equals(element))
        {
          response = true;
          break;
        }
      }
      return response;
    }

    public static T[] Empty<T>()
    {
      T[] array = new T[0];
      return array;
    }
  }
}
