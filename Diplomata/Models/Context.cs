using System;
using System.Text.RegularExpressions;
using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models.Submodels;
using LavaLeak.Diplomata.Persistence;
using LavaLeak.Diplomata.Persistence.Models;
using UnityEngine;

namespace LavaLeak.Diplomata.Models
{
  /// <summary>
  /// The states in the editor.
  /// </summary>
  public enum MessageEditorState
  {
    None = 0,
    Normal = 1,
    Conditions = 2,
    Effects = 3
  }

  /// <summary>
  /// The context of the messages of a character.
  /// </summary>
  [Serializable]
  public class Context : Data
  {
    [SerializeField] private string uniqueId = Guid.NewGuid().ToString();

    public int id;
    public string talkableName;
    public bool idFilter = false;
    public bool conditionsFilter = true;
    public bool titleFilter = true;
    public bool contentFilter = true;
    public bool effectsFilter = true;
    public CurrentMessage currentMessage = new CurrentMessage(-1, -1);
    public MessageEditorState messageEditorState = MessageEditorState.None;
    public ushort columnWidth = 200;
    public ushort fontSize = 11;
    public Column[] columns;
    public LanguageDictionary[] name;
    public Label[] labels = new Label[] {new Label()};
    public LocalVariable[] LocalVariables;

    [NonSerialized] public Action OnEnd;
    private bool happened;

    /// <summary>
    /// Get and set if the context finished.
    /// </summary>
    /// <value>The bool happened field.</value>
    public bool Finished
    {
      get { return happened; }
      set
      {
        if (OnEnd != null)
          OnEnd();
        happened = value;
      }
    }

    /// <summary>
    /// Current message nested struct.
    /// </summary>
    public struct CurrentMessage
    {
      public int columnId;
      public int rowId;

      /// <summary>
      /// Current message constructor with column and row id.
      /// </summary>
      /// <param name="columnId">The column index id.</param>
      /// <param name="rowId">The row index id.</param>
      public CurrentMessage(int columnId, int rowId)
      {
        this.columnId = columnId;
        this.rowId = rowId;
      }

      /// <summary>
      /// Set the current fields column and row id.
      /// </summary>
      /// <param name="columnId">The column index id.</param>
      /// <param name="rowId">The row index id.</param>
      public void Set(int columnId, int rowId)
      {
        this.columnId = columnId;
        this.rowId = rowId;
      }
    }

    /// <summary>
    /// Basic constructor.
    /// </summary>
    public Context()
    {
    }

    /// <summary>
    /// Context constructor with id and the talkable name.
    /// </summary>
    /// <param name="id">A int id.</param>
    /// <param name="talkableName">The talkable parent name.</param>
    public Context(int id, string talkableName)
    {
      uniqueId = Guid.NewGuid().ToString();
      this.id = id;
      this.talkableName = talkableName;
      name = new LanguageDictionary[0];
      columns = new Column[0];
      labels = new Label[] {new Label()};
      LocalVariables = new LocalVariable[0];

      foreach (Language lang in DiplomataManager.Data.options.languages)
      {
        name = ArrayHelper.Add(name, new LanguageDictionary(lang.name, "Name [Change clicking on Edit]"));
      }
    }

    /// <summary>
    /// Add local variable to local variables of the context.
    /// </summary>
    /// <param name="variableName">The name of the variable.</param>
    /// <param name="type">The type of the variable.</param>
    /// <param name="value">The value of the variable.</param>
    public void AddLocalVariable(string variableName, VariableType type, object value)
    {
      LocalVariables = ArrayHelper.Add(LocalVariables, new LocalVariable(variableName, type, value));
    }

    /// <summary>
    /// Get a local variable from the context.
    /// </summary>
    /// <param name="variableName">The local variable name.</param>
    /// <returns>The local variable if exists or null.</returns>
    public LocalVariable GetLocalVariable(string variableName)
    {
      foreach (var localVariable in LocalVariables)
      {
        if (localVariable.Name.Equals(variableName))
          return localVariable;
      }

      return null;
    }

    /// <summary>
    /// Get only the names of all local variables.
    /// </summary>
    /// <returns>A array of strings with all the names.</returns>
    public string[] GetLocalVariablesNames()
    {
      var names = new string[0];
      foreach (var localVariable in LocalVariables)
        names = ArrayHelper.Add(names, localVariable.Name);
      return names;
    }

