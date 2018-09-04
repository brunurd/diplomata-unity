using System;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Persistence;
using LavaLeak.Diplomata.Persistence.Models;
using UnityEngine;

namespace LavaLeak.Diplomata.Models
{
  [Serializable]
  public class TalkLog : Data
  {
    [SerializeField] public string uniqueId = Guid.NewGuid().ToString();
    public string talkableName;
    public string[] messagesIds = new string[0];

    public TalkLog() {}

    public TalkLog(string talkableName)
    {
      uniqueId = Guid.NewGuid().ToString();
      this.talkableName = talkableName;
    }

    public static TalkLog Find(TalkLog[] array, string talkableName)
    {
      return (TalkLog) Helpers.Find.In(array).Where("talkableName", talkableName).Result;
    }

    /// <summary>
    /// Return the data of the object to save in a persistent object.
    /// </summary>
    /// <returns>A persistent object.</returns>
    public override Persistent GetData()
    {
      var talkLog = new TalkLogPersistent();
      talkLog.id = uniqueId;
      talkLog.messagesIds = messagesIds;
      return talkLog;
    }

    /// <summary>
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    public override void SetData(Persistent persistentData)
    {
      var talkLogPersistentData = (TalkLogPersistent) persistentData;
      uniqueId = talkLogPersistentData.id;
      messagesIds = talkLogPersistentData.messagesIds;
    }
  }
}
