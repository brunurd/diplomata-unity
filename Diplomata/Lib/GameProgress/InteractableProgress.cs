using System;

namespace Diplomata.GameProgress
{
  [Serializable]
  public class InteractableProgress
  {
    public string name;
    public ContextProgress[] contexts = new ContextProgress[0];

    public InteractableProgress() {}

    public InteractableProgress(string name)
    {
      this.name = name;
    }
  }
}
