using System;
using System.Collections.Generic;
using Diplomata.Helpers;

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
      var character = (Character) Find.In(characters.ToArray()).Where("name", talkableName).Results[0];

      if (Context.Find(character, contextId) != null) return Context.Find(character, contextId);

      if (Context.Find(Interactable.Find(interactables, talkableName), contextId) != null)
        return Context.Find(Interactable.Find(interactables, talkableName), contextId);
      return null;
    }
  }
}
