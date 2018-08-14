
using System;

namespace Diplomata.GameProgess
{
  [Serializable]
  public class ContextProgress
  {
    public uint id;
    public bool happened;
    public ColumnProgress[] columns = new ColumnProgress[0];

    public ContextProgress() {}

    public ContextProgress(int id, bool happened)
    {
      this.id = (uint) id;
      this.happened = happened;
    }
  }
}
