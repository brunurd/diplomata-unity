using System;

namespace LavaLeak.Diplomata.Models.Submodels
{
  [Serializable]
  public struct GoTo
  {
    public string uniqueId;

    public GoTo(string uniqueId)
    {
      this.uniqueId = uniqueId;
    }

    public Message GetMessage(Context context)
    {
      foreach (Column col in context.columns)
      {
        if (Message.Find(col.messages, uniqueId) != null)
        {
          return Message.Find(col.messages, uniqueId);
        }
      }

      return null;
    }
  }
}
