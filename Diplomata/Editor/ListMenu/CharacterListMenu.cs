using DiplomataEditor.Core;
using DiplomataEditor.Editors;
using DiplomataEditor.Helpers;
using DiplomataLib;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.ListMenu
{
  public class CharacterListMenu : EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    private Core.Diplomata diplomataEditor;

    [MenuItem("Diplomata/Characters")]
    static public void Init()
    {
      Core.Diplomata.Instantiate();

      CharacterListMenu window = (CharacterListMenu) GetWindow(typeof(CharacterListMenu), false, "Character List");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 80, 300);

      window.Show();
    }

    public void OnEnable()
    {
      diplomataEditor = (Core.Diplomata) AssetHelper.Read("Diplomata.asset", "Diplomata/");
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (diplomataEditor.preferences.characterList.Length <= 0)
      {
        EditorGUILayout.HelpBox("No characters yet.", MessageType.Info);
      }

      for (int i = 0; i < diplomataEditor.preferences.characterList.Length; i++)
      {
        var name = diplomataEditor.preferences.characterList[i];

        GUILayout.BeginHorizontal();
        GUILayout.BeginHorizontal();

        if (EditorGUIUtility.isProSkin)
        {
          GUIHelper.labelStyle.normal.textColor = Color.white;
        }

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
        GUILayout.Label(name, GUIHelper.labelStyle);

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleRight;
        if (diplomataEditor.preferences.playerCharacterName == name)
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

            if (name == diplomataEditor.preferences.playerCharacterName)
            {
              isPlayer = true;
            }

            diplomataEditor.characters.Remove(Character.Find(diplomataEditor.characters, name));
            diplomataEditor.preferences.characterList = ArrayHandler.Remove(diplomataEditor.preferences.characterList, name);

            JSONHelper.Delete(name, "Diplomata/Characters/");

            if (isPlayer && diplomataEditor.preferences.characterList.Length > 0)
            {
              diplomataEditor.preferences.playerCharacterName = diplomataEditor.preferences.characterList[0];
            }

            diplomataEditor.SavePreferences();

            CharacterEditor.Reset(name);
            CharacterMessagesManager.Reset(name);
            ContextEditor.Reset(name);
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        if (i < diplomataEditor.preferences.characterList.Length - 1)
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
