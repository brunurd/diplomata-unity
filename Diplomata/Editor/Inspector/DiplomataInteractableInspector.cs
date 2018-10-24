using System.Collections.Generic;
using LavaLeak.Diplomata;
using LavaLeak.Diplomata.Editor;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Editor.Windows;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Inspector
{
  /// <summary>
  /// The inspector class to DiplomataInteractable
  /// <seealso cref="Diplomata.DiplomataInteractable">
  /// </summary>
  [CustomEditor(typeof(DiplomataInteractable))]
  [CanEditMultipleObjects]
  public class DiplomataInteractableInspector : UnityEditor.Editor
  {
    public DiplomataInteractable diplomataInteractable;
    private Options options;
    private List<Interactable> interactables;

    private void OnEnable()
    {
      Refresh();
    }

    private void Refresh()
    {
      options = OptionsController.GetOptions();
      interactables = InteractablesController.GetInteractables(options);
      diplomataInteractable = target as DiplomataInteractable;
    }

    private Interactable GetTalkable()
    {
      return Interactable.Find(interactables, diplomataInteractable.talkableName);
    }

    public override void OnInspectorGUI()
    {
      GUIHelper.Init();
      serializedObject.Update();
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (interactables.Count > 0)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Interactable: ");

        if (!Application.isPlaying)
        {
          var selected = 0;

          for (var i = 0; i < interactables.Count; i++)
          {
            if (interactables[i].name == diplomataInteractable.talkableName)
            {
              selected = i;
              break;
            }
          }

          var selectedBefore = selected;
          selected = EditorGUILayout.Popup(selected, options.interactableList);

          for (var i = 0; i < interactables.Count; i++)
          {
            if (selected == i)
            {
              diplomataInteractable.talkableName = interactables[i].name;
              interactables[selectedBefore].onScene = false;
              GetTalkable().onScene = true;
              break;
            }
          }
        }

        else
        {
          GUILayout.Label(diplomataInteractable.talkableName);
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Refresh", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          Refresh();
        }

        GUIHelper.Separator();

        if (GUILayout.Button("Edit Interactable", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          InteractableEditor.Edit(Interactable.Find(Controller.Instance.Interactables,
            diplomataInteractable.talkableName));
        }

        if (GUILayout.Button("Edit Messages", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          TalkableMessagesEditor.OpenContextMenu(Interactable.Find(Controller.Instance.Interactables,
            diplomataInteractable.talkableName));
        }

        GUIHelper.Separator();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create Interactable", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_BIG)))
        {
          InteractableEditor.OpenCreate();
        }

        EditorGUILayout.HelpBox("Create does not interfe in this interactable.", MessageType.Info);
        GUILayout.EndHorizontal();
      }

      else
      {
        if (GUILayout.Button("Create Interactable", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          InteractableEditor.OpenCreate();
        }
      }

      GUILayout.EndVertical();
      serializedObject.ApplyModifiedProperties();
    }
  }
}