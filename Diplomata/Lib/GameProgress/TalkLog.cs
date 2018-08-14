using System;

namespace Diplomata.GameProgress
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
      foreach (TalkLog talkLog in array)
      {
        if (talkLog.talkableName == talkableName)
        {
          return talkLog;
        }
      }

      return null;
    }
  }
}
