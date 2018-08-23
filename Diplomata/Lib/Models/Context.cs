using System;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Models;

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
  public class Context
  {
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
    public LanguageDictionary[] name;
    public LanguageDictionary[] description;
    public Column[] columns;
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
      this.id = id;
      this.talkableName = talkableName;
      columns = new Column[0];
      name = new LanguageDictionary[0];
      labels = new Label[] { new Label() };
      description = new LanguageDictionary[0];

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
  }
}
