using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Diplomata.Models.Submodels
{
  [Serializable]
  public class Label
  {
    [FormerlySerializedAs("id")]
    public string uniqueId;
    public string name = String.Empty;
    public Color color;
    public bool show;

    public Label()
    {
      uniqueId = Guid.NewGuid().ToString();
      name = "Label name";
      show = true;
      color = new Color(1.0f, 1.0f, 1.0f);
    }

    /// <summary>
    /// Find a label by unique id or name.
    /// </summary>
    /// <param name="array">A array of labels.</param>
    /// <param name="value">The id (a string guid) of the label or a name.</param>
    /// <returns>The label if found, or null.</returns>
    public static Label Find(Label[] array, string value)
    {
      var label = (Label) Helpers.Find.In(array).Where("uniqueId", value).Result;
      if (label == null) label = (Label) Helpers.Find.In(array).Where("name", value).Result;
      return label;
    }
  }
}
