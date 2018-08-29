using System;

namespace Diplomata.Persistence.Models
{
    [Serializable]
    public class FlagPersistent : Persistent
    {
      public string id;
      public bool value;
    }
}