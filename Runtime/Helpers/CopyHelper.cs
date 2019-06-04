using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LavaLeak.Diplomata.Helpers
{
  /// <summary>
  /// Class to help copy elements.
  /// </summary>
  public static class CopyHelper
  {
    /// <summary>
    /// Make a real copy of a element, not a reference.
    /// </summary>
    /// <param name="other">The element to copy.</param>
    /// <typeparam name="T">The type of the array, auto setted by the parameters.</typeparam>
    /// <returns>The copy.</returns>
    public static T DeepCopy<T>(T other)
    {
      using(var ms = new MemoryStream())
      {
        var formatter = new BinaryFormatter();
        formatter.Serialize(ms, other);
        ms.Position = 0;
        return (T) formatter.Deserialize(ms);
      }
    }
  }
}