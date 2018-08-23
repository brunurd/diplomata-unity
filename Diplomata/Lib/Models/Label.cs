using System;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Label
  {
    public string id;
    public string name = String.Empty;
    public Color color;
    public bool show;

    public Label()
    {
      id = Guid.NewGuid().ToString();
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
      var label = (Label) Helpers.Find.In(array).Where("id", value).Result;
      if (label == null) label = (Label) Helpers.Find.In(array).Where("name", value).Result;
      return label;
    }
  }
}
