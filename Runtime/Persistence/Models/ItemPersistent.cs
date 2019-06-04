using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class ItemPersistent : Persistent
  {
    public string id;
    public bool have;
    public bool discarded;
  }
}
