using System;

namespace Diplomata.Models
{
  public class QuestState
  {
    public string Id { get; private set; }
    public string Name;

    public QuestState(string name)
    {
      Id = Guid.NewGuid().ToString();
      Name = name;
    }
  }
}
