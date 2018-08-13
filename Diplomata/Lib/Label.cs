using System;
using UnityEngine;

namespace DiplomataLib
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

    public static Label Find(Label[] array, string id)
    {
      foreach (Label label in array)
      {
        if (label.id == id)
        {
          return label;
        }
      }
      return null;
    }
  }
}
