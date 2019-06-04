using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class FlagPersistent : Persistent
  {
    public string id;
    public bool value;
  }
}
