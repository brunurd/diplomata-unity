using System;
using System.Collections.Generic;

namespace Diplomata.Models
{
  [Serializable]
  public struct EndOfContext
  {
    public string talkableName;
    public int contextId;

    public EndOfContext(string talkableName, int contextId)
    {
      this.talkableName = talkableName;
      this.contextId = contextId;
    }

    public void Set(string talkableName, int contextId)
    {
      this.talkableName = talkableName;
      this.contextId = contextId;
    }

    public Context GetContext(List<Character> characters, List<Interactable> interactables)
    {
      if (Context.Find(Character.Find(characters, talkableName), contextId) != null)
        return Context.Find(Character.Find(characters, talkableName), contextId);
      if (Context.Find(Interactable.Find(interactables, talkableName), contextId) != null)
        return Context.Find(Interactable.Find(interactables, talkableName), contextId);
      return null;
    }
  }
}
