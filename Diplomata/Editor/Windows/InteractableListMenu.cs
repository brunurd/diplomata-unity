using System.Collections.Generic;
using LavaLeak.Diplomata;
using LavaLeak.Diplomata.Editor;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class InteractableListMenu : UnityEditor.EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    public Options options;
    public List<Interactable> interactables;

    [MenuItem("Diplomata/Interactables", false, 0)]
    static public void Init()
    {
      InteractableListMenu window = (InteractableListMenu) GetWindow(typeof(InteractableListMenu), false, "Interactable List");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 80, 300);
      window.Show();
    }

    public void OnEnable()
    {
      options = OptionsController.GetOptions();
      interactables = InteractablesController.GetInteractables(options);
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (options.interactableList.Length <= 0)
      {
        EditorGUILayout.HelpBox("No interactables yet.", MessageType.Info);
      }

      for (int i = 0; i < options.interactableList.Length; i++)
      {
        var name = options.interactableList[i];
        var interactable = Interactable.Find(interactables, name);

        if (interactable.SetId())
        {
          InteractablesController.Save(interactable, options.jsonPrettyPrint);
        }

        GUILayout.BeginHorizontal();
        GUILayout.BeginHorizontal();

        if (EditorGUIUtility.isProSkin)
        {
          GUIHelper.labelStyle.normal.textColor = Color.white;
        }

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
        GUILayout.Label(name, GUIHelper.labelStyle);

        GUILayout.EndHorizontal();

        GUILayout.Space(10.0f);

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 2));

        if (GUILayout.Button("Edit", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          InteractableEditor.Edit(Interactable.Find(interactables, name));
          Close();
        }

        if (GUILayout.Button("Edit Messages", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          TalkableMessagesEditor.OpenContextMenu(interactable);
          Close();
        }

        if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No"))
          {
            interactables.Remove(interactable);
            options.interactableList = ArrayHelper.Remove(options.interactableList, name);

            JSONHelper.Delete(name, "Diplomata/Interactables/");

            OptionsController.Save(options, options.jsonPrettyPrint);
            interactables = InteractablesController.GetInteractables(options);

            InteractableEditor.Reset(name);
            TalkableMessagesEditor.Reset(name);
            ContextEditor.Reset(name);
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        if (i < options.interactableList.Length - 1)
        {
          GUIHelper.Separator();
        }
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        InteractableEditor.OpenCreate();
        Close();
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void OnInspectorUpdate()
    {
      Repaint();
    }
  }
}
