namespace DiplomataLib
{
  [Serializable]
  public class Options
  {
    public string currentSubtitledLanguage = string.Empty;
    public string currentDubbedLanguage = string.Empty;
    public float volumeScale = 1.0f;

    public Options()
    {
      foreach (Language lang in Diplomata.preferences.languages)
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