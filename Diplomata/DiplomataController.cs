using Diplomata.Helpers;
using Diplomata.Models;
using Diplomata.Persistence;
using Diplomata.Persistence.Models;

namespace Diplomata
{
  public static class DiplomataController
  {
    /// <summary>
    /// Get a global flag by name.
    /// </summary>
    /// <param name="name">The name of the flag.</param>
    /// <returns>The <seealso cref="Diplomata.Models.Flag"> object or null.</returns>
    public static Flag GetFlag(string name)
    {
      return DiplomataData.globalFlags.Find(DiplomataData.globalFlags.flags, name);
    }

    /// <summary>
    /// Get a character by his name.
    /// </summary>
    /// <param name="name">The character name.</param>
    /// <returns>The <seealso cref="Diplomata.Models.Character"> object or null.</returns>
    public static Character GetCharacter(string name)
    {
      return Character.Find(DiplomataData.characters, name);
    }

    /// <summary>
    /// Get a context by it's index.
    /// </summary>
    /// <param name="character">The character parent of the context.</param>
    /// <param name="contextIndex">The index of the context.</param>
    /// <returns>The <seealso cref="Diplomata.Models.Context"> object or null.</returns>
    public static Context GetContext(Character character, int contextIndex)
    {
      return Context.Find(character, contextIndex);
    }

    /// <summary>
    /// Get a message by it's context and row index.
    /// </summary>
    /// <param name="context">The context parent of the message.</param>
    /// <param name="columnIndex">The index of the column.</param>
    /// <param name="rowIndex">The index of the row in the column.</param>
    /// <returns></returns>
    public static Message GetMessage(Context context, int columnIndex, int rowIndex)
    {
      var column = Column.Find(context, columnIndex);
      if (column == null) return null;
      var message = Message.Find(column.messages, rowIndex);
      if (message != null) return message;
      return null;
    }

    /// <summary>
    /// Get a context by it's unique id.
    /// </summary>
    /// <param name="uniqueId">The unique id (a guid string).</param>
    /// <returns>The <seealso cref="Diplomata.Models.Message"> object or null.</returns>
    public static Message GetMessage(string uniqueId)
    {
      foreach (Character character in DiplomataData.characters)
      {
        foreach (Context context in character.contexts)
        {
          foreach (Column column in context.columns)
          {
            var message = Message.Find(column.messages, uniqueId);
            if (message != null) return message;
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Get a quest by it's name.
    /// </summary>
    /// <param name="name">The quest name.</param>
    /// <returns>The <seealso cref="Diplomata.Models.Quest"> object or null.</returns>
    public static Quest GetQuest(string name)
    {
      return Quest.Find(DiplomataData.quests, name);
    }

    /// <summary>
    /// Get a item by it's name.
    /// </summary>
    /// <param name="name">The item name.</param>
    /// <param name="language">The language of this name, if empty uses the options first language.</param>
    /// <returns>The <seealso cref="Diplomata.Models.Item"> object or null.</returns>
    public static Item GetItem(string name, string language = "")
    {
      if (language == "") language = DiplomataData.options.languages[0].name;
      return Item.Find(DiplomataData.inventory.items, name, language);
    }

    /// <summary>
    /// Return all characters persistent 
    /// </summary>
    /// <returns>A array of characters persistent </returns>
    public static CharacterPersistentContainer GetPersistentCharacters()
    {
      return new CharacterPersistentContainer(Persistence.Data.GetArrayData<CharacterPersistent>(DiplomataData.characters.ToArray()));
    }

    /// <summary>
    /// Return all interactables persistent 
    /// </summary>
    /// <returns>A array of interactables persistent </returns>
    public static InteractablePersistentContainer GetPersistentInteractables()
    {
      return new InteractablePersistentContainer(Persistence.Data.GetArrayData<InteractablePersistent>(DiplomataData.interactables.ToArray()));
    }

    /// <summary>
    /// Return all quests persistent 
    /// </summary>
    /// <returns>A array of quests persistent </returns>
    public static QuestPersistentContainer GetPersistentQuests()
    {
      return new QuestPersistentContainer(Persistence.Data.GetArrayData<QuestPersistent>(DiplomataData.quests));
    }

    /// <summary>
    /// Return all talk logs persistent 
    /// </summary>
    /// <returns>A array of talk logs persistent </returns>
    public static TalkLogPersistentContainer GetPersistentTalkLogs()
    {
      return new TalkLogPersistentContainer(Persistence.Data.GetArrayData<TalkLogPersistent>(DiplomataData.talkLogs));
    }

    /// <summary>
    /// Find a model by a field value.
    /// </summary>
    /// <param name="model">The desired Diplomata model.</param>
    /// <param name="field">The name of the field model.</param>
    /// <param name="value">The values to find.</param>
    /// <returns>A array of results.</returns>
    public static object[] Find(Model model, string field, object value)
    {
      switch (model)
      {
        case Model.OPTIONS:
          return (Options[]) Helpers.Find.In(new Options[] { DiplomataData.options }).Where(field, value).Results;

        case Model.CHARACTER:
          return (Character[]) Helpers.Find.In(DiplomataData.characters.ToArray()).Where(field, value).Results;

        case Model.CONTEXT:
          var contexts = new Context[0];
          foreach (var character in DiplomataData.characters)
          {
            contexts = ArrayHelper.Merge(contexts, (Context[]) Helpers.Find.In(character.contexts).Where(field, value).Results);
          }
          foreach (var interactable in DiplomataData.interactables)
          {
            contexts = ArrayHelper.Merge(contexts, (Context[]) Helpers.Find.In(interactable.contexts).Where(field, value).Results);
          }
          return contexts;

        case Model.COLUMN:
          var columns = new Column[0];
          foreach (var character in DiplomataData.characters)
          {
            foreach (var context in character.contexts)
            {
              columns = ArrayHelper.Merge(columns, (Column[]) Helpers.Find.In(context.columns).Where(field, value).Results);
            }
          }
          foreach (var interactable in DiplomataData.interactables)
          {
            foreach (var context in interactable.contexts)
            {
              columns = ArrayHelper.Merge(columns, (Column[]) Helpers.Find.In(context.columns).Where(field, value).Results);
            }
          }
          return columns;

        case Model.MESSAGE:
          var messages = new Message[0];
          foreach (var character in DiplomataData.characters)
          {
            foreach (var context in character.contexts)
            {
              foreach (var column in context.columns)
              {
                messages = ArrayHelper.Merge(messages, (Message[]) Helpers.Find.In(column.messages).Where(field, value).Results);
              }
            }
          }
          foreach (var interactable in DiplomataData.interactables)
          {
            foreach (var context in interactable.contexts)
            {
              foreach (var column in context.columns)
              {
                messages = ArrayHelper.Merge(messages, (Message[]) Helpers.Find.In(column.messages).Where(field, value).Results);
              }
            }
          }
          return messages;

        case Model.INTERACTABLE:
          return (Interactable[]) Helpers.Find.In(DiplomataData.interactables.ToArray()).Where(field, value).Results;

        case Model.ITEM:
          return (Item[]) Helpers.Find.In(DiplomataData.inventory.items).Where(field, value).Results;

        case Model.FLAG:
          return (Flag[]) Helpers.Find.In(DiplomataData.globalFlags.flags).Where(field, value).Results;

        case Model.QUEST:
          return (Quest[]) Helpers.Find.In(DiplomataData.quests).Where(field, value).Results;

        case Model.TALKLOG:
          return (TalkLog[]) Helpers.Find.In(DiplomataData.talkLogs).Where(field, value).Results;

        default:
          return new object[0];
      }
    }
  }
}
