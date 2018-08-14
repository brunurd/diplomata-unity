using System;

namespace Diplomata.Models
{
  [Serializable]
  public class Language
  {
    public string name;
    public bool subtitle;
    public bool dubbing;

    public Language(string name)
    {
      this.name = name;
      subtitle = true;
      dubbing = true;
    }

    public Language(Language other)
    {
      name = other.name;
      subtitle = other.subtitle;
      dubbing = other.dubbing;
    }
  }
}
