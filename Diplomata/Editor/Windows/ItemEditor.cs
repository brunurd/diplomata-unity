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
  public class ItemEditor : UnityEditor.EditorWindow
  {
    public static Item item;
    private Vector2 scrollPos = new Vector2(0, 0);
    private static State state;
    private Options options;
    private Inventory inventory;

    public enum State
    {
      None,
      Edit,
      Close
    }

    public static void Init(State state = State.None)
    {
      ItemEditor window = (ItemEditor) GetWindow(typeof(ItemEditor), false, "Item", true);
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH, 321);

      ItemEditor.state = state;

      if (state == State.Close)
      {
        window.Close();
      }

      else
      {
        window.Show();
      }
    }

    public void OnEnable()
    {
      options = OptionsController.GetOptions();
      inventory = InventoryController.GetInventory(options.jsonPrettyPrint);
    }
    public static void OpenEdit(Item item)
    {
      ItemEditor.item = item;
      Init(State.Edit);
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      switch (state)
      {
        case State.None:
          Init(State.Close);
          break;

        case State.Edit:
          DrawEditWindow();
          break;
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void DrawEditWindow()
    {
      var name = DictionariesHelper.ContainsKey(item.name, options.currentLanguage);

      GUILayout.BeginHorizontal();
      GUILayout.BeginVertical();
      GUILayout.Label("Name: ");
      name.value = EditorGUILayout.TextField(name.value);
      EditorGUILayout.Separator();
      GUILayout.EndVertical();

      GUILayout.BeginVertical();
      GUILayout.Label("Category: ");
      EditorGUI.BeginChangeCheck();
      var category = item.Category;
      category = EditorGUILayout.TextField(category);
      if (EditorGUI.EndChangeCheck())
      {
        item.Category = category;
      }
      EditorGUILayout.Separator();
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();

      var description = DictionariesHelper.ContainsKey(item.description, options.currentLanguage);
      GUIHelper.textContent.text = description.value;
      var height = GUIHelper.textAreaStyle.CalcHeight(GUIHelper.textContent, Screen.width - (2 * GUIHelper.MARGIN));
      GUILayout.Label("Description: ");
      description.value = EditorGUILayout.TextArea(description.value, GUIHelper.textAreaStyle, GUILayout.Height(height));

      EditorGUILayout.Separator();

      item.image = (Texture2D) Resources.Load(item.imagePath);
      if (item.image == null && item.imagePath != string.Empty)
      {
        Debug.LogWarning(string.Format("Cannot find the file \"{0}\" in Resources folder.", item.imagePath));
      }

      item.highlightImage = (Texture2D) Resources.Load(item.highlightImagePath);
      if (item.highlightImage == null && item.highlightImagePath != string.Empty)
      {
        Debug.LogWarning("Cannot find the file \"" + item.highlightImagePath + "\" in Resources folder.");
      }

      GUILayout.Label("Image: ");
      EditorGUI.BeginChangeCheck();
      item.image = (Texture2D) EditorGUILayout.ObjectField(item.image, typeof(Texture2D), false);
      if (EditorGUI.EndChangeCheck())
      {
        item.imagePath = FilenameExtract(item.image);
      }

      GUILayout.Label("Highlight Image: ");
      EditorGUI.BeginChangeCheck();
      item.highlightImage = (Texture2D) EditorGUILayout.ObjectField(item.highlightImage, typeof(Texture2D), false);
      if (EditorGUI.EndChangeCheck())
      {
        item.highlightImagePath = FilenameExtract(item.highlightImage);
      }
      EditorGUILayout.Separator();

      EditorGUILayout.HelpBox("\nMake sure to store this texture in Resources folder.\n", MessageType.Info);
      EditorGUILayout.Separator();

      if (GUILayout.Button("Save and Close", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Save();
        Close();
      }
    }

    public string FilenameExtract(Texture2D image)
    {
      if (image != null)
      {
        var str = AssetDatabase.GetAssetPath(image).Replace("Resources/", "¬");
        var strings = str.Split('¬');
        if (strings.Length >= 2)
        {
          str = strings[1].Replace(".png", "");
          str = str.Replace(".jpg", "");
          str = str.Replace(".jpeg", "");
          str = str.Replace(".psd", "");
          str = str.Replace(".tga", "");
          str = str.Replace(".tiff", "");
          str = str.Replace(".gif", "");
          str = str.Replace(".bmp", "");
        }
        return str;
      }

      else
      {
        return string.Empty;
      }
    }

    public void Save()
    {
      if (item != null)
      {
        inventory.AddCategory(item.Category);
      }
      for (var i = 0; i < inventory.items.Length; i++)
      {
        if (inventory.items[i].GetId() == item.GetId())
          inventory.items[i] = item;
      }
      InventoryController.Save(inventory, options.jsonPrettyPrint);
    }

    public void OnDisable()
    {
      if (state == State.Edit && item != null)
      {
        Save();
      }
    }
  }

}
