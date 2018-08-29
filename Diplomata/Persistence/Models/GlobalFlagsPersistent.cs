using System;

namespace Diplomata.Persistence.Models
{
  [Serializable]
  public class GlobalFlagsPersistent : Persistent
  {
    public FlagPersistent[] flags;
  }
}
