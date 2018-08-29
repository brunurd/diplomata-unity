using System;
using Diplomata.Persistence;
using Diplomata.Persistence.Models;

namespace Diplomata.Models
{
  [Serializable]
  public class GlobalFlags : Data
  {
    public Flag[] flags = new Flag[0];

    /// <summary>
    /// Find a flag by name.
    /// </summary>
    /// <param name="name">The name of the flag.</param>
    /// <returns>The flag if found, or null.</returns>
    public Flag Find(string name)
    {
      return (Flag) Helpers.Find.In(flags).Where("name", name).Result;
    }

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

    /// <summary>
    /// Return the data of the object to save in a persistent object.
    /// </summary>
    /// <returns>A persistent object.</returns>
    public override Persistent GetData()
    {
      var globalFlags = new GlobalFlagsPersistent();
      globalFlags.flags = Data.GetArrayData<FlagPersistent>(flags);
      return globalFlags;
    }

    /// <summary>
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    public override void SetData(Persistent persistentData)
    {
      var globalFlagsPersistent = (GlobalFlagsPersistent) persistentData;
      flags = Data.SetArrayData<Flag>(flags, globalFlagsPersistent.flags);
    }
  }
}
