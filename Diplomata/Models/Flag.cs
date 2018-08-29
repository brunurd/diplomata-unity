using System;
using Diplomata.Persistence;
using Diplomata.Persistence.Models;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Flag : Data
  {
    [SerializeField] private string uniqueId = Guid.NewGuid().ToString();
    public string name;
    public bool value;

    public Flag() {}

    public Flag(string name, bool value)
    {
      this.name = name;
      this.value = value;
    }

    public void Set(string name, bool value)
    {
      this.name = name;
      this.value = value;
    }

    public override Persistent GetData()
    {
      var flag = new FlagPersistent();
      flag.id = uniqueId;
      flag.value = value;
      return flag;
    }

    public override void SetData(Persistent persistentData)
    {
      var flagPersistentData = (FlagPersistent) persistentData;
      uniqueId = flagPersistentData.id;
      value = flagPersistentData.value;
    }
  }
}
