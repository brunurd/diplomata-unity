using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class InteractablePersistent : TalkablePersistent {}

  [Serializable]
  public class InteractablePersistentContainer
  {
    public InteractablePersistent[] interactables;

    public InteractablePersistentContainer(InteractablePersistent[] interactables)
    {
      this.interactables = interactables;
    }
  }
}
