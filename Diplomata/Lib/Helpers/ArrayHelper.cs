using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Diplomata.Helpers
{
  /// <summary>
  /// A helper to manipulate any Array.
  /// </summary>
  public class ArrayHelper
  {
    /// <summary>
    /// Return one array with a element added in the last position.
    /// </summary>
    /// <param name="array">The array of elements.</param>
    /// <param name="element">The element to add.</param>
    /// <typeparam name="T">The type of the array, auto setted by the parameters.</typeparam>
    /// <returns>The new array with the element.</returns>
    public static T[] Add<T>(T[] array, T element)
    {
      // TODO: Test this first statement.
      // If array is null return a not null array.
      if (array == null)
      {
        array = new T[0];
      }

      // Reset the array with a new empty position.
      var tempArray = new T[array.Length];
      for (var i = 0; i < tempArray.Length; i++)
      {
        tempArray[i] = array[i];
      }

      // TODO: Test if this loop block is necessary.
      array = new T[tempArray.Length + 1];
      for (var i = 0; i < tempArray.Length; i++)
      {
        array[i] = tempArray[i];
      }
      array[array.Length - 1] = element;

      return array;
    }

    /// <summary>
    /// Return a array without the element to remove.
    /// </summary>
    /// <param name="array">The array of elements.</param>
    /// <param name="element">The element to remove.</param>
    /// <typeparam name="T">The type of the array, auto setted by the parameters.</typeparam>
    /// <returns>The array without the element.</returns>
    public static T[] Remove<T>(T[] array, T element)
    {
      // Create a new array with one less position.
      var returnedArray = new T[array.Length - 1];

      // Flag to use if element was found or don't.
      var unfound = true;

      // Initialize to set the positions of to the new array.
      var j = 0;

      // Loop of the referenced array.
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

      // Check if the element is in the array.
      if (unfound)
      {
        Debug.LogWarning("Object not found in this array.");
        return array;
      }

      // Return the array without the element.
      else
      {
        return returnedArray;
      }
    }

    /// <summary>
    /// Return a array with the position swapped.
    /// </summary>
    /// <param name="array">The array of elements.</param>
    /// <param name="from">Original position.</param>
    /// <param name="to">Target position.</param>
    /// <typeparam name="T">The type of the array, auto setted by the parameters.</typeparam>
    /// <returns>The array copy with elements positions swapped.</returns>
    public static T[] Swap<T>(T[] array, int from, int to)
    {
      // Check if the positions are in the array range.
      if (from < array.Length && to < array.Length && from >= 0 && to >= 0)
      {
        // Create a new array with the same length of the reference.
        T[] returnedArray = new T[array.Length];

        // Make a loop with the referenced array.
        for (int i = 0; i < array.Length; i++)
        {
          if (i != from && i != to)
          {
            returnedArray[i] = array[i];
          }

          // Make the swaps.
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

    /// <summary>
    /// Make a real copy of a element, not a reference.
    /// </summary>
    /// <param name="other">The element to copy.</param>
    /// <typeparam name="T">The type of the array, auto setted by the parameters.</typeparam>
    /// <returns>The copy.</returns>
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

    /// <summary>
    /// Make a real copy of a array, not a reference.
    /// </summary>
    /// <param name="copyOf">The array to copy.</param>
    /// <typeparam name="T">The type of the array, auto setted by the parameters.</typeparam>
    /// <returns>The copy of the array.</returns>
    public static T[] Copy<T>(T[] copyOf)
    {
      // Create a array with the same length of the referenced
      T[] array = new T[copyOf.Length];

      // Do a loop to deep copy every element
      for (int i = 0; i < copyOf.Length; i++)
      {
        array[i] = DeepCopy(copyOf[i]);
      }

      return array;
    }

    /// <summary>
    /// Get the index of a element.
    /// </summary>
    /// <param name="array">The array of elements.</param>
    /// <param name="element">The element to find the index.</param>
    /// <typeparam name="T">The element and array type.</typeparam>
    /// <returns>The index.</returns>
    public static int GetIndex<T>(T[] array, T element)
    {
      // TODO: Check if the anonymous type is a problem.
      for (int i = 0; i < array.Length; i++)
      {
        if (array[i].Equals(element))
        {
          return i;
        }
      }

      Debug.LogWarning("Cannot find this index. returned: -1");
      return -1;
    }

    /// <summary>
    /// Check if the array constains a element.
    /// </summary>
    /// <param name="array">The array of elements.</param>
    /// <param name="element">The element to check if contains.</param>
    /// <typeparam name="T">The type of the array, auto setted by the parameters.</typeparam>
    /// <returns>A flag with the answer.</returns>
    public static bool Contains<T>(T[] array, T element)
    {
      bool response = false;
      if (element != null)
      {
        for (int i = 0; i < array.Length; i++)
        {
          if (array[i] != null)
          {
            if (array[i].Equals(element))
            {
              response = true;
              break;
            }
          }
        }
      }
      return response;
    }

    /// <summary>
    /// Return a empty array.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    /// <returns></returns>
    public static T[] Empty<T>()
    {
      T[] array = new T[0];
      return array;
    }

    /// <summary>
    /// Check if the element is the last element of the array.
    /// </summary>
    /// <param name="array">The array to check.</param>
    /// <param name="element">The element to check.</param>
    /// <typeparam name="T">The type of the array, auto setted by the parameters.</typeparam>
    /// <returns>Return a boolean flag, is true if is the last.</returns>
    public static bool IsLast<T>(T[] array, T element)
    {
      if (array == null || element == null) return false;
      return array[array.Length - 1].Equals(element);
    }
  }
}
