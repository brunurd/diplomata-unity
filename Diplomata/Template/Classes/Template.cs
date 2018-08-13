using UnityEngine;

namespace DiplomataLib
{
  public class Template : MonoBehaviour
  {
    public static Template instance;
    public static bool canTalk;

    public Talk talk;
    public MessageBox messageBox;
    public ChoiceMenu choiceMenu;

    public void Awake()
    {
      if (instance == null)
      {
        instance = this;
        canTalk = true;

        // Set Talk
        talk.messageBox = messageBox;
        talk.choiceMenu = choiceMenu;

        // Set Message Box
        messageBox.talk = talk;
        messageBox.choiceMenu = choiceMenu;
        messageBox.messageBox.SetActive(false);
        messageBox.playerEmitterBox.SetActive(false);
        messageBox.otherEmitterBox.SetActive(false);

        // Set Choice Menu
        choiceMenu.talk = talk;
        choiceMenu.messageBox = messageBox;
        choiceMenu.choiceMenu.SetActive(false);
        choiceMenu.choiceObject.SetActive(false);
      }

      else
      {
        DestroyImmediate(gameObject);
      }
    }
  }
}
