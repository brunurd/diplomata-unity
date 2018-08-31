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
    /// Update the list of interactables in the DiplomataManager.Data.
    /// </summary>
    public static void UpdateList()
    {
      var interactablesFiles = Resources.LoadAll("Diplomata/Interactables/");

      DiplomataManager.Data.interactables = new List<Interactable>();
      DiplomataManager.Data.options.interactableList = new string[0];

      foreach (UnityEngine.Object obj in interactablesFiles)
      {
        var json = (TextAsset) obj;
        var interactable = JsonUtility.FromJson<Interactable>(json.text);

        DiplomataManager.Data.interactables.Add(interactable);
        DiplomataManager.Data.options.interactableList = ArrayHelper.Add(DiplomataManager.Data.options.interactableList, obj.name);
      }

      SetOnScene();
    }

    /// <summary>
    /// Set if the interactable is on the current scene.
    /// </summary>
    public static void SetOnScene()
    {
      var interactablesOnScene = UnityEngine.Object.FindObjectsOfType<DiplomataInteractable>();

      foreach (Interactable interactable in DiplomataManager.Data.interactables)
      {
        foreach (DiplomataInteractable diplomataInteractable in interactablesOnScene)
        {
          if (diplomataInteractable.talkable != null)
          {
            if (interactable.name == diplomataInteractable.talkable.name)
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
