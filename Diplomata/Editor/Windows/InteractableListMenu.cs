using Diplomata;
using Diplomata.Helpers;
using Diplomata.Models;
using DiplomataEditor;
using DiplomataEditor.Helpers;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.Windows
{
  public class InteractableListMenu : EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    private DiplomataEditorData diplomataEditor;

    [MenuItem("Diplomata/Interactables")]
    static public void Init()
    {
      DiplomataEditorData.Instantiate();

      InteractableListMenu window = (InteractableListMenu) GetWindow(typeof(InteractableListMenu), false, "Interactable List");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 80, 300);

      window.Show();
    }

    public void OnEnable()
    {
      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (diplomataEditor.options.interactableList.Length <= 0)
      {
        EditorGUILayout.HelpBox("No interactables yet.", MessageType.Info);
      }

      for (int i = 0; i < diplomataEditor.options.interactableList.Length; i++)
      {
        var name = diplomataEditor.options.interactableList[i];

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
          InteractableEditor.Edit(Interactable.Find(diplomataEditor.interactables, name));
        }

        if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No"))
          {

            diplomataEditor.interactables.Remove(Interactable.Find(diplomataEditor.interactables, name));
            diplomataEditor.options.characterList = ArrayHelper.Remove(diplomataEditor.options.interactableList, name);

            JSONHelper.Delete(name, "Diplomata/Interactables/");

            diplomataEditor.SavePreferences();

            InteractableEditor.Reset(name);
            ContextEditor.Reset(name);
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        if (i < diplomataEditor.options.interactableList.Length - 1)
        {
          GUIHelper.Separator();
        }
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        InteractableEditor.OpenCreate();
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
