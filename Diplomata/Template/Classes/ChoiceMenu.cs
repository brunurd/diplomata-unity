using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DiplomataLib
{

  [Serializable]
  public class ChoiceMenu
  {

    public enum Type
    {
      LIST,
      CARROUSEL
    }

    public Type type;

    public Action onStart = delegate {};
    public Action onUpdate = delegate {};
    public Action onEnd = delegate {};
    public Action onChange = delegate {};

    public GameObject choiceMenu;
    public Transform choiceList;
    [NonSerialized] RectTransform choiceListRect;
    public GameObject lastMessageBox;
    public Text lastMessageText;
    public Text playerEmitterText;
    public GameObject choiceObject;

    [Header("Carrousel")]
    public Button goToLeft;
    public Button goToRight;
    public Button confirm;
    public float offset;
    public int currentChoice;

    //[NonSerialized] public string choice;
    [NonSerialized] public bool selectFirst;

    [NonSerialized] public Talk talk;
    [NonSerialized] public MessageBox messageBox;

    public void Start()
    {
      if (talk.character.talking)
      {

        if (!choiceMenu.activeSelf && talk.character.choiceMenu)
        {
          choiceMenu.SetActive(true);
          onStart();

          choiceObject.SetActive(false);
          currentChoice = 0;

          if (type == Type.CARROUSEL)
          {
            choiceListRect = choiceList.GetComponent<RectTransform>();

            choiceListRect.anchoredPosition = new Vector2(0, choiceListRect.anchoredPosition.y);

            goToLeft.onClick.RemoveAllListeners();
            goToLeft.onClick.AddListener(delegate
            {
              GoToLeft();
            });

            goToRight.onClick.RemoveAllListeners();
            goToRight.onClick.AddListener(delegate
            {
              GoToRight();
            });

            confirm.onClick.RemoveAllListeners();
            confirm.onClick.AddListener(delegate
            {
              End();
            });
          }

          if (lastMessageText != null)
          {
            lastMessageText.text = talk.character.GetLastMessageContent();

            if (lastMessageText.text == "")
            {
              lastMessageBox.SetActive(false);
            }

            else
            {
              lastMessageBox.SetActive(true);
            }
          }

          if (playerEmitterText != null)
          {
            playerEmitterText.text = talk.character.PlayerName();
          }

          var choices = talk.character.MessageChoices();

          for (int i = 0; i < choices.Count; i++)
          {
            var obj = GameObject.Instantiate(choiceObject);
            obj.SetActive(true);
            obj.transform.SetParent(choiceList);
            obj.transform.localScale = new Vector3(1, 1, 1);

            switch (type)
            {
              case Type.LIST:
                var button = obj.GetComponent<Button>();
                var text = obj.transform.GetChild(0).GetComponent<Text>();

                text.text = choices[i];

                button.onClick.AddListener(delegate
                {
                  currentChoice = i;
                  End();
                });
                break;

              case Type.CARROUSEL:
                var carrouselText = obj.GetComponent<Text>();
                carrouselText.text = choices[i];
                break;
            }

            if (i == 0)
            {
              if (selectFirst)
              {
                EventSystem.current.SetSelectedGameObject(obj);
              }
            }
          }
        }
      }
    }

    public void Update()
    {
      if (choiceMenu.activeSelf)
      {
        goToRight.gameObject.SetActive(ShowButton(choiceList.childCount - 2));
        goToLeft.gameObject.SetActive(ShowButton(0));
        onUpdate();
      }
    }

    public void End()
    {
      var text = choiceList.GetChild(currentChoice + 1).GetComponent<Text>();
      talk.character.ChooseMessage(text.text);
      choiceMenu.SetActive(false);

      onEnd();

      for (int i = 1; i < choiceList.childCount; i++)
      {
        GameObject.Destroy(choiceList.GetChild(i).gameObject);
      }
    }

    public bool ShowButton(int index)
    {
      if (currentChoice == index)
      {
        return false;
      }

      else
      {
        return true;
      }
    }

    public void GoToRight()
    {
      if (currentChoice < choiceList.childCount - 2)
      {
        currentChoice += 1;

        choiceListRect.anchoredPosition = new Vector2(
          choiceListRect.anchoredPosition.x - offset,
          choiceListRect.anchoredPosition.y);

        onChange();
      }
    }

    public void GoToLeft()
    {
      if (currentChoice > 0)
      {
        currentChoice -= 1;

        choiceListRect.anchoredPosition = new Vector2(
          choiceListRect.anchoredPosition.x + offset,
          choiceListRect.anchoredPosition.y);

        onChange();
      }
    }
  }

}
