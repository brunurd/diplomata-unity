using System;
using Diplomata.Helpers;

namespace Diplomata.Models
{
  [Serializable]
  public class Column
  {
    public int id;
    public string emitter;
    public Message[] messages;

    public Column() {}

    public Column(int id)
    {
      this.id = id;
      emitter = DiplomataData.options.playerCharacterName;

      messages = new Message[0];
    }

    public static Column[] RemoveEmptyColumns(Column[] columns)
    {
      var newArray = new Column[0];

      for (int i = 0; i < columns.Length; i++)
      {
        if (columns[i].messages.Length > 0)
        {
          newArray = ArrayHelper.Add(newArray, columns[i]);
        }
      }

      for (int i = 0; i < newArray.Length; i++)
      {
        if (newArray[i].id == i + 1)
        {
          newArray[i].id = i;

          foreach (Message msg in newArray[i].messages)
          {
            msg.columnId = i;
          }
        }
      }

      return newArray;
    }

    /// <summary>
    /// Find a column by it id.
    /// </summary>
    /// <param name="context">A context.</param>
    /// <param name="columnId">The id of the column.</param>
    /// <returns>The column if found, or null.</returns>
    public static Column Find(Context context, int columnId)
    {
      return (Column) Helpers.Find.In(context.columns).Where("id", columnId).Result;
    }
  }
}
