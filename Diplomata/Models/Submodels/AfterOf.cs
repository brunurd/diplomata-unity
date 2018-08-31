using System;

namespace Diplomata.Models.Submodels
{
  [Serializable]
  public struct AfterOf
  {
    public string uniqueId;

    public AfterOf(string uniqueId)
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
