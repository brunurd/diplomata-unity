using Diplomata;
using Diplomata.Models;
using DiplomataEditor;
using DiplomataEditor.Helpers;
using DiplomataEditor.Windows;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.Inspector
{
  /// <summary>
  /// The inspector class to DiplomataInteractable
  /// <seealso cref="DiplomataLib.DiplomataInteractable">
  /// </summary>
  [CustomEditor(typeof(DiplomataInteractable))]
  [CanEditMultipleObjects]
  public class DiplomataInteractableInspector : Editor
  {
    public DiplomataInteractable diplomataInteractable;
    private static DiplomataEditorData diplomataEditor;

    private void OnEnable()
    {
      Refresh();
    }

    private void Refresh()
    {
      DiplomataEditorData.Instantiate();
      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      diplomataInteractable = target as DiplomataInteractable;
    }

    public override void OnInspectorGUI()
    {
      GUIHelper.Init();
      serializedObject.Update();
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (diplomataInteractable.talkable != null && diplomataEditor.characters.Count > 0)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Interactable: ");

        if (!Application.isPlaying)
        {
          var selected = 0;

          for (var i = 0; i < diplomataEditor.interactables.Count; i++)
          {
            if (diplomataEditor.interactables[i].name == diplomataInteractable.talkable.name)
            {
              selected = i;
              break;
            }
          }

          var selectedBefore = selected;
          selected = EditorGUILayout.Popup(selected, diplomataEditor.options.interactableList);

          for (var i = 0; i < diplomataEditor.interactables.Count; i++)
          {
            if (selected == i)
            {
              diplomataInteractable.talkable = diplomataEditor.interactables[i];
              diplomataEditor.interactables[selectedBefore].onScene = false;
              diplomataInteractable.talkable.onScene = true;
              break;
            }
          }
        }

        else
        {
          GUILayout.Label(diplomataInteractable.talkable.name);
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Refresh", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          Refresh();
        }

        GUIHelper.Separator();

        if (GUILayout.Button("Edit Interactable", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          InteractableEditor.Edit((Interactable) diplomataInteractable.talkable);
        }

        if (GUILayout.Button("Edit Messages", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          TalkableMessagesManager.OpenContextMenu((Interactable) diplomataInteractable.talkable);
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
