using DiplomataEditor.Core;
using DiplomataEditor.Editors;
using DiplomataEditor.Helpers;
using Diplomata;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.ListMenu
{
  public class CharacterListMenu : EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    private DiplomataEditorManager diplomataEditor;

    [MenuItem("Diplomata/Characters")]
    static public void Init()
    {
      DiplomataEditorManager.Instantiate();

      CharacterListMenu window = (CharacterListMenu) GetWindow(typeof(CharacterListMenu), false, "Character List");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 80, 300);

      window.Show();
    }

    public void OnEnable()
    {
      diplomataEditor = (DiplomataEditorManager) AssetHelper.Read("Diplomata.asset", "Diplomata/");
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (diplomataEditor.options.characterList.Length <= 0)
      {
        EditorGUILayout.HelpBox("No characters yet.", MessageType.Info);
      }

      for (int i = 0; i < diplomataEditor.options.characterList.Length; i++)
      {
        var name = diplomataEditor.options.characterList[i];

        GUILayout.BeginHorizontal();
        GUILayout.BeginHorizontal();

        if (EditorGUIUtility.isProSkin)
        {
          GUIHelper.labelStyle.normal.textColor = Color.white;
        }

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
        GUILayout.Label(name, GUIHelper.labelStyle);

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleRight;
        if (diplomataEditor.options.playerCharacterName == name)
        {
          GUILayout.Label("<b>[Player]</b>", GUIHelper.labelStyle);
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10.0f);

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 2));

        if (GUILayout.Button("Edit", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          CharacterEditor.Edit(Character.Find(diplomataEditor.characters, name));
        }

        if (GUILayout.Button("Edit Messages", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          CharacterMessagesManager.OpenContextMenu(Character.Find(diplomataEditor.characters, name));
          Close();
        }

        if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No"))
          {
            var isPlayer = false;

            if (name == diplomataEditor.options.playerCharacterName)
            {
              isPlayer = true;
            }

            diplomataEditor.characters.Remove(Character.Find(diplomataEditor.characters, name));
            diplomataEditor.options.characterList = ArrayHelper.Remove(diplomataEditor.options.characterList, name);

            JSONHelper.Delete(name, "Diplomata/Characters/");

            if (isPlayer && diplomataEditor.options.characterList.Length > 0)
            {
              diplomataEditor.options.playerCharacterName = diplomataEditor.options.characterList[0];
            }

            diplomataEditor.SavePreferences();

            CharacterEditor.Reset(name);
            CharacterMessagesManager.Reset(name);
            ContextEditor.Reset(name);
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        if (i < diplomataEditor.options.characterList.Length - 1)
        {
          GUIHelper.Separator();
        }
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        CharacterEditor.OpenCreate();
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
