using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class ContextPersistent : Persistent
  {
    public string id;
    public bool happened;
    public ColumnPersistent[] columns;
  }
}
