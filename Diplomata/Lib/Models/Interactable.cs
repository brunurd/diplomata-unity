using System;
using System.Collections.Generic;
using UnityEngine;
using Diplomata.Helpers;

namespace Diplomata.Models
{
  [Serializable]
  public class Interactable : Talkable
  {
    public Interactable() : base() {}

    public Interactable(string name) : base(name) {}
    
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

    public static void SetOnScene()
    {
      var interactablesOnScene = UnityEngine.Object.FindObjectsOfType<DiplomataInteractable>();

      foreach (Interactable interactable in DiplomataData.interactables)
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

    public static Interactable Find(List<Interactable> interactables, string name)
    {
      foreach (Interactable interactable in interactables)
      {
        if (interactable.name == name)
        {
          return interactable;
        }
      }

      return null;
    }
  }
}
