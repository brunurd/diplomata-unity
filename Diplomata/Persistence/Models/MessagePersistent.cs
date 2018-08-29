using System;

namespace Diplomata.Persistence.Models
{
  [Serializable]
  public class MessagePersistent : Persistent
  {
    public string id;
    public bool alreadySpoked;
  }
}
