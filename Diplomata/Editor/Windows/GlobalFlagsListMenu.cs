using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using LavaLeak.Diplomata.Models.Collections;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class GlobalFlagsListMenu : UnityEditor.EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    private string[] booleanArray = new string[] { "True", "False" };
    private GlobalFlags globalFlags;
    private Options options;

    [MenuItem("Diplomata/Global Flags", false, 0)]
    static public void Init()
    {
      GlobalFlagsListMenu window = (GlobalFlagsListMenu) GetWindow(typeof(GlobalFlagsListMenu), false, "Custom Flags");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 150, 300);
      window.Show();
    }

    public void OnEnable()
    {
      options = OptionsController.GetOptions();
      globalFlags = GlobalFlagsController.GetGlobalFlags(options.jsonPrettyPrint);
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (globalFlags.flags.Length <= 0)
      {
        EditorGUILayout.HelpBox("No flags yet.", MessageType.Info);
      }

      var width = Screen.width - (2 * GUIHelper.MARGIN);

      for (var i = 0; i < globalFlags.flags.Length; i++)
      {
        var flag = globalFlags.flags[i];

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

          GlobalFlagsController.Save(globalFlags, options.jsonPrettyPrint);
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
            globalFlags.flags = ArrayHelper.Swap(globalFlags.flags, i, i - 1);
            GlobalFlagsController.Save(globalFlags, options.jsonPrettyPrint);
          }
        }

        if (GUILayout.Button("Down", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (i < globalFlags.flags.Length - 1)
          {
            globalFlags.flags = ArrayHelper.Swap(globalFlags.flags, i, i + 1);
            GlobalFlagsController.Save(globalFlags, options.jsonPrettyPrint);
          }
        }

        GUILayout.Space(5.0f);

        if (GUILayout.Button("Add Next", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          globalFlags.flags = ArrayHelper.Add(globalFlags.flags, new Flag("", false));

          for (int j = 1; j < (globalFlags.flags.Length - 1) - i; j++)
          {
            globalFlags.flags = ArrayHelper.Swap(globalFlags.flags, globalFlags.flags.Length - 1, i + j);
          }

          GlobalFlagsController.Save(globalFlags, options.jsonPrettyPrint);
        }

        if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No"))
          {
            globalFlags.flags = ArrayHelper.Remove(globalFlags.flags, flag);
            GlobalFlagsController.Save(globalFlags, options.jsonPrettyPrint);
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        if (i < globalFlags.flags.Length - 1)
        {
          GUIHelper.Separator();
        }
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        globalFlags.flags = ArrayHelper.Add(globalFlags.flags, new Flag("", false));
        GlobalFlagsController.Save(globalFlags, options.jsonPrettyPrint);
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();

      GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
    }

    public void OnDisable()
    {
      GlobalFlagsController.Save(globalFlags, options.jsonPrettyPrint);
    }
  }

}
