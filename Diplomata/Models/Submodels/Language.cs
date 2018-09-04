using System;
using UnityEngine;

namespace LavaLeak.Diplomata.Models.Submodels
{
  [Serializable]
  public class Language
  {
    // [SerializeField] private string uniqueId = Guid.NewGuid().ToString();
    public string name;
    public bool subtitle;
    public bool dubbing;

    public Language(string name)
    {
      // uniqueId = Guid.NewGuid().ToString();
      this.name = name;
      subtitle = true;
      dubbing = true;
    }

    public Language(Language other)
    {
      // uniqueId = Guid.NewGuid().ToString();
      name = other.name;
      subtitle = other.subtitle;
      dubbing = other.dubbing;
    }
  }
}
