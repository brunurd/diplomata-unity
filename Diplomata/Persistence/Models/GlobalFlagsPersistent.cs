using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class GlobalFlagsPersistent : Persistent
  {
    public FlagPersistent[] flags;
  }
}
