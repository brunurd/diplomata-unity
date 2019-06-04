using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class CharacterPersistent : TalkablePersistent
  {
    public byte influence;
  }

  [Serializable]
  public class CharacterPersistentContainer
  {
    public CharacterPersistent[] characters;

    public CharacterPersistentContainer(CharacterPersistent[] characters)
    {
      this.characters = characters;
    }
  }
}
