using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class MessagePersistent : Persistent
  {
    public string id;
    public bool alreadySpoked;
  }
}
