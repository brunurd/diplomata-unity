using System;
using Diplomata.Persistence;

namespace Diplomata
{
  [Serializable]
  sealed public class DiplomataPersistentData
  {
    public OptionsPersistent options;

    public DiplomataPersistentData()
    {
      options = new OptionsPersistent();
      options = (OptionsPersistent) DiplomataData.options.GetData();
    }
  }
}
