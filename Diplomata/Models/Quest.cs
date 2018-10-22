using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LavaLeak.Diplomata.Dictionaries;
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
    private bool finished;

    // The null value is necessary for database serialization.
    private string currentStateId = null;

    public LanguageDictionary[] Name;
    public QuestState[] questStates;
    public bool Initialized { get; private set; }

    /// <summary>
    /// Basic constructor, it sets the id and create the default "In progress" state.
    /// </summary>
    public Quest()
    {
      uniqueId = Guid.NewGuid().ToString();
      Name = new LanguageDictionary[0];
      questStates = new QuestState[0];
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
      return Initialized && !finished;
    }

    /// <summary>
    /// Get if the quest is complete.
    /// </summary>
    /// <returns>Return true if is complete or false.</returns>
    public bool IsComplete()
    {
      return Initialized && finished;
    }

    /// <summary>
    /// Add a state to the states list.
    /// </summary>
    /// <param name="questState">The state name.</param>
    public void AddState(string questState, string longDescription)
    {
      if (!finished) questStates = ArrayHelper.Add(questStates, new QuestState());
    }

    /// <summary>
    /// Get the index of the current state of the quest in questStates array.
    /// </summary>
    /// <returns>Return the index or -1 if don't have a current state.</returns>
    private int GetStateIndex()
    {
      var index = -1;
      if (string.IsNullOrEmpty(currentStateId) || !Initialized || finished)
        return index;
      for (var i = 0; i < questStates.Length; i++)
      {
        if (questStates[i].GetId() != currentStateId) continue;
        index = i;
        break;
      }

      return index;
    }

    /// <summary>
    /// Remove a state from states list if the quest has not initialized.
    /// </summary>
    /// <param name="questState">The state to remove.</param>
    public void RemoveState(QuestState questState)
    {
      if (Initialized) return;
      if (ArrayHelper.Contains(questStates, questState))
        questStates = ArrayHelper.Remove(questStates, questState);
    }

    /// <summary>
    /// Initialize a quest setting the state to the first one.
    /// </summary>
    public void Initialize()
    {
      if (!finished && !Initialized)
      {
        Initialized = true;
        currentStateId = questStates[0].GetId();
      }
    }

    /// <summary>
    /// Get the current quest name.
    /// </summary>
    /// <returns>Return the current quest name, if don't have one return null.</returns>
    public string GetCurrentState(string language = "")
    {
      var index = GetStateIndex();
      return index != -1 ? questStates[index].GetShortDescription(language) : null;
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
      foreach (var state in questStates)
      {
        if (state.GetId() == id)
          return state;
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
            DiplomataManager.EventController.SendQuestEnd(this);
          }
          else
          {
            currentStateId = questStates[index + 1].GetId();
            DiplomataManager.EventController.SendQuestStateChange(this);
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
        Initialized = true;
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
    /// Return a dictionary with all the quest states names and if it is complete or don't.
    /// </summary>
    /// <returns>A dictionary with all the quest states names.</returns>
    public Dictionary<string, bool> GetQuestLog(string language = "")
    {
      var questLog = new Dictionary<string, bool>();
      var currentStateIndex = GetStateIndex();

      for (var i = 0; i < questStates.Length; i++)
      {
        var completed = true;

        if (!Initialized)
          completed = false;
        else if (!finished)
          completed = currentStateIndex > i && currentStateIndex > -1;

        var questStateName = questStates[i].GetShortDescription(language);
        if (questStateName != null)
          questLog.Add(questStateName, completed);
      }

      return questLog;
    }

    /// <summary>
    /// Return a dictionary with all the quest states and if it is complete or don't.
    /// </summary>
    /// <returns>A dictionary with all the quest states.</returns>
    public Dictionary<QuestState, bool> GetQuestStates()
    {
      var questLog = new Dictionary<QuestState, bool>();
      var currentStateIndex = GetStateIndex();

      for (var i = 0; i < questStates.Length; i++)
      {
        var completed = true;

        if (!Initialized)
          completed = false;
        else if (!finished)
          completed = currentStateIndex > i && currentStateIndex > -1;

        questLog.Add(questStates[i], completed);
      }

      return questLog;
    }

    /// <summary>
    /// Return the first QuestState that is incomplete.
    /// </summary>
    /// <returns>QuestState</returns>
    public QuestState GetFirstIncomplete()
    {
      var currentQuestStateId = GetStateIndex();

      // If don't have quest states.
      if (questStates.Length < 0)
        return null;

      // If can't return current quest state.
      if (currentQuestStateId < 0)
        return questStates[0];

      return questStates[currentQuestStateId];
    }
    
    /// <summary>
    /// Checks if the QuestState with the required ID is complete.
    /// </summary>
    /// <param name="id">Unique ID of the QuestState.</param>
    /// <returns>A bool that indicates if the quest state is complete or not.</returns>
    public bool IsQuestStateCompleteById(string id)
    {
      var allQuestStates = GetQuestStates();

      for (int i = 0; i < allQuestStates.Count; i++)
      {
        //TODO: Change this LINQ
        if (allQuestStates.Keys.ElementAt(i).GetId() == id)
        {
          if (allQuestStates.Values.ElementAt(i))
            return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Get the quest name in the setted language.
    /// </summary>
    /// <param name="language">The language or the current if don't exists.</param>
    /// <returns>The name as string.</returns>
    public string GetName(string language = "")
    {
      language = string.IsNullOrEmpty(language) ? DiplomataManager.Data.options.currentLanguage : language;
      var name = DictionariesHelper.ContainsKey(Name, language);
      return name != null ? name.value : string.Empty;
    }
    
    /// <summary>
    /// Get the quest name in the current language.
    /// </summary>
    /// <param name="options">The options to get current language.</param>
    /// <returns>The name as string.</returns>
    public string GetName(Options options)
    {
      return GetName(options.currentLanguage);
    }

    /// <summary>
    /// Get a list of the names of the quests of a array.
    /// </summary>
    /// <param name="quests">The quests array.</param>
    /// <param name="language">The language to get names.</param>
    /// <returns>A array of names as strings.</returns>
    public static string[] GetNames(Quest[] quests, string language = "")
    {
      string[] questsReturn = quests == null ? new string[0] : new string[quests.Length];
      if (quests != null)
      {
        for (var i = 0; i < quests.Length; i++)
        {
          questsReturn[i] = quests[i].GetName(language);
        }
      }

      return questsReturn;
    }

    /// <summary>
    /// Get a list of the names of the quests of a array.
    /// </summary>
    /// <param name="quests">The quests array.</param>
    /// <param name="options">The options to get current language.</param>
    /// <returns></returns>
    public static string[] GetNames(Quest[] quests, Options options)
    {
      return GetNames(quests, options.currentLanguage);
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
      if (Initialized)
      {
        finished = true;
        currentStateId = null;
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
      quest.initialized = Initialized;
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
      Initialized = questPersistentData.initialized;
      finished = questPersistentData.finished;
    }
  }
}
