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
      var character = (Character) Find.In(characters.ToArray()).Where("name", talkableName).Result;
      if (character != null)
      {
        var context = (Context) Find.In(character.contexts).Where("id", contextId).Result;
        if (context != null)
          return context;
      }

      var interactable = (Interactable) Find.In(interactables.ToArray()).Where("name", talkableName).Result;
      if (interactable != null)
      {
        var context = (Context) Find.In(interactable.contexts).Where("id", contextId).Result;
        if (context != null)
          return context;
      }

      return null;
    }
  }
}
