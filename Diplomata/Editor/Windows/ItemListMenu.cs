using LavaLeak.Diplomata;
using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Editor;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using LavaLeak.Diplomata.Models.Collections;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class ItemListMenu : UnityEditor.EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    public Options options;
    public Inventory inventory;

    [MenuItem("Diplomata/Inventory", false, 0)]
    static public void Init()
    {
      ItemListMenu window = (ItemListMenu) GetWindow(typeof(ItemListMenu), false, "Inventory");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 80, 300);
      window.Show();
    }

    public void OnEnable()
    {
      options = OptionsController.GetOptions();
      inventory = InventoryController.GetInventory(options.jsonPrettyPrint);
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (inventory.items.Length <= 0)
      {
        EditorGUILayout.HelpBox("No items yet.", MessageType.Info);
      }

      foreach (var item in inventory.items)
      {
        if (item.SetId())
        {
          InventoryController.Save(inventory, options.jsonPrettyPrint);
        }

        GUILayout.BeginHorizontal();
        GUILayout.BeginHorizontal();

        var name = DictionariesHelper.ContainsKey(item.name, options.currentLanguage);

        if (EditorGUIUtility.isProSkin)
        {
          GUIHelper.labelStyle.normal.textColor = Color.white;
        }

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
        if (name != null)
        {
          GUILayout.Label(name.value, GUIHelper.labelStyle);
        }
        else
        {
          GUILayout.Label("(!) Name not found (!)", GUIHelper.labelStyle);
        }

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
            inventory.items = ArrayHelper.Remove(inventory.items, item);
            inventory.RemoveNotUsedCategory();
            InventoryController.Save(inventory, options.jsonPrettyPrint);
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        inventory.items = ArrayHelper.Add(inventory.items, new Item(inventory.items.Length, options));
        InventoryController.Save(inventory, options.jsonPrettyPrint);
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
