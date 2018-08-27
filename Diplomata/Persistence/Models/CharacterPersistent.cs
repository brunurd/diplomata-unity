using System;

namespace Diplomata.Persistence.Models
{
  [Serializable]
  public class CharacterPersistent
  {
    public string name;
    public byte influence;
    public ContextPersistent[] contexts;
  }
}
