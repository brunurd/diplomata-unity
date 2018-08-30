using System;
using System.Collections.Generic;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Models;
using Diplomata.Persistence;
using Diplomata.Persistence.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Diplomata.Models
{
  public enum MessageEditorState
  {
    None,
    Normal,
    Conditions,
    Effects
  }

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
    public LanguageDictionary[] description;
    public Label[] labels = new Label[] { new Label() };

    [NonSerialized]
    public bool happened;

    public struct CurrentMessage
    {
      public int columnId;
      public int rowId;

      public CurrentMessage(int columnId, int rowId)
      {
        this.columnId = columnId;
        this.rowId = rowId;
      }

      public void Set(int columnId, int rowId)
      {
        this.columnId = columnId;
        this.rowId = rowId;
      }
    }

    public Context() {}

    public Context(int id, string talkableName)
    {
      uniqueId = Guid.NewGuid().ToString();
      this.id = id;
      this.talkableName = talkableName;
      name = new LanguageDictionary[0];
      description = new LanguageDictionary[0];
      columns = new Column[0];
      labels = new Label[] { new Label() };

      foreach (Language lang in DiplomataData.options.languages)
      {
        name = ArrayHelper.Add(name, new LanguageDictionary(lang.name, "Name [Change clicking on Edit]"));
        description = ArrayHelper.Add(description, new LanguageDictionary(lang.name, "Description [Change clicking on Edit]"));
      }
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
