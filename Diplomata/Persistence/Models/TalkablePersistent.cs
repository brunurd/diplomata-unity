using System;

namespace Diplomata.Persistence.Models
{
  [Serializable]
  public class TalkablePersistent : Persistent
  {
    public string name;
    public ContextPersistent[] contexts;
  }
}
