using System;

namespace Diplomata
{
  [Serializable]
  public class Talk
  {
    public Action onStart;
    public Action onEnd;

    [NonSerialized] public bool canTalk;
    [NonSerialized] public DiplomataCharacter character;
    [NonSerialized] public MessageBox messageBox;
    [NonSerialized] public ChoiceMenu choiceMenu;

    public void Start()
    {
      if (character.talking)
      {
        canTalk = false;
        onStart();
      }
    }

    public void End()
    {
      onEnd();
      character.NextMessage();
      messageBox.messageBox.SetActive(false);
      canTalk = true;
    }

  }
}
