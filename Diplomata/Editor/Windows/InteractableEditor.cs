using System;
using System.Collections.Generic;
using Diplomata.Dictionaries;
using Diplomata.Editor;
using Diplomata.Editor.Controllers;
using Diplomata.Editor.Helpers;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Windows
{
  public class InteractableEditor : UnityEditor.EditorWindow
  {
    private Vector2 scrollPos = new Vector2(0, 0);
    private string interactableName = "";
    public Options options;
    public List<Interactable> interactables;
    public static Interactable interactable;

    public enum State
    {
      None,
      Create,
      Edit,
      Close
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

    public void OnEnable()
    {
      options = OptionsController.GetOptions();
      interactables = InteractablesController.GetInteractables(options);
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
        InteractablesController.AddInteractable(interactableName, options, interactables);
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

      var description = DictionariesHelper.ContainsKey(interactable.description, options.currentLanguage);

      if (description == null)
      {
        interactable.description = ArrayHelper.Add(interactable.description, new LanguageDictionary(options.currentLanguage, ""));
        description = DictionariesHelper.ContainsKey(interactable.description, options.currentLanguage);
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
      InteractablesController.Save(interactable, options.jsonPrettyPrint);
      OptionsController.Save(options, options.jsonPrettyPrint);
    }
  }
}
