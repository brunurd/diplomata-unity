using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class GlobalFlagsListMenu : EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    private string[] booleanArray = new string[] { "True", "False" };

    [MenuItem("Diplomata/Global Flags", false, 0)]
    static public void Init()
    {
      GlobalFlagsListMenu window = (GlobalFlagsListMenu) GetWindow(typeof(GlobalFlagsListMenu), false, "Custom Flags");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 150, 300);
      window.Show();
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (Controller.Instance.GlobalFlags.flags.Length <= 0)
      {
        EditorGUILayout.HelpBox("No flags yet.", MessageType.Info);
      }

      var width = Screen.width - (2 * GUIHelper.MARGIN);

      for (var i = 0; i < Controller.Instance.GlobalFlags.flags.Length; i++)
      {
        var flag = Controller.Instance.GlobalFlags.flags[i];

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        GUIHelper.labelStyle.normal.textColor = GUIHelper.grey;
        GUIHelper.labelStyle.alignment = TextAnchor.MiddleRight;
        GUILayout.Label("flag : " + i, GUIHelper.labelStyle);
        GUIHelper.labelStyle.normal.textColor = Color.black;

        GUILayout.EndHorizontal();

        GUILayout.Space(5.0f);

        GUILayout.BeginHorizontal();

        GUIHelper.textContent.text = flag.name;

        GUIHelper.textAreaStyle.padding = GUIHelper.padding;

        var height = GUIHelper.textAreaStyle.CalcHeight(GUIHelper.textContent, width);

        flag.name = EditorGUILayout.TextArea(flag.name, GUIHelper.textAreaStyle, GUILayout.ExpandWidth(true), GUILayout.Height(height));

        GUILayout.EndHorizontal();

        GUILayout.Space(5.0f);

        GUILayout.BeginHorizontal();

        string selected = flag.value.ToString();

        EditorGUI.BeginChangeCheck();

        selected = GUIHelper.Popup("Start in ", selected, booleanArray);

        if (EditorGUI.EndChangeCheck())
        {

          if (selected == "True")
          {
            flag.value = true;
          }
          else
          {
            flag.value = false;
          }

          GlobalFlagsController.Save(Controller.Instance.GlobalFlags, Controller.Instance.Options.jsonPrettyPrint);
        }

        if (EditorGUIUtility.isProSkin)
        {
          GUIHelper.labelStyle.normal.textColor = Color.white;
        }

        GUILayout.Space(10.0f);

        GUILayout.Label("Move: ", GUIHelper.labelStyle);

        if (GUILayout.Button("Up", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (i > 0)
          {
            Controller.Instance.GlobalFlags.flags = ArrayHelper.Swap(Controller.Instance.GlobalFlags.flags, i, i - 1);
            GlobalFlagsController.Save(Controller.Instance.GlobalFlags, Controller.Instance.Options.jsonPrettyPrint);
          }
        }

        if (GUILayout.Button("Down", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (i < Controller.Instance.GlobalFlags.flags.Length - 1)
          {
            Controller.Instance.GlobalFlags.flags = ArrayHelper.Swap(Controller.Instance.GlobalFlags.flags, i, i + 1);
            GlobalFlagsController.Save(Controller.Instance.GlobalFlags, Controller.Instance.Options.jsonPrettyPrint);
          }
        }

        GUILayout.Space(5.0f);

        if (GUILayout.Button("Add Next", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          Controller.Instance.GlobalFlags.flags = ArrayHelper.Add(Controller.Instance.GlobalFlags.flags, new Flag("", false));

          for (int j = 1; j < (Controller.Instance.GlobalFlags.flags.Length - 1) - i; j++)
          {
            Controller.Instance.GlobalFlags.flags = ArrayHelper.Swap(Controller.Instance.GlobalFlags.flags, Controller.Instance.GlobalFlags.flags.Length - 1, i + j);
          }

          GlobalFlagsController.Save(Controller.Instance.GlobalFlags, Controller.Instance.Options.jsonPrettyPrint);
        }

        if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No"))
          {
            Controller.Instance.GlobalFlags.flags = ArrayHelper.Remove(Controller.Instance.GlobalFlags.flags, flag);
            GlobalFlagsController.Save(Controller.Instance.GlobalFlags, Controller.Instance.Options.jsonPrettyPrint);
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        if (i < Controller.Instance.GlobalFlags.flags.Length - 1)
        {
          GUIHelper.Separator();
        }
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Controller.Instance.GlobalFlags.flags = ArrayHelper.Add(Controller.Instance.GlobalFlags.flags, new Flag("", false));
        GlobalFlagsController.Save(Controller.Instance.GlobalFlags, Controller.Instance.Options.jsonPrettyPrint);
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();

      GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
    }

    public void OnDisable()
    {
      GlobalFlagsController.Save(Controller.Instance.GlobalFlags, Controller.Instance.Options.jsonPrettyPrint);
    }
  }

}
