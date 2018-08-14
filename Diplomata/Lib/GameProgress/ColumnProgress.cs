using System;

namespace Diplomata.GameProgess
{
  [Serializable]
  public class ColumnProgress
  {
    public uint id;
    public MessageProgress[] messages = new MessageProgress[0];

    public ColumnProgress() {}

    public ColumnProgress(int id)
    {
      this.id = (uint) id;
    }
  }
}