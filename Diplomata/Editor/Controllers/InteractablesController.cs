using System.Collections.Generic;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Controllers
{
  public static class InteractablesController
  {
    public static List<Interactable> GetInteractables(Options options)
    {
      JSONHelper.CreateFolder("Diplomata/Interactables/");
      return UpdateList(options);
    }

    public static List<Interactable> UpdateList(Options options)
    {
      var interactablesFiles = Resources.LoadAll("Diplomata/Interactables/");
      var interactables = new List<Interactable>();
      options.interactableList = new string[0];

      foreach (Object obj in interactablesFiles)
      {
        var json = (TextAsset) obj;
        var interactable = JsonUtility.FromJson<Interactable>(json.text);

        interactables.Add(interactable);
        options.interactableList = ArrayHelper.Add(options.interactableList, obj.name);
      }

      return interactables;
    }

    public static void Save(Interactable interactable, bool prettyPrint = false)
    {
      JSONHelper.Update(interactable, interactable.name, prettyPrint, "Diplomata/Interactables/");
    }

    public static void AddInteractable(string name, Options options, List<Interactable> interactables)
    {
      Interactable interactable = new Interactable(name);
      CheckRepeatedInteractable(interactable, options, interactables);
    }

    private static void CheckRepeatedInteractable(Interactable interactable, Options options, List<Interactable> interactables)
    {
      bool canAdd = true;

      foreach (string interactableName in options.interactableList)
      {
        if (interactableName == interactable.name)
        {
          canAdd = false;
          break;
        }
      }

      if (canAdd)
      {
        interactables.Add(interactable);

        options.interactableList = ArrayHelper.Add(options.interactableList, interactable.name);
        OptionsController.Save(options, options.jsonPrettyPrint);

        JSONHelper.Create(interactable, interactable.name, options.jsonPrettyPrint, "Diplomata/Interactables/");
      }

      else
      {
        Debug.LogError("This name already exists!");
      }
    }
  }
}
