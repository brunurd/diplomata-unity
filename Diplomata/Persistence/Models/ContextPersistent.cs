using System;

namespace Diplomata.Persistence.Models
{
  [Serializable]
  public class ContextPersistent : Persistent
  {
    public string id;
    public bool happened;
    public ColumnPersistent[] columns;
  }
}
