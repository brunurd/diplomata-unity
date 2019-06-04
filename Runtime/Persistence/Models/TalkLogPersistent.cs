using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class TalkLogPersistent : Persistent
  {
    public string id;
    public string[] messagesIds;
  }

  [Serializable]
  public class TalkLogPersistentContainer
  {
    public TalkLogPersistent[] talkLogs;

    public TalkLogPersistentContainer(TalkLogPersistent[] talkLogs)
    {
      this.talkLogs = talkLogs;
    }
  }
}
