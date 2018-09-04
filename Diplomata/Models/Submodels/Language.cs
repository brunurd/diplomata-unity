using System;
using LavaLeak.Diplomata.Helpers;
using UnityEngine;

namespace LavaLeak.Diplomata.Models.Submodels
{
  /// <summary>
  /// The language class to use in Diplomata localization.
  /// </summary>
  [Serializable]
  public class Language
  {
    [SerializeField]
    private string uniqueId = Guid.NewGuid().ToString();

    public string name;
    public bool subtitle;
    public bool dubbing;

    /// <summary>
    /// Get the unique id of the Language.
    /// </summary>
    /// <returns>The unique id (a string guid).</returns>
    public string GetId()
    {
      return uniqueId;
    }

    /// <summary>
    /// Instantiate a language from this name.
    /// </summary>
    /// <param name="name">The language name.</param>
    public Language(string name)
    {
      uniqueId = Guid.NewGuid().ToString();
      this.name = name;
      subtitle = true;
      dubbing = true;
    }

    /// <summary>
    /// Instantiate a language from another language.
    /// </summary>
    /// <param name="other">The other language.</param>
    public Language(Language other)
    {
      uniqueId = Guid.NewGuid().ToString();
      name = other.name;
      subtitle = other.subtitle;
      dubbing = other.dubbing;
    }

    /// <summary>
    /// Get the names of the languages from a array of languages.
    /// </summary>
    /// <param name="languages">A array of languages.</param>
    /// <returns>A array of strings with the languages names.</returns>
    public static string[] GetNames(Language[] languages)
    {
      var names = new string[0];
      foreach (var language in languages) names = ArrayHelper.Add(names, language.name);
      return names;
    }
  }
}
