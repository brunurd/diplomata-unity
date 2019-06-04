using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class InventoryPersistent : Persistent
  {
    public ItemPersistent[] items;
  }
}
