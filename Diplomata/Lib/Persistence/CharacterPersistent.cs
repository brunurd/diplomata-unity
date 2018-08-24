using System;

namespace Diplomata.Persistence
{
    [Serializable]
    public class CharacterPersistent
    {
        public string name;
        public byte influence;
        public ContextPersistent[] contexts;
    }
}