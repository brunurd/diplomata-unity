using System;
using System.Collections.Generic;
using LavaLeak.Diplomata.Helpers;

namespace LavaLeak.Diplomata.Models.Submodels
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
      var character = Character.Find(characters, talkableName);
      if (character != null)
      {
        var context = Context.Find(character, contextId);
        if (context != null)
          return context;
      }

      var interactable = Interactable.Find(interactables, talkableName);
      if (interactable != null)
      {
        var context = Context.Find(interactable, contextId);
        if (context != null)
          return context;
      }

      return null;
    }
  }
}
