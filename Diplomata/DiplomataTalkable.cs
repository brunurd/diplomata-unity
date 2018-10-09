using System;
using System.Collections.Generic;
using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using LavaLeak.Diplomata.Models.Submodels;
using UnityEngine;

namespace LavaLeak.Diplomata
{
  /// <summary>
  /// The main class of the talkable.
  /// </summary>
  [AddComponentMenu("")]
  [Serializable]
  [DisallowMultipleComponent]
  public class DiplomataTalkable : MonoBehaviour
  {
    public Talkable talkable;
    public Column currentColumn;
    public Message currentMessage;
    public bool choiceMenu;
    public bool IsTalking;
    public List<Message> choices;

    protected Context currentContext;
    protected Dictionary<string, int> controlIndexes;
    private string lastUniqueId;

    // Events.
    public event Action<Context> OnContextEnd;
    public event Action<Message> OnMessageEnd;
    public event Action<Item> OnItemWasCaughtLocal;
    public event Action<Quest> OnQuestStartLocal;
    public event Action<Quest> OnQuestStateChangeLocal;
    public event Action<Quest> OnQuestEndLocal;

    /// <summary>
    /// Set if the talkable is on scene.
    /// </summary>
    private void OnEnable()
    {
      if (talkable != null)
      {
        talkable.onScene = true;
      }
    }

    /// <summary>
    /// Set to false the field onScene.
    /// </summary>
    private void OnDisable()
    {
      if (talkable != null)
      {
        talkable.onScene = false;
      }
    }

    /// <summary>
    /// Set to false the field onScene.
    /// </summary>
    private void OnDestroy()
    {
      if (talkable != null)
      {
        talkable.onScene = false;
      }
    }

