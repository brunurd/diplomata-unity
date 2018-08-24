using System;

namespace Diplomata.Persistence
{
  [Serializable]
  public class TalkablePersistent : Persistent
  {
    public string name;
    public ContextPersistent[] contexts;
  }
}
