using System;
using System.Collections.Generic;
using Diplomata.Dictionaries;
using Diplomata.GameProgress;
using Diplomata.Helpers;
using Diplomata.Interfaces;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata
{
  /// <summary>
  /// The main class of the talkable
  /// </summary>
  [System.Serializable]
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  public class DiplomataTalkable : MonoBehaviour
  {
    protected Action onStart;

    public List<Message> choices;
    public Talkable talkable;
    public Column currentColumn;
    public Message currentMessage;
    public bool choiceMenu;
    private Context currentContext;
    private Dictionary<string, int> controlIndexes;
    private string lastUniqueId;

    protected void Start()
    {
      choices = new List<Message>();
      controlIndexes = new Dictionary<string, int>();

      controlIndexes.Add("context", 0);
      controlIndexes.Add("column", 0);
      controlIndexes.Add("message", 0);

      onStart();
    }

    private void OnEnable()
    {
      if (talkable != null)
      {
        talkable.onScene = true;
      }
    }

    private void OnDisable()
    {
      if (talkable != null)
      {
        talkable.onScene = false;
      }
    }

    private void OnDestroy()
    {
      if (talkable != null)
      {
        talkable.onScene = false;
      }
    }

    public void StartTalk()
    {
      if (talkable != null)
      {
        controlIndexes = new Dictionary<string, int>();
        controlIndexes.Add("context", 0);
        controlIndexes.Add("column", 0);
        controlIndexes.Add("message", 0);

        DiplomataData.isTalking = true;

        currentContext = null;
        currentColumn = null;
        currentMessage = null;

        choices = new List<Message>();

        foreach (ITalk iTalk in DiplomataData.iTalks) iTalk.OnStart();

        for (controlIndexes["context"] = 0; controlIndexes["context"] < talkable.contexts.Length; controlIndexes["context"]++)
        {
          var context = Context.Find(talkable, controlIndexes["context"]);
          var lastContext = talkable.contexts.Length - 1;

          if (context != null)
          {
            if (!context.happened)
            {
              currentContext = context;
              break;
            }

            else if (controlIndexes["context"] == lastContext)
            {
              Debug.LogWarning("No more contexts avaliable in " + talkable.name + ".");
              EndTalk();
              break;
            }
          }
        }

        if (currentContext == null)
        {
          Debug.LogError("No context found in " + talkable.name + ".");
          EndTalk();
        }

        else
        {
          Next(false);
        }
      }

      else
      {
        Debug.LogError("You don't attached a character or a interactable in the Game Object!");
        EndTalk();
      }
    }

    private void Next(bool hasFate)
    {
      if (talkable != null && currentContext != null)
      {
        if (!currentContext.happened)
        {
          currentColumn = Column.Find(currentContext, controlIndexes["column"]);

          if (currentColumn == null)
          {
            for (controlIndexes["column"] = controlIndexes["column"]; controlIndexes["column"] < talkable.contexts.Length; controlIndexes["column"]++)
            {
              currentColumn = Column.Find(currentContext, controlIndexes["column"]);

              if (currentColumn != null)
              {
                break;
              }
            }
          }

          if (currentColumn != null)
          {
            if (hasFate)
            {
              currentMessage = currentColumn.messages[controlIndexes["message"]];
              OnStartCallbacks();
            }
            else
            {
              var msg = Message.Find(currentColumn.messages, controlIndexes["message"]);
              var proceed = true;

              if (msg != null)
              {

                if (msg.alreadySpoked && msg.disposable)
                {
                  controlIndexes["message"] += 1;
                  Next(false);
                }

                else
                {
                  if (msg.conditions.Length > 0)
                  {

                    foreach (Condition condition in msg.conditions)
                    {

                      switch (condition.type)
                      {
                        case Condition.Type.AfterOf:
                          if (condition.afterOf.uniqueId == lastUniqueId)
                          {
                            condition.proceed = true;
                          }

                          else
                          {
                            condition.proceed = false;
                          }

                          break;

                        case Condition.Type.InfluenceEqualTo:
                          if (talkable.GetType() == typeof(Talkable)) {
                            Character character = (Character) talkable;
                            if (character.influence == condition.comparedInfluence)
                            {
                              condition.proceed = true;
                            }

                            else
                            {
                              condition.proceed = false;
                            }
                          }
                          break;

                        case Condition.Type.InfluenceGreaterThan:
                          if (talkable.GetType() == typeof(Talkable)) {
                            Character character = (Character) talkable;
                            if (character.influence <= condition.comparedInfluence)
                            {
                              condition.proceed = false;
                            }
                          }
                          break;

                        case Condition.Type.InfluenceLessThan:
                          if (talkable.GetType() == typeof(Talkable)) {
                            Character character = (Character) talkable;
                            if (character.influence >= condition.comparedInfluence)
                            {
                              condition.proceed = false;
                            }
                          }
                          break;
                        case Condition.Type.HasItem:
                          var item = Item.Find(DiplomataData.inventory.items, condition.itemId);

                          if (item != null)
                          {

                            if (item.have)
                            {
                              condition.proceed = true;
                            }

                            else
                            {
                              condition.proceed = false;
                            }

                          }

                          else
                          {
                            Debug.LogWarning("Cannot find the item with id " + condition.itemId + " to check.");
                            condition.proceed = false;
                          }

                          break;
                        case Condition.Type.DoesNotHaveTheItem:
                          var itemDont = Item.Find(DiplomataData.inventory.items, condition.itemId);

                          if (itemDont != null)
                          {

                            if (itemDont.have)
                            {
                              condition.proceed = false;
                            }

                            else
                            {
                              condition.proceed = true;
                            }

                          }

                          else
                          {
                            Debug.LogWarning("Cannot find the item with id " + condition.itemId + " to check.");
                            condition.proceed = false;
                          }

                          break;
                        case Condition.Type.ItemIsEquipped:
                          var equippedItem = Item.Find(DiplomataData.inventory.items, condition.itemId);
                          if (equippedItem != null)
                          {
                            if (DiplomataData.inventory.IsEquipped(condition.itemId))
                            {
                              condition.proceed = true;
                            }

                            else
                            {
                              condition.proceed = false;
                            }
                          }

                          else
                          {
                            Debug.LogWarning("Cannot find the item with id " + condition.itemId + " to check.");
                            condition.proceed = false;
                          }

                          break;
                        case Condition.Type.ItemWasDiscarded:
                          var discardedItem = Item.Find(DiplomataData.inventory.items, condition.itemId);

                          if (discardedItem != null)
                          {

                            if (discardedItem.discarded)
                            {
                              condition.proceed = true;
                            }

                            else
                            {
                              condition.proceed = false;
                            }

                          }

                          else
                          {
                            Debug.LogWarning("Cannot find the item with id " + condition.itemId + " to check.");
                            condition.proceed = false;
                          }

                          break;
                        case Condition.Type.GlobalFlagEqualTo:
                          var flag = DiplomataData.globalFlags.Find(DiplomataData.globalFlags.flags, condition.globalFlag.name);

                          if (flag != null)
                          {

                            if (flag.value == condition.globalFlag.value)
                            {
                              condition.proceed = true;
                            }

                            else
                            {
                              condition.proceed = false;
                            }

                          }

                          else
                          {
                            Debug.LogWarning("Cannot find the custom flag " + condition.globalFlag.name);
                            condition.proceed = false;
                          }

                          break;
                      }

                      if (!condition.custom.CheckAll())
                      {
                        condition.proceed = false;
                      }
                    }

                    proceed = Condition.CanProceed(msg.conditions);
                  }

                  var lastMsg = currentColumn.messages.Length - 1;

                  if (proceed)
                  {
                    if (msg.isAChoice)
                    {
                      choiceMenu = true;
                      choices.Add(msg);

                      if (controlIndexes["message"] < lastMsg)
                      {
                        controlIndexes["message"] += 1;
                        Next(false);
                      }
                    }

                    else if (choices.Count == 0)
                    {
                      currentMessage = msg;
                      OnStartCallbacks();
                    }

                    else if (controlIndexes["message"] < lastMsg)
                    {
                      controlIndexes["message"] += 1;
                      Next(false);
                    }

                    else
                    {
                      choiceMenu = true;
                    }
                  }

                  else if (controlIndexes["message"] == lastMsg)
                  {

                    if (IsLastMessage())
                    {
                      EndTalk();
                    }

                    else
                    {
                      controlIndexes["column"] += 1;
                      controlIndexes["message"] = 0;
                      Next(false);
                    }

                  }

                  else
                  {
                    controlIndexes["message"] += 1;
                    Next(false);
                  }
                }
              }

              else
              {
                controlIndexes["column"] += 1;
                controlIndexes["message"] = 0;
                Next(false);
              }
            }
          }

          else
          {
            EndTalk();
          }

        }

        else
        {
          EndTalk();
        }
      }

      else
      {
        Debug.LogError("Unable to found current context, this character or interactable don't have contexts or you need to use StartTalk() method to start the conversation.");
        EndTalk();
      }
    }

    public string ShowMessageContentSubtitle()
    {
      if (DiplomataData.isTalking)
      {
        if (currentMessage != null)
        {
          var talkLog = TalkLog.Find(DiplomataData.gameProgress.talkLog, talkable.name);

          if (talkLog == null)
          {
            DiplomataData.gameProgress.talkLog = ArrayHelper.Add(DiplomataData.gameProgress.talkLog, new TalkLog(talkable.name));
            talkLog = TalkLog.Find(DiplomataData.gameProgress.talkLog, talkable.name);
          }

          talkLog.messagesIds = ArrayHelper.Add(talkLog.messagesIds, currentMessage.GetUniqueId());

          return DictionariesHelper.ContainsKey(currentMessage.content, DiplomataData.gameProgress.options.currentSubtitledLanguage).value;
        }

        else
        {
          var errorText = "Current message to show is not setted.";
          Debug.LogError(errorText);
          EndTalk();
          return errorText;
        }
      }

      Debug.Log("Empty string returned.");
      return string.Empty;
    }

    public void PlayMessageAudioContent()
    {
      if (currentMessage != null)
      {
        var audioClipPath = DictionariesHelper.ContainsKey(currentMessage.audioClipPath, DiplomataData.gameProgress.options.currentDubbedLanguage);

        if (audioClipPath.value != string.Empty)
        {
          if (currentMessage.audioClip == null)
          {
            currentMessage.audioClip = (AudioClip) Resources.Load(audioClipPath.value);

            if (currentMessage.audioClip == null)
            {
              Debug.LogError("This audio clip doesn't exist or is not in the Resources folder.");
            }
          }

          if (currentMessage.audioClip != null)
          {
            var audioSource = GetComponent<AudioSource>();

            if (audioSource != null)
            {
              audioSource.PlayOneShot(currentMessage.audioClip, DiplomataData.gameProgress.options.volumeScale);
            }

            else
            {
              Debug.LogWarning("You have a audio clip in this message, but the game object don't have a AudioSource.");
            }
          }
        }
      }
    }

    public void StopMessageAudioContent()
    {
      if (currentMessage != null)
      {
        var audioClipPath = DictionariesHelper.ContainsKey(currentMessage.audioClipPath, DiplomataData.gameProgress.options.currentDubbedLanguage);

        if (audioClipPath.value != string.Empty)
        {

          var audioSource = GetComponent<AudioSource>();

          if (audioSource != null)
          {
            audioSource.Stop();
          }

          else
          {
            Debug.LogWarning("You have a audio clip in this message, but the game object don't have a AudioSource.");
          }
        }
      }
    }

    public void SetAnimatorAttributes()
    {
      if (currentMessage != null)
      {
        if (currentMessage.animatorAttributesSetters.Length > 0)
        {
          var animators = FindObjectsOfType(typeof(Animator));

          foreach (Animator animator in animators)
          {

            if (animator.runtimeAnimatorController != null)
            {

              foreach (AnimatorAttributeSetter animatorAttribute in currentMessage.animatorAttributesSetters)
              {

                if (animatorAttribute.animator == null)
                {
                  animatorAttribute.animator = (RuntimeAnimatorController) Resources.Load(animatorAttribute.animatorPath);
                }

                if (animatorAttribute.animator != null)
                {

                  if (animator.runtimeAnimatorController.Equals(animatorAttribute.animator))
                  {

                    switch (animatorAttribute.type)
                    {

                      case AnimatorControllerParameterType.Bool:
                        animator.SetBool(animatorAttribute.name, animatorAttribute.setBool);
                        break;

                      case AnimatorControllerParameterType.Float:
                        animator.SetFloat(animatorAttribute.name, animatorAttribute.setFloat);
                        break;

                      case AnimatorControllerParameterType.Int:
                        animator.SetInteger(animatorAttribute.name, animatorAttribute.setInt);
                        break;

                      case AnimatorControllerParameterType.Trigger:
                        animator.SetTrigger(animatorAttribute.name);
                        break;

                    }

                  }

                }

                else
                {
                  Debug.LogError("Animator controller is not setted in message.");
                }

              }

            }

          }

        }

      }
    }

    public void ResetAnimators()
    {
      var animators = FindObjectsOfType(typeof(Animator));

      foreach (Animator animator in animators)
      {
        if (animator.runtimeAnimatorController != null)
        {

          foreach (AnimatorAttributeSetter animatorAttribute in currentMessage.animatorAttributesSetters)
          {

            if (animatorAttribute.animator == null)
            {
              animatorAttribute.animator = (RuntimeAnimatorController) Resources.Load(animatorAttribute.animatorPath);
            }

            if (animatorAttribute.animator != null)
            {

              if (animator.runtimeAnimatorController.Equals(animatorAttribute.animator))
              {
                animator.Rebind();
              }

            }

            else
            {
              Debug.LogError("Animator controller is not setted in message.");
            }

          }

        }

      }

    }

    public void SwapStaticSprite(Vector2 pivot)
    {
      if (currentMessage != null)
      {
        if (currentMessage.imagePath != string.Empty)
        {
          var spriteRenderer = GetComponent<SpriteRenderer>();

          if (spriteRenderer != null)
          {
            spriteRenderer.sprite = currentMessage.GetSprite(pivot);
          }

          else
          {
            Debug.LogWarning("You have a static image in this message, but the game object don't have a Sprite Renderer.");
          }
        }
      }
    }

    public void SwapStaticSprite()
    {
      SwapStaticSprite(new Vector2(0.5f, 0.5f));
    }

    public bool IsLastMessage()
    {
      if (controlIndexes["column"] < currentContext.columns.Length - 1)
      {
        return false;
      }

      else
      {
        return true;
      }
    }

    public void EndTalk()
    {
      foreach (ITalk iTalk in DiplomataData.iTalks) iTalk.OnEnd();
      DiplomataData.isTalking = false;
    }

    public void NextMessage()
    {
      var hasFate = false;

      if (currentMessage != null)
      {
        controlIndexes["column"] = currentMessage.columnId;
        controlIndexes["message"] = currentMessage.id;
        lastUniqueId = currentMessage.GetUniqueId();

        foreach (Effect effect in currentMessage.effects)
        {
          switch (effect.type)
          {
            case Effect.Type.EndOfContext:
              if (effect.endOfContext.GetContext(DiplomataData.characters) != null)
                effect.endOfContext.GetContext(DiplomataData.characters).happened = true;
              
              if (effect.endOfContext.GetContext(DiplomataData.interactables) != null)
                effect.endOfContext.GetContext(DiplomataData.interactables).happened = true;

              if (currentContext.talkableName == effect.endOfContext.talkableName && currentContext.id == effect.endOfContext.contextId)
              {
                currentContext.happened = true;
              }

              break;

            case Effect.Type.GoTo:
              Message goToMsg = effect.goTo.GetMessage(currentContext);
              controlIndexes["column"] = goToMsg.columnId;
              controlIndexes["message"] = goToMsg.id;
              hasFate = true;
              break;

            case Effect.Type.SetAnimatorAttribute:
              var animators = FindObjectsOfType(typeof(Animator));

              foreach (Animator animator in animators)
              {

                if (animator.runtimeAnimatorController != null)
                {

                  if (effect.animatorAttributeSetter.animator == null)
                  {
                    effect.animatorAttributeSetter.animator = (RuntimeAnimatorController) Resources.Load(effect.animatorAttributeSetter.animatorPath);
                  }

                  if (effect.animatorAttributeSetter.animator != null)
                  {

                    if (animator.runtimeAnimatorController.Equals(effect.animatorAttributeSetter.animator))
                    {

                      switch (effect.animatorAttributeSetter.type)
                      {

                        case AnimatorControllerParameterType.Bool:
                          animator.SetBool(effect.animatorAttributeSetter.name, effect.animatorAttributeSetter.setBool);
                          break;

                        case AnimatorControllerParameterType.Float:
                          animator.SetFloat(effect.animatorAttributeSetter.name, effect.animatorAttributeSetter.setFloat);
                          break;

                        case AnimatorControllerParameterType.Int:
                          animator.SetInteger(effect.animatorAttributeSetter.name, effect.animatorAttributeSetter.setInt);
                          break;

                        case AnimatorControllerParameterType.Trigger:
                          animator.SetTrigger(effect.animatorAttributeSetter.name);
                          break;

                      }

                    }

                  }

                  else
                  {
                    Debug.LogError("Animator controller is not setted in message effect.");
                  }

                }

              }

              break;

            case Effect.Type.GetItem:
              var getItem = Item.Find(DiplomataData.inventory.items, effect.itemId);

              if (getItem != null)
              {
                getItem.have = true;
              }

              else
              {
                Debug.LogError("Cannot find the item with id " + effect.itemId + " to get.");
              }

              break;

            case Effect.Type.EquipItem:
              var equipItem = Item.Find(DiplomataData.inventory.items, effect.itemId);

              if (equipItem != null)
              {
                DiplomataData.inventory.Equip(effect.itemId);
              }

              else
              {
                Debug.LogError("Cannot find the item with id " + effect.itemId + " to equip.");
              }

              break;

            case Effect.Type.DiscardItem:
              var discardItem = Item.Find(DiplomataData.inventory.items, effect.itemId);

              if (discardItem != null)
              {
                discardItem.discarded = true;
              }

              else
              {
                Debug.LogError("Cannot find the item with id " + effect.itemId + " to discard.");
              }

              break;

            case Effect.Type.SetGlobalFlag:
              var flag = DiplomataData.globalFlags.Find(DiplomataData.globalFlags.flags, effect.globalFlag.name);

              if (flag != null)
              {
                flag.value = effect.globalFlag.value;
              }

              else
              {
                Debug.LogError("Cannot find the custom flag " + effect.globalFlag.name);
              }

              break;
          }

          effect.onComplete.Invoke();
        }

        if (currentMessage.disposable)
        {
          currentMessage.alreadySpoked = true;
        }
      }

      if (hasFate)
      {
        Next(true);
      }

      else if (IsLastMessage())
      {
        EndTalk();
      }

      else
      {
        controlIndexes["column"] += 1;
        controlIndexes["message"] = 0;
        Next(false);
      }
    }

    public List<string> MessageChoices()
    {
      List<string> choicesText = new List<string>();

      if (choices.Count > 0)
      {
        foreach (Message choice in choices)
        {
          var content = DictionariesHelper.ContainsKey(choice.content, DiplomataData.gameProgress.options.currentSubtitledLanguage).value;
          var choiceText = content; //(shortContent != "") ? shortContent : content;

          if (!choice.alreadySpoked && choice.disposable)
          {
            choicesText.Add(choiceText);
          }
          else if (!choice.disposable)
          {
            choicesText.Add(choiceText);
          }
        }
      }

      else
      {
        Debug.Log("There's no choice this time.");

        if (IsLastMessage())
        {
          EndTalk();
        }

        else
        {
          controlIndexes["column"] += 1;
          controlIndexes["message"] = 0;
          Next(false);
        }
      }

      return choicesText;
    }

    public void OnStartCallbacks()
    {
      if (currentMessage != null)
      {
        foreach (Effect effect in currentMessage.effects)
        {
          effect.onStart.Invoke();
        }
      }
    }

    public bool EmitterIsPlayer()
    {
      if (currentMessage != null)
      {
        if (currentColumn.emitter == DiplomataData.options.playerCharacterName)
        {
          return true;
        }
      }

      return false;
    }

    public string PlayerName()
    {
      return DiplomataData.options.playerCharacterName;
    }

    public string Emitter()
    {
      if (currentColumn != null)
      {
        return currentColumn.emitter;
      }

      return null;
    }

    public Message GetLastMessage()
    {
      foreach (Column col in currentContext.columns)
      {
        if (Message.Find(col.messages, lastUniqueId) != null)
        {
          return Message.Find(col.messages, lastUniqueId);
        }
      }

      return null;
    }

    public string GetLastMessageContent()
    {
      if (lastUniqueId == null || lastUniqueId == "")
      {
        return "";
      }

      return DictionariesHelper.ContainsKey(GetLastMessage().content,
        DiplomataData.gameProgress.options.currentSubtitledLanguage).value;
    }
  }
}
