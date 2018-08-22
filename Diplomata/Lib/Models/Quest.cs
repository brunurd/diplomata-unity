using System;
using System.Collections.Generic;
using System.Text;
using Diplomata.Helpers;
using UnityEngine;

namespace Diplomata.Models
{
  /// <summary>
  /// Class of quest that can have multiples states.
  /// </summary>
  [Serializable]
  public class Quest
  {
    [SerializeField] private string id;
    public string Name;
    [SerializeField] private QuestState[] questStates;
    private string questStateId;
    private bool initialized;
    private bool finished;

    /// <summary>
    /// Basic constructor, it sets the id and create the default "In progress" state.
    /// </summary>
    public Quest()
    {
      id = Guid.NewGuid().ToString();
      questStates = new QuestState[] { new QuestState("In progress.") };
    }

    /// <summary>
    /// Get the quest id private field.
    /// </summary>
    /// <returns>The id (a guid string).</returns>
    public string GetId()
    {
      return id;
    }

    /// <summary>
    /// Add a state to the states list.
    /// </summary>
    /// <param name="questState">The state name.</param>
    public void AddState(string questState)
    {
      if (!finished) questStates = ArrayHelper.Add(questStates, new QuestState(questState));
    }

    /// <summary>
    /// Get the index of the current state of the quest in questStates array.
    /// </summary>
    /// <returns>Return the index or -1 if don't have a current state.</returns>
    private int GetStateIndex()
    {
      int index = -1;
      if (questStateId != string.Empty && questStateId != null && initialized && !finished)
      {
        for (var i = 0; i < questStates.Length; i++)
        {
          if (questStates[i].GetId() == questStateId)
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
      if (!initialized)
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
      if (!finished && !initialized)
      {
        initialized = true;
        questStateId = questStates[0].GetId();
      }
    }

    /// <summary>
    /// Get the current quest name.
    /// </summary>
    /// <returns>Return the current quest name, if don't have one return null.</returns>
    public string GetCurrentState()
    {
      int index = GetStateIndex();
      if (index != -1) return questStates[index].Name;
      else return null;
    }

    /// <summary>
    /// Get a state from id.
    /// </summary>
    /// <param name="id">The id to find.</param>
    /// <returns>The state object.</returns>
    public QuestState GetState(string id)
    {
      foreach (QuestState state in questStates)
      {
        if (state.GetId() == id) return state;
      }
      return null;
    }

    /// <summary>
    /// Go to the next state, if don't have one finish the quest.
    /// </summary>
    public void NextState()
    {
      if (!finished)
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
            questStateId = questStates[index + 1].GetId();
          }
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
        var completed = (currentStateIndex > i && currentStateIndex > -1) ? true : false;
        questLog.Add(name, completed);
      }

      return questLog;
    }

    /// <summary>
    /// Get a list of the names of the quests of a array.
    /// </summary>
    /// <param name="quests">The quests array.</param>
    /// <returns>A array of names as strings.</returns>
    public static string[] GetNames(Quest[] quests)
    {
      string[] questsReturn = quests == null ? new string[0] : new string[quests.Length];
      if (quests != null)
      {
        for (var i = 0; i < quests.Length; i++)
        {
          questsReturn[i] = quests[i].Name;
        }
      }
      return questsReturn;
    }

    /// <summary>
    /// Get a list of the ids of the quests of a array.
    /// </summary>
    /// <param name="quests">The quests array.</param>
    /// <returns>A array of ids as strings.</returns>
    public static string[] GetIDs(Quest[] quests)
    {
      string[] questsReturn = quests == null ? new string[0] : new string[quests.Length];
      if (quests != null)
      {
        for (var i = 0; i < quests.Length; i++)
        {
          questsReturn[i] = quests[i].GetId();
        }
      }
      return questsReturn;
    }

    /// <summary>
    /// Return the quest states.
    /// </summary>
    /// <returns>A array with the quest states.</returns>
    public QuestState[] GetQuestStates()
    {
      return questStates;
    }

    /// <summary>
    /// Finish the quest and set the state to empty.
    /// </summary>
    private void Finish()
    {
      if (initialized)
      {
        finished = true;
        questStateId = string.Empty;
      }
    }

    /// <summary>
    /// Find a quest from a array.
    /// </summary>
    /// <param name="quests">A array of quests.</param>
    /// <param name="with">The quest id or name.</param>
    /// <returns>The quest with that id or null if don't exists.</returns>
    public static Quest Find(Quest[] quests, params string[] with)
    {
      foreach (Quest quest in quests)
      {
        if (ArrayHelper.Contains(with, quest.id) || ArrayHelper.Contains(with, quest.Name))
        {
          return quest;
        }
      }
      return null;
    }

    /// <summary>
    /// The string return.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      var questString = new StringBuilder();
      questString.Append(string.Format("{0}: ", Name));
      foreach (KeyValuePair<string, bool> state in GetQuestLog())
      {
        questString.Append(string.Format("{0}:{1}; ", state.Key, state.Value));
      }
      return questString.ToString();
    }
  }

  /// <summary>
  /// Class to serialize as json.
  /// </summary>
  [Serializable]
  public class Quests
  {
    [SerializeField] private Quest[] quests = new Quest[0];

    /// <summary>
    /// A clean constructor.
    /// </summary>
    public Quests() {}

    /// <summary>
    /// A constructor with a quests array.
    /// Made to work with <seealso cref="UnityEngine.JsonUtility.ToJson(object)">.
    /// </summary>
    /// <param name="quests">A array of quests objects.</param>
    public Quests(Quest[] quests)
    {
      this.quests = quests;
    }

    /// <summary>
    /// Get the quest array private field.
    /// Made to work with <seealso cref="UnityEngine.JsonUtility.FromJson(string, Type)">.
    /// </summary>
    /// <returns>A array of quests.</returns>
    public Quest[] GetQuests()
    {
      return quests;
    }
  }
}
