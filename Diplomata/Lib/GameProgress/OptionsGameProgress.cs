using System;
using Diplomata.Models;

namespace Diplomata.GameProgess
{
  [Serializable]
  public class OptionsGameProgess
  {
    public string currentSubtitledLanguage = string.Empty;
    public string currentDubbedLanguage = string.Empty;
    public float volumeScale = 1.0f;

    public OptionsGameProgess()
    {
      foreach (Language lang in DiplomataData.options.languages)
      {
        if (lang.subtitle && currentSubtitledLanguage == string.Empty)
        {
          currentSubtitledLanguage = lang.name;
        }

        if (lang.dubbing && currentDubbedLanguage == string.Empty)
        {
          currentDubbedLanguage = lang.name;
        }
      }
    }
  }
}
