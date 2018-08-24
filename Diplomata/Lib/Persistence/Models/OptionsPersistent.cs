using System;
using Diplomata.Models;

namespace Diplomata.Persistence
{
  [Serializable]
  public class OptionsPersistent : Persistent
  {
    public string currentLanguage;
    public float volumeScale;
  }
}
