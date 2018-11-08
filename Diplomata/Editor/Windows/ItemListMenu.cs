using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using LavaLeak.Diplomata.Models.Collections;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class ItemListMenu : EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);

    [MenuItem("Tools/Diplomata/Edit/Inventory", false, 0)]
    static public void Init()
    {
      ItemListMenu window = (ItemListMenu) GetWindow(typeof(ItemListMenu), false, "Inventory");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 80, 300);
      window.Show();
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (Controller.Instance.Inventory.items.Length <= 0)
      {
        EditorGUILayout.HelpBox("No items yet.", MessageType.Info);
      }

      foreach (var item in Controller.Instance.Inventory.items)
      {
        if (item.SetId())
        {
          InventoryController.Save(Controller.Instance.Inventory, Controller.Instance.Options.jsonPrettyPrint);
        }

        GUILayout.BeginHorizontal();
        GUILayout.BeginHorizontal();

        var name = DictionariesHelper.ContainsKey(item.name, Controller.Instance.Options.currentLanguage);

        if (EditorGUIUtility.isProSkin)
        {
          GUIHelper.labelStyle.normal.textColor = Color.white;
        }

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
        if (name != null)
        {
          var nameLabel = name.value;

          if (!string.IsNullOrEmpty(item.Category))
          {
            nameLabel += string.Format("   [{0}]", item.Category);
          }

          GUILayout.Label(nameLabel, GUIHelper.labelStyle);
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
            Controller.Instance.Inventory.items = ArrayHelper.Remove(Controller.Instance.Inventory.items, item);
            Controller.Instance.Inventory.RemoveNotUsedCategory();
            InventoryController.Save(Controller.Instance.Inventory, Controller.Instance.Options.jsonPrettyPrint);
          }
        }

//        if (GUILayout.Button("Duplicate", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
//        {
//          Controller.Instance.Inventory.items = ArrayHelper.Add(Controller.Instance.Inventory.items, item.Copy(Controller.Instance.Inventory.GenerateId(), Controller.Instance.Options));
//          InventoryController.Save(Controller.Instance.Inventory, Controller.Instance.Options.jsonPrettyPrint);
//        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Controller.Instance.Inventory.items = ArrayHelper.Add(Controller.Instance.Inventory.items, new Item(Controller.Instance.Inventory.GenerateId(), Controller.Instance.Options));
        InventoryController.Save(Controller.Instance.Inventory, Controller.Instance.Options.jsonPrettyPrint);
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
