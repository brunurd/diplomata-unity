using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class InteractableEditor : EditorWindow
  {
    private Vector2 scrollPos = new Vector2(0, 0);
    private string interactableName = "";
    public static Interactable interactable;

    public enum State
    {
      None = 0,
      Create = 1,
      Edit = 2,
      Close = 3
    }

    private static State state;

    public static void Init(State state = State.None)
    {
      InteractableEditor.state = state;
      GUIHelper.focusOnStart = true;

      InteractableEditor window = (InteractableEditor) GetWindow(typeof(InteractableEditor), false, "Interactable", true);

      if (state == State.Create)
      {
        window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH, 100);
      }

      else
      {
        window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH, 390);
      }

      if (state == State.Close)
      {
        window.Close();
      }

      else
      {
        window.Show();
      }
    }

    public void OnDisable()
    {
      if (state == State.Edit && interactable != null)
      {
        Save();
      }
    }

    public static void OpenCreate()
    {
      interactable = null;
      Init(State.Create);
    }

    public static void Edit(Interactable currentInteractable)
    {
      interactable = currentInteractable;
      Init(State.Edit);
    }

    public static void Reset(string interactableName)
    {
      if (interactable != null)
      {
        if (interactable.name == interactableName)
        {
          interactable = null;
          Init(State.Close);
        }
      }
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

        case State.Create:
          DrawCreateWindow();
          break;

        case State.Edit:
          DrawEditWindow();
          break;
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void DrawCreateWindow()
    {
      GUILayout.Label("Name: ");

      GUI.SetNextControlName("name");
      interactableName = EditorGUILayout.TextField(interactableName);

      GUIHelper.Focus("name");

      EditorGUILayout.Separator();

      GUILayout.BeginHorizontal();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Create();
      }

      if (GUILayout.Button("Cancel", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Close();
      }

      GUILayout.EndHorizontal();

      if (focusedWindow != null)
      {
        if (focusedWindow.ToString() == " (Diplomata.Editor.Windows.InteractableEditor)")
        {
          if (Event.current.keyCode == KeyCode.Return)
          {
            Create();
          }
        }
      }
    }

    public void Create()
    {
      if (interactableName != "")
      {
        InteractablesController.AddInteractable(interactableName, Controller.Instance.Options, Controller.Instance.Interactables);
      }
      else
      {
        Debug.LogError("Interactable name was empty.");
      }
      Close();
    }

    public void DrawEditWindow()
    {
      GUILayout.Label(string.Format("Name: {0}", interactable.name));

      GUIHelper.Separator();

      var description = DictionariesHelper.ContainsKey(interactable.description, Controller.Instance.Options.currentLanguage);

      if (description == null)
      {
        interactable.description = ArrayHelper.Add(interactable.description, new LanguageDictionary(Controller.Instance.Options.currentLanguage, ""));
        description = DictionariesHelper.ContainsKey(interactable.description, Controller.Instance.Options.currentLanguage);
      }

      GUIHelper.textContent.text = description.value;
      var height = GUIHelper.textAreaStyle.CalcHeight(GUIHelper.textContent, Screen.width - (2 * GUIHelper.MARGIN));

      GUILayout.Label("Description: ");
      description.value = EditorGUILayout.TextArea(description.value, GUIHelper.textAreaStyle, GUILayout.Height(height));

      EditorGUILayout.Separator();

      GUILayout.BeginHorizontal();

      if (GUILayout.Button("Save", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Save();
        Close();
      }

      if (GUILayout.Button("Close", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Save();
        Close();
      }

      GUILayout.EndHorizontal();
    }

    public void Save()
    {
      InteractablesController.Save(interactable, Controller.Instance.Options.jsonPrettyPrint);
      OptionsController.Save(Controller.Instance.Options, Controller.Instance.Options.jsonPrettyPrint);
    }
  }
}
