using System;

namespace LavaLeak.Diplomata.Persistence.Models
{
  [Serializable]
  public class OptionsPersistent : Persistent
  {
    public string currentLanguage;
    public float volumeScale;
  }
}
