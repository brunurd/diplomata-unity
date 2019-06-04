using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class TalkablePersistent : Persistent
  {
    public string id;
    public ContextPersistent[] contexts;
  }
}
