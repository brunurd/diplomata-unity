using System;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Persistence;
using LavaLeak.Diplomata.Persistence.Models;
using UnityEngine;

namespace LavaLeak.Diplomata.Models
{
  [Serializable]
  public class Column : Data
  {
    [SerializeField]
    public string uniqueId = Guid.NewGuid().ToString();

    // TODO: Use only unique id.
    public int id;

    public string emitter;
    public Message[] messages;

    public Column() {}

    public Column(int id)
    {
      uniqueId = Guid.NewGuid().ToString();
      this.id = id;
      emitter = DiplomataManager.Data.options.playerCharacterName;

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

    public override Persistent GetData()
    {
      var column = new ColumnPersistent();
      column.id = uniqueId;
      column.messages = Data.GetArrayData<MessagePersistent>(messages);
      return column;
    }

    public override void SetData(Persistent persistentData)
    {
      var column = (ColumnPersistent) persistentData;
      uniqueId = column.id;
      messages = Data.SetArrayData<Message>(messages, column.messages);
    }
  }
}