    /// <summary>
    /// Return a text with local variables replaced.
    /// </summary>
    /// <param name="text">The text to replace.</param>
    /// <returns>The text with all the local variables replaced.</returns>
    public string ReplaceVariables(string text)
    {
      var replacedText = string.Copy(text);
      var matches = Regex.Matches(replacedText, "{{(.*?)}}");

      foreach (var match in matches)
      {
        var varName = match.ToString().Replace("{{", "");
        varName = varName.Replace("}}", "");

        var localVariable = GetLocalVariable(varName);
        if (localVariable != null && !string.IsNullOrEmpty(varName))
        {
          switch (localVariable.Type)
          {
            case VariableType.String:
              replacedText = replacedText.Replace(match.ToString(),
                DictionariesHelper.ContainsKey(localVariable.StringValue, DiplomataManager.Data.options.currentLanguage)
                  .value);
              break;
            case VariableType.Int:
              replacedText = replacedText.Replace(match.ToString(), localVariable.IntValue.ToString());
              break;
            case VariableType.Float:
              replacedText = replacedText.Replace(match.ToString(), localVariable.FloatValue.ToString());
              break;
          }
        }
        else
        {
          replacedText = replacedText.Replace(match.ToString(), "");
        }
      }

      return replacedText;
    }

    /// <summary>
    /// Find a context by id.
    /// </summary>
    /// <param name="talkable">A talkable (Character or Interactable).</param>
    /// <param name="contextId">The id of the context.</param>
    /// <returns>The context if found, or null.</returns>
    public static Context Find(Talkable talkable, int contextId)
    {
      return (Context) Helpers.Find.In(talkable.contexts).Where("id", contextId).Result;
    }

    /// <summary>
    /// Find a context by name.
    /// </summary>
    /// <param name="talkable">The talkable to find in.</param>
    /// <param name="contextName">The context name.</param>
    /// <param name="language">The language of the name.</param>
    /// <returns>The context if found or null.</returns>
    public static Context Find(Talkable talkable, string contextName, string language)
    {
      if (talkable != null)
      {
        foreach (var context in talkable.contexts)
        {
          foreach (var name in context.name)
          {
            if (name.key == language && name.value == contextName) return context;
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Reset all contexts index id's.
    /// </summary>
    /// <param name="talkable">The talkable parent of the contexts.</param>
    /// <param name="array">The array of contexts.</param>
    /// <returns>The resetted array of contexts.</returns>
    public static Context[] ResetIDs(Talkable talkable, Context[] array)
    {
      var temp = new Context[0];

      for (int i = 0; i < array.Length + 1; i++)
      {
        var ctx = Find(talkable, i);

        if (ctx != null)
        {
          temp = ArrayHelper.Add(temp, ctx);
        }
      }

      for (int j = 0; j < temp.Length; j++)
      {
        if (temp[j].id == j + 1)
        {
          temp[j].id = j;
        }
      }

      return temp;
    }

    /// <summary>
    /// Get all context names of the array.
    /// </summary>
    /// <param name="contexts">A array of contexts.</param>
    /// <param name="language">The language of the names.</param>
    /// <returns>Return a string array with all names.</returns>
    public static string[] GetNames(Context[] contexts, string language)
    {
      var names = new string[0];
      foreach (var context in contexts)
      {
        foreach (var name in context.name)
        {
          if (name.key == language) names = ArrayHelper.Add(names, name.value);
        }
      }

      return names;
    }

    /// <summary>
    /// Get the context name.
    /// </summary>
    /// <param name="language">The language of the name.</param>
    /// <returns>The name string.</returns>
    public string GetName(string language)
    {
      foreach (var langName in name)
      {
        if (langName.key == language) return langName.value;
      }

      return string.Empty;
    }

    /// <summary>
    /// Return the data of the object to save in a persistent object.
    /// </summary>
    /// <returns>A persistent object.</returns>
    public override Persistent GetData()
    {
      var context = new ContextPersistent();
      context.id = uniqueId;
      context.happened = happened;
      context.columns = Data.GetArrayData<ColumnPersistent>(columns);
      return context;
    }

    /// <summary>
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    public override void SetData(Persistent persistentData)
    {
      var context = (ContextPersistent) persistentData;
      uniqueId = context.id;
      happened = context.happened;
      columns = Data.SetArrayData<Column>(columns, context.columns);
    }
  }
}