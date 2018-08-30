using System;
using System.Collections.Generic;
using Diplomata.Helpers;
using UnityEngine;

namespace Diplomata.Models
{
  /// <summary>
  /// Interactable class, a talkable model for objects in the environment.
  /// </summary>
  [Serializable]
  public class Interactable : Talkable
  {
    /// <summary>
    /// Constructor with a name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Interactable(string name) : base(name) {}

    /// <summary>
    /// Update the list of interactables in the DiplomataData.
    /// </summary>
    public static void UpdateList()
    {
      var interactablesFiles = Resources.LoadAll("Diplomata/Interactables/");

      DiplomataData.interactables = new List<Interactable>();
      DiplomataData.options.interactableList = new string[0];

      foreach (UnityEngine.Object obj in interactablesFiles)
      {
        var json = (TextAsset) obj;
        var interactable = JsonUtility.FromJson<Interactable>(json.text);

        DiplomataData.interactables.Add(interactable);
        DiplomataData.options.interactableList = ArrayHelper.Add(DiplomataData.options.interactableList, obj.name);
      }

      SetOnScene();
    }

    /// <summary>
    /// Set if the interactable is on the current scene.
    /// </summary>
    public static void SetOnScene()
    {
      var interactablesOnScene = UnityEngine.Object.FindObjectsOfType<DiplomataInteractable>();

      foreach (Interactable interactable in DiplomataData.interactables)
      {
        foreach (DiplomataInteractable diplomataInteractable in interactablesOnScene)
        {
          if (diplomataInteractable.Interactable != null)
          {
            if (interactable.name == diplomataInteractable.Interactable.name)
            {
              interactable.onScene = true;
            }
          }
        }
      }
    }

    /// <summary>
    /// Find a interactable by name.
    /// </summary>
    /// <param name="list">A list of interactables.</param>
    /// <param name="name">The name of the interactable.</param>
    /// <returns>The interactable if found, or null.</returns>
    public static Interactable Find(List<Interactable> list, string name)
    {
      return (Interactable) Helpers.Find.In(list.ToArray()).Where("name", name).Result;
    }
  }
}
