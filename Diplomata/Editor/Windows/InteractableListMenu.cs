using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class InteractableListMenu : EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);

    [MenuItem("Tools/Diplomata/Edit/Interactables", false, 0)]
    static public void Init()
    {
      InteractableListMenu window = (InteractableListMenu) GetWindow(typeof(InteractableListMenu), false, "Interactable List");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 80, 300);
      window.Show();
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (Controller.Instance.Options.interactableList.Length <= 0)
      {
        EditorGUILayout.HelpBox("No interactables yet.", MessageType.Info);
      }

      for (int i = 0; i < Controller.Instance.Options.interactableList.Length; i++)
      {
        var name = Controller.Instance.Options.interactableList[i];
        var interactable = Interactable.Find(Controller.Instance.Interactables, name);

        if (interactable.SetId())
        {
          InteractablesController.Save(interactable, Controller.Instance.Options.jsonPrettyPrint);
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

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(position.width / 2));

        if (GUILayout.Button("Edit", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          InteractableEditor.Edit(Interactable.Find(Controller.Instance.Interactables, name));
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
            Controller.Instance.Interactables.Remove(interactable);
            Controller.Instance.Options.interactableList = ArrayHelper.Remove(Controller.Instance.Options.interactableList, name);

            JSONHelper.Delete(name, "Diplomata/Interactables/");

            OptionsController.Save(Controller.Instance.Options, Controller.Instance.Options.jsonPrettyPrint);
            Controller.Instance.Interactables = InteractablesController.GetInteractables(Controller.Instance.Options);

            InteractableEditor.Reset(name);
            TalkableMessagesEditor.Reset(name);
            ContextEditor.Reset(name);
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        if (i < Controller.Instance.Options.interactableList.Length - 1)
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
