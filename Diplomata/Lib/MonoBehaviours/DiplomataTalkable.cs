using System;
using System.Collections.Generic;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
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
    public List<Message> choices;
    public Talkable talkable;
    public Column currentColumn;
    public Message currentMessage;
    public bool choiceMenu;
    private Context currentContext;
    protected Dictionary<string, int> controlIndexes;
    private string lastUniqueId;

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

    /// <summary>
    /// Start the talk
    /// </summary>
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

    /// <summary>
    /// Internal method to go the next message
    /// </summary>
    /// <param name="hasFate">is true if use GoTo effect</param>
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
                        case Condition.Type.QuestStateIs:
                          var quest = Quest.Find(DiplomataData.quests, condition.questAndState.questId);

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
                              Debug.Log(string.Format("Invalid quest state id: {0}.", condition.questAndState.questStateId));
                              condition.proceed = false;
                            }
                            else if (currentState == targetState.Name)
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
        Debug.LogError("Unable to found current context, this character or interactable don't have contexts or you need to use StartTalk() method to start the conversation.");
        EndTalk();
      }
    }

    /// <summary>
    /// Returns the text of the current message.
    /// </summary>
    /// <returns>The current message.</returns>
    public string ShowMessageContentSubtitle()
    {
      if (DiplomataData.isTalking)
      {
        if (currentMessage != null)
        {
          // Save in the talk log.
          var talkLog = TalkLog.Find(DiplomataData.talkLogs, talkable.name);
          if (talkLog != null)
          {
            talkLog.messagesIds = ArrayHelper.Add(talkLog.messagesIds, currentMessage.GetUniqueId());
          }

          // Return the text.
          return DictionariesHelper.ContainsKey(currentMessage.content, DiplomataData.options.currentLanguage).value;
        }
        else
        {
          var errorText = "Current message to show is not setted.";
          Debug.LogError(errorText);
          EndTalk();
          return errorText;
        }
      }
      return string.Empty;
    }

    /// <summary>
    /// Play the current audio of the current message
    /// </summary>
    public void PlayMessageAudioContent()
    {
      if (currentMessage != null)
      {
        var audioClipPath = DictionariesHelper.ContainsKey(currentMessage.audioClipPath, DiplomataData.options.currentLanguage);

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
              audioSource.PlayOneShot(currentMessage.audioClip, DiplomataData.options.volumeScale);
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
    /// Stop the current audio of the current message
    /// </summary>
    public void StopMessageAudioContent()
    {
      if (currentMessage != null)
      {
        var audioClipPath = DictionariesHelper.ContainsKey(currentMessage.audioClipPath, DiplomataData.options.currentLanguage);

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
    /// Apply the current message animator attributes
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

    /// <summary>
    /// Reset the animator attributes
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
    /// Change the sprite of the current message
    /// </summary>
    /// <param name="pivot"></param>
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

    /// <summary>
    /// Change the sprite of the current message
    /// </summary>
    public void SwapStaticSprite()
    {
      SwapStaticSprite(new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// Return if the current message is the last
    /// </summary>
    /// <returns></returns>
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
    /// Internally end the talk
    /// </summary>
    public void EndTalk()
    {
      DiplomataData.isTalking = false;
    }

    /// <summary>
    /// The method need to go the next message
    /// </summary>
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
              if (effect.endOfContext.GetContext(DiplomataData.characters, DiplomataData.interactables) != null)
                effect.endOfContext.GetContext(DiplomataData.characters, DiplomataData.interactables).happened = true;

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

            case Effect.Type.EndOfDialogue:
              EndTalk();
              break;
            case Effect.Type.SetQuestState:
              var quest = Quest.Find(DiplomataData.quests, effect.questAndState.questId);

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
                }
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
    /// Get all the choices in a list
    /// </summary>
    /// <returns>A list with all the choices of the current column</returns>
    public List<string> MessageChoices()
    {
      List<string> choicesText = new List<string>();

      if (choices.Count > 0)
      {
        foreach (Message choice in choices)
        {
          var content = DictionariesHelper.ContainsKey(choice.content, DiplomataData.options.currentLanguage).value;
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

    /// <summary>
    /// Execute the onStart events of the effect
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
    /// Check if the emitter is the player character
    /// </summary>
    /// <returns>The response flag</returns>
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

    /// <summary>
    /// Get the player name
    /// </summary>
    /// <returns>the player name</returns>
    public string PlayerName()
    {
      return DiplomataData.options.playerCharacterName;
    }

    /// <summary>
    /// Get the current message emitter
    /// </summary>
    /// <returns>The name of the emitter</returns>
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
        DiplomataData.options.currentLanguage).value;
    }
  }
}
