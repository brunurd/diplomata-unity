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
      var character = (Character[]) Find.In(characters.ToArray()).Where("name", talkableName).Results;
      if (character.Length > 0)
      {
        var context = (Context[]) Find.In(character[0].contexts).Where("id", contextId).Results;
        if (context.Length > 0)
          return context[0];
      }

      var interactable = Interactable.Find(interactables, talkableName);
      if (interactable != null)
      {
        var context = (Context[]) Find.In(interactable.contexts).Where("id", contextId).Results;
        if (context.Length > 0)
          return context[0];
      }

      return null;
    }
  }
}
