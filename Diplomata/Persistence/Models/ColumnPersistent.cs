using System;

namespace Diplomata.Persistence.Models
{
  [Serializable]
  public class ColumnPersistent : Persistent
  {
    public string id;
    public MessagePersistent[] messages;
  }
}
