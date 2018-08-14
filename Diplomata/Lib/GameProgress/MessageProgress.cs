using System;

namespace Diplomata.GameProgess
{
  [Serializable]
  public class MessageProgress
  {
    public uint id;
    public bool alreadySpoked;

    public MessageProgress() {}

    public MessageProgress(int id, bool alreadySpoked)
    {
      this.id = (uint) id;
      this.alreadySpoked = alreadySpoked;
    }
  }
}
