using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class ItemEditor : EditorWindow
  {
    public static Item item;
    private Vector2 scrollPos = new Vector2(0, 0);
    private static State state;

    public enum State
    {
      None = 0,
      Edit = 1,
      Close = 2
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
      var name = DictionariesHelper.ContainsKey(item.name, Controller.Instance.Options.currentLanguage);

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

      var description = DictionariesHelper.ContainsKey(item.description, Controller.Instance.Options.currentLanguage);
      if (description == null)
      {
        item.description = ArrayHelper.Add(item.description, new LanguageDictionary(Controller.Instance.Options.currentLanguage, ""));
        description = DictionariesHelper.ContainsKey(item.description, Controller.Instance.Options.currentLanguage);
      }
      GUIHelper.textContent.text = description.value;
      var descriptionHeight = GUIHelper.textAreaStyle.CalcHeight(GUIHelper.textContent, Screen.width - (2 * GUIHelper.MARGIN));
      GUILayout.Label("Description: ");
      description.value = EditorGUILayout.TextArea(description.value, GUIHelper.textAreaStyle, GUILayout.Height(descriptionHeight));

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

      item.pressedImage = (Texture2D) Resources.Load(item.pressedImagePath);
      // if (item.pressedImage == null && item.pressedImagePath != string.Empty)
      // {
      //   Debug.LogWarning("Cannot find the file \"" + item.pressedImagePath + "\" in Resources folder.");
      // }

      item.disabledImage = (Texture2D) Resources.Load(item.disabledImagePath);
      // if (item.disabledImage == null && item.disabledImagePath != string.Empty)
      // {
      //   Debug.LogWarning("Cannot find the file \"" + item.disabledImagePath + "\" in Resources folder.");
      // }

      GUILayout.Label("Image: ");
      EditorGUI.BeginChangeCheck();
      item.image = (Texture2D) EditorGUILayout.ObjectField(item.image, typeof(Texture2D), false);
      if (EditorGUI.EndChangeCheck())
      {
        item.imagePath = FilenameExtract(item.image);
      }

      GUILayout.Label("Highlighted Image: ");
      EditorGUI.BeginChangeCheck();
      item.highlightImage = (Texture2D) EditorGUILayout.ObjectField(item.highlightImage, typeof(Texture2D), false);
      if (EditorGUI.EndChangeCheck())
      {
        item.highlightImagePath = FilenameExtract(item.highlightImage);
      }

      GUILayout.Label("Pressed Image: ");
      EditorGUI.BeginChangeCheck();
      item.pressedImage = (Texture2D) EditorGUILayout.ObjectField(item.pressedImage, typeof(Texture2D), false);
      if (EditorGUI.EndChangeCheck())
      {
        item.pressedImagePath = FilenameExtract(item.pressedImage);
      }

      GUILayout.Label("Disabled Image: ");
      EditorGUI.BeginChangeCheck();
      item.disabledImage = (Texture2D) EditorGUILayout.ObjectField(item.disabledImage, typeof(Texture2D), false);
      if (EditorGUI.EndChangeCheck())
      {
        item.disabledImagePath = FilenameExtract(item.disabledImage);
      }
      EditorGUILayout.Separator();

      EditorGUILayout.HelpBox("\nMake sure to store all textures in Resources folder.\n", MessageType.Info);
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
        Controller.Instance.Inventory.AddCategory(item.Category);
      }
      for (var i = 0; i < Controller.Instance.Inventory.items.Length; i++)
      {
        if (Controller.Instance.Inventory.items[i].GetId() == item.GetId())
          Controller.Instance.Inventory.items[i] = item;
      }
      InventoryController.Save(Controller.Instance.Inventory, Controller.Instance.Options.jsonPrettyPrint);
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
