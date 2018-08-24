using System;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Flag
  {
    [SerializeField] string uniqueId = Guid.NewGuid().ToString();
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
  }
}
