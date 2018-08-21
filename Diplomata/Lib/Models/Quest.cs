using System;
using System.Collections.Generic;
using Diplomata.Helpers;

namespace Diplomata.Models
{
  /// <summary>
  /// Class of quest that can have multiples states.
  /// </summary>
  [Serializable]
  public class Quest
  {
    public string Id { get; private set; }
    public string Name;
    private QuestState[] questStates;
    private string questStateId;
    public bool Initialized { get; private set; }
    public bool Finished { get; private set; }

    /// <summary>
    /// Basic constructor, it sets the id and create the default "In progress" state.
    /// </summary>
    public Quest()
    {
      Id = Guid.NewGuid().ToString();
      questStates = new QuestState[] { new QuestState("In progress") };
    }

    /// <summary>
    /// Add a state to the states list.
    /// </summary>
    /// <param name="questState">The state name.</param>
    public void AddState(string questState)
    {
      questStates = ArrayHelper.Add(questStates, new QuestState(questState));
    }

    /// <summary>
    /// Get the index of the current state of the quest in questStates array.
    /// </summary>
    /// <returns>Return the index or -1 if don't have a current state.</returns>
    private int GetStateIndex()
    {
      int index = -1;
      if (questStateId != string.Empty && questStateId != null && Initialized)
      {
        for (var i = 0; i < questStates.Length; i++)
        {
          if (questStates[i].Id == questStateId)
          {
            index = i;
            break;
          }
        }
      }
      return index;
    }

    /// <summary>
    /// Remove a state from states list if the quest has not initialized.
    /// </summary>
    /// <param name="questState">The state to remove.</param>
    public void RemoveState(QuestState questState)
    {
      if (!Initialized)
      {
        if (ArrayHelper.Contains(questStates, questState))
        {
          questStates = ArrayHelper.Remove(questStates, questState);
        }
      }
    }

    /// <summary>
    /// Initialize a quest setting the state to the first one.
    /// </summary>
    public void Initialize()
    {
      Initialized = true;
      questStateId = questStates[0].Id;
    }

    /// <summary>
    /// Get the current quest name.
    /// </summary>
    /// <returns>Return the current quest name, if don't have one return null.</returns>
    public string GetState()
    {
      int index = GetStateIndex();
      if (index != -1) return questStates[index].Name;
      else return null;
    }

    /// <summary>
    /// Go to the next state, if don't have one finish the quest.
    /// </summary>
    public void NextState()
    {
      int index = GetStateIndex();
      if (index != -1)
      {
        if (index == questStates.Length - 1)
        {
          Finish();
        }
        else
        {
          questStateId = questStates[index + 1].Id;
        }
      }
    }

    /// <summary>
    /// Return a dictionary with all the quest states and if it is complete or don't.
    /// </summary>
    /// <typeparam name="string">The quest state name.</typeparam>
    /// <typeparam name="bool">If the quest is complete.</typeparam>
    /// <returns>A dictionary with all the quest states.</returns>
    public Dictionary<string, bool> GetQuestLog()
    {
      var questLog = new Dictionary<string, bool>();
      int currentStateIndex = GetStateIndex();

      for (var i = 0; i < questStates.Length; i++)
      {
        var name = questStates[i].Name;
        var completed = currentStateIndex > i ? true : false;
        questLog.Add(name, completed);
      }

      return questLog;
    }

    /// <summary>
    /// Finish the quest and set the state to empty.
    /// </summary>
    public void Finish()
    {
      Finished = true;
      questStateId = string.Empty;
    }
  }
}
