using Diplomata;
using Diplomata.Editor;
using Diplomata.Editor.Helpers;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Windows
{
  public class GlobalFlagsListMenu : UnityEditor.EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    private DiplomataEditorData diplomataEditor;
    private string[] booleanArray = new string[] { "True", "False" };

    [MenuItem("Diplomata/Global Flags")]
    static public void Init()
    {
      DiplomataEditorData.Instantiate();

      GlobalFlagsListMenu window = (GlobalFlagsListMenu) GetWindow(typeof(GlobalFlagsListMenu), false, "Custom Flags");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 150, 300);

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

      if (diplomataEditor.globalFlags.flags.Length <= 0)
      {
        EditorGUILayout.HelpBox("No flags yet.", MessageType.Info);
      }

      var width = Screen.width - (2 * GUIHelper.MARGIN);

      for (int i = 0; i < diplomataEditor.globalFlags.flags.Length; i++)
      {

        var flag = diplomataEditor.globalFlags.flags[i];

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

          diplomataEditor.SaveGlobalFlags();
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
            diplomataEditor.globalFlags.flags = ArrayHelper.Swap(diplomataEditor.globalFlags.flags, i, i - 1);
            diplomataEditor.SaveGlobalFlags();
          }
        }

        if (GUILayout.Button("Down", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (i < diplomataEditor.globalFlags.flags.Length - 1)
          {
            diplomataEditor.globalFlags.flags = ArrayHelper.Swap(diplomataEditor.globalFlags.flags, i, i + 1);
            diplomataEditor.SaveGlobalFlags();
          }
        }

        GUILayout.Space(5.0f);

        if (GUILayout.Button("Add Next", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {

          diplomataEditor.globalFlags.flags = ArrayHelper.Add(diplomataEditor.globalFlags.flags, new Flag("", false));

          for (int j = 1; j < (diplomataEditor.globalFlags.flags.Length - 1) - i; j++)
          {
            diplomataEditor.globalFlags.flags = ArrayHelper.Swap(diplomataEditor.globalFlags.flags, diplomataEditor.globalFlags.flags.Length - 1, i + j);
          }

          diplomataEditor.SaveGlobalFlags();
        }

        if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No"))
          {

            diplomataEditor.globalFlags.flags = ArrayHelper.Remove(diplomataEditor.globalFlags.flags, flag);
            diplomataEditor.SaveGlobalFlags();
          }
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        if (i < diplomataEditor.globalFlags.flags.Length - 1)
        {
          GUIHelper.Separator();
        }
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        diplomataEditor.globalFlags.flags = ArrayHelper.Add(diplomataEditor.globalFlags.flags, new Flag("", false));
        diplomataEditor.SaveGlobalFlags();
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();

      GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
    }

    public void OnDisable()
    {
      diplomataEditor.SaveGlobalFlags();
    }
  }

}
