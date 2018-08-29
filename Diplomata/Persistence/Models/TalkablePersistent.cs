using System;

namespace Diplomata.Persistence.Models
{
  [Serializable]
  public class TalkablePersistent : Persistent
  {
    public string id;
    public ContextPersistent[] contexts;
  }
}
