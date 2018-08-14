using System;

namespace Diplomata.Preferences
{
  [Serializable]
  public class Options
  {
    public Language[] languages = new Language[] { new Language("English") };
    public string[] languagesList = new string[] { "English" };
    public string[] characterList = new string[0];
    public string[] attributes = new string[] { "fear", "politeness", "argumentation", "insistence", "charm", "confidence" };
    public string currentLanguage = "English";
    public string playerCharacterName;
    public bool jsonPrettyPrint;

    public void SetCurrentLanguage(string language)
    {
      currentLanguage = language;

      SetLanguageList();
    }

    public void SetLanguageList()
    {
      languagesList = new string[languages.Length];

      for (int i = 0; i < languages.Length; i++)
      {
        languagesList[i] = languages[i].name;
      }
    }
  }
}
