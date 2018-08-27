using System;

namespace Diplomata.Models
{
  [Serializable]
  public class GlobalFlags
  {
    public Flag[] flags = new Flag[0];

    /// <summary>
    /// Find a flag by name.
    /// </summary>
    /// <param name="array">A array of flags.</param>
    /// <param name="name">The name of the flag.</param>
    /// <returns>The flag if found, or null.</returns>
    public Flag Find(Flag[] array, string name)
    {
      return (Flag) Helpers.Find.In(array).Where("name", name).Result;
    }
  }
}
