using System;

namespace Diplomata.Models
{
  [Serializable]
  public class GlobalFlags
  {
    public Flag[] flags = new Flag[0];

    public Flag Find(Flag[] flagArray, string name)
    {
      foreach (Flag flag in flagArray)
      {
        if (flag.name == name)
        {
          return flag;
        }
      }

      return null;
    }
  }
}
