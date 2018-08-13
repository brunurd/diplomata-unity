using System;
using UnityEngine;
using UnityEngine.UI;

namespace DiplomataLib
{

  [Serializable]
  public class MessageBox
  {

    const int LOCKFRAMESLIMIT = 15;

    public Action onStart = delegate {};
    public Action onUpdate = delegate {};
    public Action onEnd = delegate {};
    public Action onEveryLetter = delegate {};

    private int currentFrame;
    private int currentLenght;
    private bool nextMessageLock;
    private int lockTime;

    [NonSerialized] public int maxFrame = 1;
    [NonSerialized] public string messageContent = "";

    [NonSerialized] public bool canFastForward;
    [NonSerialized] public bool exceedBox;

    [NonSerialized] public Talk talk;
    [NonSerialized] public ChoiceMenu choiceMenu;

    public GameObject messageBox;
    public GameObject playerEmitterBox;
    public GameObject otherEmitterBox;
    public ScrollRect contentScrollRect;
    public Text contentText;
    public bool letterByLetter;
    private bool letterByLetterIntern;
    public Button button;

    [Header("Text Button")]
    public Text buttonText;
    public string nextString;
    public string endString;

    [Header("Image Button")]
    public Image buttonImage;
    public Sprite nextSprite;
    public Sprite nextSpriteHighlighted;
    public Sprite endSprite;
    public Sprite endSpriteHighlighted;

    public void Start()
    {
      if (talk.character.talking)
      {
        if (!messageBox.activeSelf && !talk.character.choiceMenu)
        {
          onStart();
          messageBox.SetActive(true);
          canFastForward = false;
          exceedBox = false;

          if (contentScrollRect != null)
          {
            contentScrollRect.verticalNormalizedPosition = 1;
            contentText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
          }

          if (talk.character.EmitterIsPlayer())
          {
            otherEmitterBox.SetActive(false);
            playerEmitterBox.SetActive(true);
            var text = playerEmitterBox.transform.GetChild(0).GetComponent<Text>();
            text.text = talk.character.Emitter();
          }

          else
          {
            playerEmitterBox.SetActive(false);
            otherEmitterBox.SetActive(true);
            var text = otherEmitterBox.transform.GetChild(0).GetComponent<Text>();
            text.text = talk.character.Emitter();
          }

          button.onClick.RemoveAllListeners();

          if (talk.character.IsLastMessage())
          {
            if (buttonText != null)
            {
              buttonText.text = endString;
            }

            if (buttonImage != null && endSprite != null)
            {
              buttonImage.sprite = endSprite;

              if (endSpriteHighlighted != null)
              {
                SpriteState spriteState = new SpriteState();
                spriteState.highlightedSprite = endSpriteHighlighted;
                spriteState.pressedSprite = endSprite;
                spriteState.disabledSprite = endSprite;

                button.transition = Selectable.Transition.SpriteSwap;
                button.spriteState = spriteState;
              }
            }

            button.onClick.AddListener(() =>
            {
              talk.End();
            });
          }

          else
          {
            if (buttonText != null)
            {
              buttonText.text = nextString;
            }

            if (buttonImage != null && nextSprite != null)
            {
              buttonImage.sprite = nextSprite;

              if (nextSpriteHighlighted != null)
              {
                SpriteState spriteState = new SpriteState();
                spriteState.highlightedSprite = nextSpriteHighlighted;
                spriteState.pressedSprite = nextSprite;
                spriteState.disabledSprite = nextSprite;

                button.transition = Selectable.Transition.SpriteSwap;
                button.spriteState = spriteState;
              }
            }

            button.onClick.AddListener(() =>
            {
              End();
            });
          }

          if (letterByLetter)
          {
            letterByLetterIntern = true;
            contentText.text = "";
            button.gameObject.SetActive(false);
          }

          else
          {
            contentText.text = talk.character.ShowMessageContentSubtitle();
          }
        }
      }
    }

    public void Update()
    {
      if (messageBox.activeSelf)
      {
        var fullContent = talk.character.ShowMessageContentSubtitle();

        if (letterByLetterIntern)
        {
          if (currentFrame < maxFrame)
          {
            currentFrame += 1;
          }

          else
          {
            currentFrame = 0;

            messageContent += fullContent[currentLenght];
            currentLenght += 1;

            contentText.text = messageContent;

            onEveryLetter();
          }

          if (currentLenght == fullContent.Length)
          {
            currentFrame = 0;
            currentLenght = 0;
            messageContent = "";
            letterByLetterIntern = false;
            button.gameObject.SetActive(true);
          }
        }

        if (canFastForward)
        {
          if (Input.GetMouseButtonDown(0) || Input.anyKeyDown || Input.touchCount > 0)
          {
            if (button.gameObject.activeSelf && !nextMessageLock)
            {
              button.gameObject.SetActive(false);
              End();
            }

            else
            {
              nextMessageLock = true;
              contentText.text = fullContent;
              currentFrame = 0;
              currentLenght = 0;
              messageContent = "";
              letterByLetterIntern = false;
              button.gameObject.SetActive(true);
            }
          }
        }

        if (nextMessageLock && lockTime < LOCKFRAMESLIMIT)
        {
          lockTime += 1;
        }

        else
        {
          lockTime = 0;
          nextMessageLock = false;
        }

        onUpdate();
      }
    }

    public void End()
    {
      onEnd();
      talk.character.NextMessage();

      if (!talk.character.talking)
      {
        talk.End();
      }

      messageBox.SetActive(false);
    }
  }

}
