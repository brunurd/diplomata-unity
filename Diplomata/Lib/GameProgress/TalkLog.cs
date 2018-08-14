namespace DiplomataLib
{
  [Serializable]
  public class TalkLog
  {
    public string characterName;
    public string[] messagesIds = new string[0];

    public TalkLog() {}

    public TalkLog(string characterName)
    {
      this.characterName = characterName;
    }

    public static TalkLog Find(TalkLog[] array, string characterName)
    {
      foreach (TalkLog talkLog in array)
      {
        if (talkLog.characterName == characterName)
        {
          return talkLog;
        }
      }

      return null;
    }
  }
}