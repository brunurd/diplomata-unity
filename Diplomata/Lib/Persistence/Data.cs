using System;
using Diplomata.Helpers;
using Diplomata.Models;

namespace Diplomata.Persistence
{
  [Serializable]
  abstract public class Data
  {
    abstract public Persistent GetData();
    abstract public Persistent[] GetArrayData(Data[] array);
    abstract public void SetData(Persistent persistentData);
    abstract public Data[] SetArrayData(Persistent[] from);
  }
}
