using Diplomata;
using Diplomata.Editor;
using Diplomata.Editor.Helpers;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Windows
{
  public class ItemListMenu : UnityEditor.EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    private DiplomataEditorData diplomataEditor;

    [MenuItem("Diplomata/Inventory")]
    static public void Init()
    {
      DiplomataEditorData.Instantiate();

      ItemListMenu window = (ItemListMenu) GetWindow(typeof(ItemListMenu), false, "Inventory");
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

      if (diplomataEditor.inventory.items.Length <= 0)
      {
        EditorGUILayout.HelpBox("No items yet.", MessageType.Info);
      }

      for (int i = 0; i < diplomataEditor.inventory.items.Length; i++)
      {

        var item = diplomataEditor.inventory.items[i];

        GUILayout.BeginHorizontal();
        GUILayout.BeginHorizontal();

        var name = DictionariesHelper.ContainsKey(item.name, diplomataEditor.options.currentLanguage);

        if (EditorGUIUtility.isProSkin)
        {
          GUIHelper.labelStyle.normal.textColor = Color.white;
        }

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
        GUILayout.Label(name.value, GUIHelper.labelStyle);

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleRight;
        GUIHelper.labelStyle.normal.textColor = GUIHelper.grey;
        GUILayout.Label("id: " + item.id, GUIHelper.labelStyle);

        GUIHelper.labelStyle.normal.textColor = Color.black;

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 2));

        if (GUILayout.Button("Edit", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          ItemEditor.OpenEdit(item);
        }

        if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No"))
          {
            ItemEditor.Init(ItemEditor.State.Close);
            diplomataEditor.inventory.items = ArrayHelper.Remove(diplomataEditor.inventory.items, item);
            diplomataEditor.SaveInventory();
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        if (i < diplomataEditor.inventory.items.Length - 1)
        {
          GUIHelper.Separator();
        }
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        diplomataEditor.inventory.items = ArrayHelper.Add(diplomataEditor.inventory.items, new Item(diplomataEditor.inventory.items.Length));
        diplomataEditor.SaveInventory();
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
