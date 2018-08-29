using System;

namespace Diplomata.Persistence.Models
{
  [Serializable]
  public class InventoryPersistent : Persistent
  {
    public ItemPersistent[] items;
  }
}
