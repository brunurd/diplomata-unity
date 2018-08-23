using System;
using Diplomata.Helpers;

namespace Diplomata.Models
{
  [Serializable]
  public class TalkLog
  {
    public string talkableName;
    public string[] messagesIds = new string[0];

    public TalkLog() {}

    public TalkLog(string talkableName)
    {
      this.talkableName = talkableName;
    }

    public static TalkLog Find(TalkLog[] array, string talkableName)
    {
      return (TalkLog) Helpers.Find.In(array).Where("talkableName", talkableName).Result;
    }
  }
}
