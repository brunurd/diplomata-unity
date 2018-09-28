using System;
using System.Collections.Generic;
using System.Text;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models.Submodels;
using LavaLeak.Diplomata.Persistence;
using LavaLeak.Diplomata.Persistence.Models;
using UnityEngine;

namespace LavaLeak.Diplomata.Models
{
  /// <summary>
  /// Class of quest that can have multiples states.
  /// </summary>
  [Serializable]
  public class Quest : Data
  {
    [SerializeField]
    private string uniqueId;

    public string Name;
    public QuestState[] questStates;
    private string currentStateId;
    
    public bool Initialized
    {
      get { return initialized; }
    }
    
    private bool initialized;
    private bool finished;

    /// <summary>
    /// Basic constructor, it sets the id and create the default "In progress" state.
    /// </summary>
    public Quest()
    {
      uniqueId = Guid.NewGuid().ToString();
      questStates = new QuestState[] { new QuestState("Short description.", "Long description.") };
    }

    /// <summary>
    /// Get the quest id private field.
    /// </summary>
    /// <returns>The id (a guid string).</returns>
    public string GetId()
    {
      return uniqueId;
    }

    /// <summary>
    /// Get if the quest is active.
    /// </summary>
    /// <returns>Return true if is active or false.</returns>
    public bool IsActive()
    {
      return initialized && !finished;
    }

    /// <summary>
    /// Get if the quest is complete.
    /// </summary>
    /// <returns>Return true if is complete or false.</returns>
    public bool IsComplete()
    {
      return initialized && finished;
    }

    /// <summary>
    /// Add a state to the states list.
    /// </summary>
    /// <param name="questState">The state name.</param>
    public void AddState(string questState, string longDescription)
    {
      if (!finished) questStates = ArrayHelper.Add(questStates, new QuestState(questState, longDescription));
    }

    /// <summary>
    /// Get the index of the current state of the quest in questStates array.
    /// </summary>
    /// <returns>Return the index or -1 if don't have a current state.</returns>
    private int GetStateIndex()
    {
      int index = -1;
      if (currentStateId != string.Empty && currentStateId != null && initialized && !finished)
      {
        for (var i = 0; i < questStates.Length; i++)
        {
          if (questStates[i].GetId() == currentStateId)
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
        currentStateId = questStates[0].GetId();
      }
    }

    /// <summary>
    /// Get the current quest name.
    /// </summary>
    /// <returns>Return the current quest name, if don't have one return null.</returns>
    public string GetCurrentState()
    {
      int index = GetStateIndex();
      if (index != -1) return questStates[index].ShortDescription;
      else return null;
    }

    public string GetCurrentStateID()
    {
      var index = GetStateIndex();
      return index != -1 ? questStates[index].GetId() : null;
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
    /// <returns>A dictionary with all the quest states.</returns>
    public Dictionary<string, bool> NextState()
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
            currentStateId = questStates[index + 1].GetId();
          }
        }
      }
      return GetQuestLog();
    }

    /// <summary>
    /// Force a quest state.
    /// </summary>
    /// <param name="stateId">The id (a string guid) of the quest state to force.</param>
    /// <returns>A dictionary with all the quest states.</returns>
    public Dictionary<string, bool> SetState(string stateId)
    {
      if (!finished)
      {
        initialized = true;
        if (ArrayHelper.Contains(QuestState.GetIDs(questStates), stateId))
          currentStateId = stateId;
      }
      else
      {
        Debug.Log(string.Format("Quest {0} already finished.", Name));
      }
      return GetQuestLog();
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
        var name = questStates[i].ShortDescription;
        var completed = true;

        if (!initialized)
          completed = false;
        else if (!finished)
          completed = (currentStateIndex > i && currentStateIndex > -1) ? true : false;

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
    /// Finish the quest and set the state to empty.
    /// </summary>
    public void Finish()
    {
      if (initialized)
      {
        finished = true;
        currentStateId = string.Empty;
      }
    }

    /// <summary>
    /// Find a quest from a array.
    /// </summary>
    /// <param name="quests">A array of quests.</param>
    /// <param name="value">The quest id (a string guid) or name.</param>
    /// <returns>The quest if found, or null.</returns>
    public static Quest Find(Quest[] quests, string value)
    {
      var quest = (Quest) Helpers.Find.In(quests).Where("uniqueId", value).Result;
      if (quest == null) quest = (Quest) Helpers.Find.In(quests).Where("Name", value).Result;
      return quest;
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

    /// <summary>
    /// Return the data of the object to save in a persistent object.
    /// </summary>
    /// <returns>A persistent object.</returns>
    public override Persistent GetData()
    {
      var quest = new QuestPersistent();
      quest.id = uniqueId;
      quest.currentStateId = currentStateId;
      quest.initialized = initialized;
      quest.finished = finished;
      return quest;
    }

    /// <summary>
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    public override void SetData(Persistent persistentData)
    {
      var questPersistentData = (QuestPersistent) persistentData;
      uniqueId = questPersistentData.id;
      currentStateId = questPersistentData.currentStateId;
      initialized = questPersistentData.initialized;
      finished = questPersistentData.finished;
    }
  }
}