    /// <summary>
    /// Start the talk.
    /// </summary>
    public void StartTalk()
    {
      if (IsTalking || DiplomataManager.OnATalk)
      {
        Debug.LogWarning("This talk cannot start, because the player is already in one.");
        return;
      }

      if (talkable != null)
      {
        IsTalking = true;
        DiplomataManager.OnATalk = true;
        controlIndexes = new Dictionary<string, int>();
        controlIndexes.Add("context", 0);
        controlIndexes.Add("column", 0);
        controlIndexes.Add("message", 0);
        controlIndexes.Add("content", -1);

        currentContext = null;
        currentColumn = null;
        currentMessage = null;

        choices = new List<Message>();

        for (controlIndexes["context"] = 0;
          controlIndexes["context"] < talkable.contexts.Length;
          controlIndexes["context"]++)
        {
          var context = Context.Find(talkable, controlIndexes["context"]);
          var lastContext = talkable.contexts.Length - 1;

          if (context != null)
          {
            if (!context.Finished)
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
          Debug.LogWarning("No context found in " + talkable.name + ".");
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

    /// <summary>
    /// Internal method to go the next message.
    /// </summary>
    /// <param name="hasFate">is true if use GoTo effect.</param>
    protected void Next(bool hasFate)
    {
      if (talkable != null && currentContext != null)
      {
        if (!currentContext.Finished)
        {
          currentColumn = Column.Find(currentContext, controlIndexes["column"]);

          if (currentColumn == null)
          {
            for (controlIndexes["column"] = controlIndexes["column"];
              controlIndexes["column"] < talkable.contexts.Length;
              controlIndexes["column"]++)
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
                          if (talkable.GetType() == typeof(Character))
                          {
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
                          if (talkable.GetType() == typeof(Character))
                          {
                            Character character = (Character) talkable;
                            if (character.influence <= condition.comparedInfluence)
                            {
                              condition.proceed = false;
                            }
                          }

                          break;

                        case Condition.Type.InfluenceLessThan:
                          if (talkable.GetType() == typeof(Character))
                          {
                            Character character = (Character) talkable;
                            if (character.influence >= condition.comparedInfluence)
                            {
                              condition.proceed = false;
                            }
                          }

                          break;
                        case Condition.Type.HasItem:
                          var item = Item.Find(DiplomataManager.Data.inventory.items, condition.itemId);

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
                          var itemDont = Item.Find(DiplomataManager.Data.inventory.items, condition.itemId);

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
                          var equippedItem = Item.Find(DiplomataManager.Data.inventory.items, condition.itemId);
                          if (equippedItem != null)
                          {
                            if (DiplomataManager.Data.inventory.IsEquipped(condition.itemId))
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
                          var discardedItem = Item.Find(DiplomataManager.Data.inventory.items, condition.itemId);

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
                          var flag = DiplomataManager.Data.globalFlags.Find(DiplomataManager.Data.globalFlags.flags,
                            condition.globalFlag.name);

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
                        case Condition.Type.QuestStateIs:
                          var quest = Quest.Find(DiplomataManager.Data.quests, condition.questAndState.questId);

                          if (quest != null)
                          {
                            var currentState = quest.GetCurrentState();
                            var targetState = quest.GetState(condition.questAndState.questStateId);
                            if (currentState == null)
                            {
                              Debug.Log(string.Format("Probably quest {0} is not initialized.", quest.Name));
                              condition.proceed = false;
                            }
                            else if (targetState == null)
                            {
                              Debug.Log(string.Format("Invalid quest state id: {0}.",
                                condition.questAndState.questStateId));
                              condition.proceed = false;
                            }
                            else if (currentState == targetState.ShortDescription)
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
                            Debug.LogWarning("Cannot find the quest " + condition.questAndState.questId);
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
        Debug.LogError(
          "Unable to found current context, this character or interactable don't have contexts or you need to use StartTalk() method to start the conversation.");
        EndTalk();
      }
    }

    /// <summary>
    /// Returns the text of the current message.
    /// </summary>
    /// <returns>The current message.</returns>
    public string ShowMessageContentSubtitle()
    {
      if (IsTalking && DiplomataManager.OnATalk)
      {
        if (currentMessage != null)
        {
          // Save in the talk log.
          var talkLog = TalkLog.Find(DiplomataManager.Data.talkLogs, talkable.name);
          if (talkLog != null)
          {
            talkLog.messagesIds = ArrayHelper.Add(talkLog.messagesIds, currentMessage.GetUniqueId());
          }

          // Return the text.
          var content = new LanguageDictionary();

          // First of all return the normal content.
          if (controlIndexes["content"] < 0)
            content = DictionariesHelper.ContainsKey(currentMessage.content,
              DiplomataManager.Data.options.currentLanguage);

          // If have any attached content return that.
          else if (currentMessage.attachedContent.Length > 0 &&
                   controlIndexes["content"] < currentMessage.attachedContent.Length)
          {
            var attachedContent = currentMessage.attachedContent[controlIndexes["content"]];
            if (attachedContent != null)
              content = DictionariesHelper.ContainsKey(attachedContent.content,
                DiplomataManager.Data.options.currentLanguage);
          }

          // If has a error return a error text.
          if (content == null)
          {
            var errorText = string.Format("Cannot find message content in the language: {0}",
              DiplomataManager.Data.options.currentLanguage);
            Debug.LogError(errorText);
            EndTalk();
            return errorText;
          }

          return currentContext.ReplaceVariables(content.value);
        }
        else
        {
          var errorText = "Current message to show is not setted.";
          Debug.LogError(errorText);
          EndTalk();
          return errorText;
        }
      }

      EndTalk();
      return string.Empty;
    }

    /// <summary>
    /// Play the current audio of the current message.
    /// </summary>
    public void PlayMessageAudioContent()
    {
      if (currentMessage != null)
      {
        var audioClipPath = DictionariesHelper.ContainsKey(currentMessage.audioClipPath,
          DiplomataManager.Data.options.currentLanguage);

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
              audioSource.PlayOneShot(currentMessage.audioClip, DiplomataManager.Data.options.volumeScale);
            }

            else
            {
              Debug.LogWarning("You have a audio clip in this message, but the game object don't have a AudioSource.");
            }
          }
        }
      }
    }

    /// <summary>
    /// Stop the current audio of the current message.
    /// </summary>
    public void StopMessageAudioContent()
    {
      if (currentMessage != null)
      {
        var audioClipPath = DictionariesHelper.ContainsKey(currentMessage.audioClipPath,
          DiplomataManager.Data.options.currentLanguage);

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

    /// <summary>
    /// Apply the current message animator attributes.
    /// </summary>
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
                  animatorAttribute.animator =
                    (RuntimeAnimatorController) Resources.Load(animatorAttribute.animatorPath);
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

    /// <summary>
    /// Reset the animator attributes.
    /// </summary>
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

    /// <summary>
    /// Change the sprite of the current message.
    /// </summary>
    /// <param name="pivot">The GetSprite pivot from image.</param>
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
            Debug.LogWarning(
              "You have a static image in this message, but the game object don't have a Sprite Renderer.");
          }
        }
      }
    }

    /// <summary>
    /// Change the sprite of the current message.
    /// </summary>
    public void SwapStaticSprite()
    {
      SwapStaticSprite(new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// Return if the current message is the last.
    /// </summary>
    /// <returns>True if is the last message.</returns>
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

    /// <summary>
    /// Internally end the talk.
    /// </summary>
    public void EndTalk()
    {
      DiplomataManager.OnATalk = false;
      IsTalking = false;
    }

    /// <summary>
    /// The method need to go the next message.
    /// </summary>
    public void NextMessage()
    {
      // If has any attached content go to that.
      if (currentMessage.attachedContent.Length > 0 &&
          controlIndexes["content"] < currentMessage.attachedContent.Length - 1)
      {
        controlIndexes["content"]++;
        return;
      }
      controlIndexes["content"] = -1;

      var hasFate = false;

      if (currentMessage != null)
      {
        if (OnMessageEnd != null)
          OnMessageEnd(currentMessage);

        controlIndexes["column"] = currentMessage.columnId;
        controlIndexes["message"] = currentMessage.id;
        lastUniqueId = currentMessage.GetUniqueId();

        foreach (var effect in currentMessage.effects)
        {
          switch (effect.type)
          {
            case Effect.Type.EndOfContext:
              if (effect.endOfContext.GetContext(DiplomataManager.Data.characters,
                    DiplomataManager.Data.interactables) != null)
              {
                effect.endOfContext.GetContext(DiplomataManager.Data.characters, DiplomataManager.Data.interactables)
                  .Finished = true;
                if (OnContextEnd != null)
                  OnContextEnd(currentContext);
              }

              if (currentContext.talkableName == effect.endOfContext.talkableName &&
                  currentContext.id == effect.endOfContext.contextId)
              {
                currentContext.Finished = true;
                if (OnContextEnd != null)
                  OnContextEnd(currentContext);
              }

              break;

            case Effect.Type.GoTo:
              var goToMsg = effect.goTo.GetMessage(currentContext);
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
                    effect.animatorAttributeSetter.animator =
                      (RuntimeAnimatorController) Resources.Load(effect.animatorAttributeSetter.animatorPath);
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
                          animator.SetFloat(effect.animatorAttributeSetter.name,
                            effect.animatorAttributeSetter.setFloat);
                          break;

                        case AnimatorControllerParameterType.Int:
                          animator.SetInteger(effect.animatorAttributeSetter.name,
                            effect.animatorAttributeSetter.setInt);
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
              var getItem = Item.Find(DiplomataManager.Data.inventory.items, effect.itemId);
              if (getItem != null)
              {
                getItem.have = true;

                DiplomataManager.EventController.SendItemWasCaught(getItem);

                if (OnItemWasCaughtLocal != null)
                  OnItemWasCaughtLocal(getItem);
              }
              else
              {
                Debug.LogError("Cannot find the item with id " + effect.itemId + " to get.");
              }

              break;

            case Effect.Type.EquipItem:
              var equipItem = Item.Find(DiplomataManager.Data.inventory.items, effect.itemId);
              if (equipItem != null)
              {
                DiplomataManager.Data.inventory.Equip(effect.itemId);
              }
              else
              {
                Debug.LogError("Cannot find the item with id " + effect.itemId + " to equip.");
              }

              break;

            case Effect.Type.DiscardItem:
              var discardItem = Item.Find(DiplomataManager.Data.inventory.items, effect.itemId);
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
              var flag = DiplomataManager.Data.globalFlags.Find(DiplomataManager.Data.globalFlags.flags,
                effect.globalFlag.name);
              if (flag != null)
              {
                flag.value = effect.globalFlag.value;
              }
              else
              {
                Debug.LogError("Cannot find the custom flag " + effect.globalFlag.name);
              }

              break;

            case Effect.Type.EndOfDialogue:
              EndTalk();
              break;

            case Effect.Type.SetQuestState:
              var quest = Quest.Find(DiplomataManager.Data.quests, effect.questAndState.questId);
              if (quest != null)
              {
                var currentState = quest.GetCurrentState();
                var targetState = quest.GetState(effect.questAndState.questStateId);
                if (currentState == null)
                {
                  Debug.Log(string.Format("Probably quest {0} is not initialized.", quest.Name));
                }
                else if (targetState == null)
                {
                  Debug.Log(string.Format("Invalid quest state id: {0}.", effect.questAndState.questStateId));
                }
                else
                {
                  quest.SetState(effect.questAndState.questStateId);
                  DiplomataManager.EventController.SendQuestStateChange(quest);

                  if (OnQuestStateChangeLocal != null)
                    OnQuestStateChangeLocal(quest);
                }
              }
              else
              {
                Debug.LogWarning("Cannot find the quest " + effect.questAndState.questId);
              }

              break;

            case Effect.Type.FinishQuest:
              var questToFinish = Quest.Find(DiplomataManager.Data.quests, effect.questAndState.questId);
              if (questToFinish != null)
              {
                questToFinish.Finish();
                DiplomataManager.EventController.SendQuestEnd(questToFinish);

                if (OnQuestEndLocal != null)
                  OnQuestEndLocal(questToFinish);
              }
              else
              {
                Debug.LogWarning("Cannot find the quest " + effect.questAndState.questId);
              }

              break;

            case Effect.Type.StartQuest:
              var questToStart = Quest.Find(DiplomataManager.Data.quests, effect.questAndState.questId);
              if (questToStart != null)
              {
                if (questToStart.Initialized)
                  break;

                questToStart.Initialize();

                DiplomataManager.EventController.SendQuestStart(questToStart);

                if (OnQuestStartLocal != null)
                  OnQuestStartLocal(questToStart);
              }
              else
              {
                Debug.LogWarning("Cannot find the quest " + effect.questAndState.questId);
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

    /// <summary>
    /// Execute the onStart events of the effect.
    /// </summary>
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

    /// <summary>
    /// Check if the emitter is the player character.
    /// </summary>
    /// <returns>The response flag.</returns>
    public bool EmitterIsPlayer()
    {
      if (currentMessage != null)
      {
        if (currentColumn.emitter == DiplomataManager.Data.options.playerCharacterName)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Get the player name.
    /// </summary>
    /// <returns>the player name.</returns>
    public string PlayerName()
    {
      return DiplomataManager.Data.options.playerCharacterName;
    }

    /// <summary>
    /// Get the current message emitter.
    /// </summary>
    /// <returns>The name of the emitter.</returns>
    public string Emitter()
    {
      if (currentColumn != null)
      {
        return currentColumn.emitter;
      }

      return null;
    }

    /// <summary>
    /// Get the last message before the current.
    /// </summary>
    /// <returns>The <seealso cref="Diplomata.Models.Message"> object.</returns>
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

    /// <summary>
    /// Get the last message content.
    /// </summary>
    /// <returns>The last message text content.</returns>
    public string GetLastMessageContent()
    {
      if (lastUniqueId == null || lastUniqueId == "")
      {
        return "";
      }

      var lastMessage = GetLastMessage();
      var content = DictionariesHelper.ContainsKey(lastMessage.content,
        DiplomataManager.Data.options.currentLanguage).value;

      if (lastMessage.attachedContent.Length == 0)
        return content;

      return DictionariesHelper.ContainsKey(lastMessage.attachedContent[lastMessage.attachedContent.Length - 1].content,
        DiplomataManager.Data.options.currentLanguage).value;
    }
  }
}