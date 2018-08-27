using System;
using Diplomata.Helpers;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class TalkLog
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
  }
}
