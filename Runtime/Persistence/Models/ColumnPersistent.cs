using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class ColumnPersistent : Persistent
  {
    public string id;
    public MessagePersistent[] messages;
  }
}
