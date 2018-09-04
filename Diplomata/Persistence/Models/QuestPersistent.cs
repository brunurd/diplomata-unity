using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class QuestPersistent : Persistent
  {
    public string id;
    public string currentStateId;
    public bool initialized;
    public bool finished;
  }

  [Serializable]
  public class QuestPersistentContainer
  {
    public QuestPersistent[] quests;

    public QuestPersistentContainer(QuestPersistent[] quests)
    {
      this.quests = quests;
    }
  }
}
