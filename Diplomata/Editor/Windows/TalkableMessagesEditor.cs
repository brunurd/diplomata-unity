using System;
using System.Collections.Generic;
using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Extensions;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using LavaLeak.Diplomata.Models.Collections;
using LavaLeak.Diplomata.Models.Submodels;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class TalkableMessagesEditor : UnityEditor.EditorWindow
  {
    private ushort iteractions = 0;

    // Models.
    public static Talkable talkable;
    public static Context context;
    private Options options;
    private List<Character> characters;
    private static List<Interactable> interactables;
    private Inventory inventory;
    private Quest[] quests;
    private GlobalFlags globalFlags;

    // Layout elements.
    public static Texture2D headerBG;
    public static Texture2D mainBG;
    public static Texture2D sidebarBG;
    public static Texture2D textAreaBGTextureNormal;
    public static Texture2D textAreaBGTextureFocused;

    // Context list scroll pos.
    private static Vector2 contextScrollPos = new Vector2(0, 0);

    // Messages consts.
    private const byte HEADER_HEIGHT = GUIHelper.BUTTON_HEIGHT_SMALL + (2 * GUIHelper.MARGIN);
    private const byte LABEL_HEIGHT = HEADER_HEIGHT + 15;
    private const ushort SIDEBAR_WIDTH = 300;

    // Messages lists.
    private static string[] messageList = new string[0];
    private static string[] characterList = new string[0];
    private static string[] contextList = new string[0];
    private static string[] itemList = new string[0];
    private static string[] globalFlagsList = new string[0];
    private static string[] labelsList = new string[0];
    private static string[] booleanArray = new string[] {"True", "False"};

    // Messages layout fields.
    public static GUIStyle messagesWindowHeaderStyle = new GUIStyle(GUIHelper.windowStyle);
    public static GUIStyle messagesWindowMainStyle = new GUIStyle(GUIHelper.windowStyle);
    public static GUIStyle messagesWindowSidebarStyle = new GUIStyle(GUIHelper.windowStyle);
    public static GUIStyle textAreaStyle = new GUIStyle(GUIHelper.textAreaStyle);
    public static GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
    public static GUIStyle fakeButtonStyle = new GUIStyle(GUI.skin.button);
    public static Color inactiveColor = new Color(0, 0, 0, 0.6f);
    private static Color proColor = new Color(0.2196f, 0.2196f, 0.2196f);
    private static Color defaultColor = new Color(0.9764f, 0.9764f, 0.9764f);
    private static Vector2 scrollPosMain = new Vector2(0, 0);
    private static Vector2 scrollPosSidebar = new Vector2(0, 0);
    private static Vector2 scrollPosLabelManager = new Vector2(0, 0);
    private static Message message;

    // Window states.
    public enum State
    {
      None = 0,
      Context = 1,
      Messages = 2,
      Close = 3
    }

    public static State state;

    public static void Init(State state = State.None)
    {
      TalkableMessagesEditor window =
        (TalkableMessagesEditor) GetWindow(typeof(TalkableMessagesEditor), false, "Messages", true);
      window.minSize = new Vector2(370, 300);

      TalkableMessagesEditor.state = state;

      if (state == State.Close)
      {
        window.Close();
      }

      else
      {
        window.Show();
      }
    }

    public void OnEnable()
    {
      options = OptionsController.GetOptions();
      characters = CharactersController.GetCharacters(options);
      interactables = InteractablesController.GetInteractables(options);
      inventory = InventoryController.GetInventory(options.jsonPrettyPrint);
      quests = QuestsController.GetQuests(options.jsonPrettyPrint);
      globalFlags = GlobalFlagsController.GetGlobalFlags(options.jsonPrettyPrint);
    }

    public void OnDisable()
    {
      if (talkable != null)
      {
        Save();
      }
    }

    #region Main Window

    public void SetTextures()
    {
      var baseColor = ColorHelper.ResetColor();

      if (headerBG == null)
      {
        headerBG = GUIHelper.UniformColorTexture(1, 1, ColorHelper.ColorAdd(baseColor, 0.05f, 0.05f, 0.05f));
      }

      if (mainBG == null)
      {
        mainBG = GUIHelper.UniformColorTexture(1, 1, baseColor);
      }

      if (sidebarBG == null)
      {
        sidebarBG = GUIHelper.UniformColorTexture(1, 1, ColorHelper.ColorAdd(baseColor, -0.03333f));
      }

      if (textAreaBGTextureNormal == null)
      {
        textAreaBGTextureNormal = GUIHelper.UniformColorTexture(1, 1, new Color(0.4f, 0.4f, 0.4f, 0.08f));
      }

      if (textAreaBGTextureFocused == null)
      {
        textAreaBGTextureFocused = GUIHelper.UniformColorTexture(1, 1, new Color(1, 1, 1, 1));
      }
    }

    public static void OpenContextMenu(Talkable currentTalkable)
    {
      talkable = currentTalkable;
      Init(State.Context);
    }

    public static void OpenMessagesMenu(Talkable currentTalkable, Context currentContext)
    {
      talkable = currentTalkable;
      context = currentContext;
      if (context.talkableName == null || context.talkableName == string.Empty)
      {
        context.talkableName = talkable.name;
        StaticSave(talkable);
      }

      Init(State.Messages);
    }

    public static void Reset(string talkableName)
    {
      if (talkable != null)
      {
        if (talkable.name == talkableName)
        {
          talkable = null;
          Init(State.Close);
        }
      }
    }

    public void OnGUI()
    {
      GUIHelper.Init();
      SetTextures();

      if (talkable == null) Init(State.Close);

      switch (state)
      {
        case State.None:
          Init(State.Close);
          break;

        case State.Context:
          DrawContextList();
          break;

        case State.Messages:
          if (context == null) Init(State.Close);
          DrawMessagesEditor();
          break;
      }

      AutoSave();
    }

    private void Save()
    {
      if (talkable.GetType() == typeof(Character))
      {
        CharactersController.Save((Character) talkable, options.jsonPrettyPrint);
      }
      else if (talkable.GetType() == typeof(Interactable))
      {
        InteractablesController.Save((Interactable) talkable, options.jsonPrettyPrint);
      }
    }

    private static void StaticSave(Talkable talkable)
    {
      var options = OptionsController.GetOptions();
      if (talkable.GetType() == typeof(Character))
      {
        CharactersController.Save((Character) talkable, options.jsonPrettyPrint);
      }
      else if (talkable.GetType() == typeof(Interactable))
      {
        InteractablesController.Save((Interactable) talkable, options.jsonPrettyPrint);
      }
    }

    private void AutoSave()
    {
      if (iteractions == 100 && talkable != null)
      {
        Save();
        iteractions = 0;
      }

      iteractions++;
    }

    #endregion

    #region Context Editor

    public void DrawContextList()
    {
      var listWidth = Screen.width / 3;

      if (talkable != null)
      {
        contextScrollPos = EditorGUILayout.BeginScrollView(contextScrollPos);

        GUILayout.BeginHorizontal();
        GUILayout.Space(listWidth);

        GUILayout.BeginVertical(GUIHelper.windowStyle, GUILayout.Width(listWidth));

        if (EditorGUIUtility.isProSkin)
        {
          GUIHelper.labelStyle.normal.textColor = GUIHelper.proTextColor;
        }

        else
        {
          GUIHelper.labelStyle.normal.textColor = GUIHelper.freeTextColor;
        }

        GUIHelper.labelStyle.fontSize = 24;
        GUIHelper.labelStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label(talkable.name, GUIHelper.labelStyle, GUILayout.Height(50));

        for (int i = 0; i < talkable.contexts.Length; i++)
        {
          var context = Context.Find(talkable, i);

          Rect boxRect = EditorGUILayout.BeginVertical(GUIHelper.boxStyle);
          GUI.Box(boxRect, GUIContent.none);

          var name = DictionariesHelper.ContainsKey(context.name, options.currentLanguage);

          if (name == null)
          {
            context.name = ArrayHelper.Add(context.name,
              new LanguageDictionary(options.currentLanguage, "Name [Change clicking on Edit]"));
            name = DictionariesHelper.ContainsKey(context.name, options.currentLanguage);
          }

          GUIHelper.labelStyle.fontSize = 11;
          GUIHelper.labelStyle.alignment = TextAnchor.UpperCenter;

          GUIHelper.textContent.text = "<size=13><i>" + name.value + "</i></size>\n";
          var height = GUIHelper.labelStyle.CalcHeight(GUIHelper.textContent, listWidth);

          GUILayout.Label(GUIHelper.textContent, GUIHelper.labelStyle, GUILayout.Width(listWidth),
            GUILayout.Height(height));

          GUILayout.BeginHorizontal();
          if (GUILayout.Button("Edit", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            ContextEditor.Edit(talkable, context);
          }

          if (GUILayout.Button("Edit Messages", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            OpenMessagesMenu(talkable, context);
          }

          if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            if (EditorUtility.DisplayDialog("Are you sure?", "All data inside this context will be lost forever.",
              "Yes", "No"))
            {
              ContextEditor.Reset(talkable.name);
              talkable.contexts = ArrayHelper.Remove(talkable.contexts, context);
              talkable.contexts = Context.ResetIDs(talkable, talkable.contexts);
              Save();
            }
          }

          if (GUILayout.Button("Move Up", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            if (context.id > 0)
            {
              var neighboorContext = Context.Find(talkable, context.id - 1);

              neighboorContext.id += 1;
              context.id -= 1;

              Save();
            }
          }

          if (GUILayout.Button("Move Down", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            if (context.id < talkable.contexts.Length - 1)
            {
              var neighboorContext = Context.Find(talkable, context.id + 1);
              neighboorContext.id -= 1;
              context.id += 1;

              Save();
            }
          }

          GUILayout.EndHorizontal();

          EditorGUILayout.EndVertical();

          EditorGUILayout.Separator();
        }

        if (GUILayout.Button("Add Context", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_BIG)))
        {
          CreateContext();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
      }

      else
      {
        GUILayout.BeginVertical(GUIHelper.windowStyle);
        EditorGUILayout.HelpBox("This characters doesn't exist anymore.", MessageType.Info);
        GUILayout.EndVertical();
      }
    }

    public void CreateContext()
    {
      talkable.contexts = ArrayHelper.Add(talkable.contexts, new Context(talkable.contexts.Length, talkable.name));
      Save();
    }

    #endregion

    public void DrawMessagesEditor()
    {
      if (context != null)
      {
        messagesWindowHeaderStyle.normal.background = TalkableMessagesEditor.headerBG;
        messagesWindowMainStyle.normal.background = TalkableMessagesEditor.mainBG;
        messagesWindowSidebarStyle.normal.background = TalkableMessagesEditor.sidebarBG;

        messagesWindowMainStyle.alignment = TextAnchor.UpperLeft;
        messagesWindowSidebarStyle.alignment = TextAnchor.UpperLeft;

        Header();
        GUILayout.BeginHorizontal();
        Main();
        Sidebar();
        GUILayout.EndHorizontal();
        LabelManager();
      }
      else
      {
        TalkableMessagesEditor.Init(State.Close);
      }
    }

    public void Header()
    {
      GUILayout.BeginHorizontal(messagesWindowHeaderStyle, GUILayout.Height(HEADER_HEIGHT));

      if (GUILayout.Button("< Back", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
      {
        Save();
        TalkableMessagesEditor.OpenContextMenu(talkable);
      }

      EditorGUILayout.Separator();

      if (talkable.GetType() == typeof(Character))
        GUILayout.Label("Character: " + talkable.name);

      if (talkable.GetType() == typeof(Interactable))
        GUILayout.Label("Interactable: " + talkable.name);

      EditorGUILayout.Separator();

      GUILayout.Label("Column Width: ");
      context.columnWidth = (ushort) EditorGUILayout.Slider(context.columnWidth, 116, 675);

      EditorGUILayout.Separator();

      GUILayout.Label("Font Size: ");
      context.fontSize = (ushort) EditorGUILayout.Slider(context.fontSize, 8, 36);

      GUIHelper.boxStyle.fontSize = context.fontSize;
      GUIHelper.labelStyle.fontSize = context.fontSize;
      textAreaStyle.fontSize = context.fontSize;

      EditorGUILayout.Separator();

      GUILayout.Label("Filters: ");
      context.idFilter = GUILayout.Toggle(context.idFilter, "Id ");
      context.conditionsFilter = GUILayout.Toggle(context.conditionsFilter, "Conditions ");
      context.contentFilter = GUILayout.Toggle(context.contentFilter, "Content ");
      context.effectsFilter = GUILayout.Toggle(context.effectsFilter, "Effects ");

      EditorGUILayout.Separator();

      if (GUILayout.Button("Save", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
      {
        Save();
      }

      GUILayout.EndHorizontal();
    }

    public void Main()
    {
      var width = Screen.width - SIDEBAR_WIDTH;

      ResetStyle();

      scrollPosMain = EditorGUILayout.BeginScrollView(scrollPosMain, messagesWindowMainStyle, GUILayout.Width(width));
      GUILayout.BeginHorizontal();

      for (int i = 0; i < context.columns.Length; i++)
      {
        Column column = context.columns[i];
        foreach (Column col in context.columns)
        {
          if (col.id == i)
          {
            column = col;
            break;
          }
        }

        GUILayout.BeginVertical(GUILayout.Width(context.columnWidth));

        GUILayout.Space(4);

        column.emitter = GUIHelper.Popup("Emitter: ", column.emitter, options.characterList);

        EditorGUILayout.Separator();

        for (int j = 0; j < column.messages.Length; j++)
        {
          Message currentMessage = column.messages[j];

          if (currentMessage.labelId == "")
          {
            currentMessage.labelId = context.labels[0].uniqueId;
          }

          var label = Label.Find(context.labels, currentMessage.labelId);

          foreach (Message msg in column.messages)
          {
            if (msg.id == j)
            {
              currentMessage = msg;
              break;
            }
          }

          label = label != null ? label : new Label();

          if (label.show)
          {
            Rect boxRect = EditorGUILayout.BeginVertical(GUIHelper.boxStyle);

            var color = (EditorGUIUtility.isProSkin) ? proColor : defaultColor;

            GUIHelper.strokeWidth = 1;

            if (context.currentMessage.columnId != -1 && context.currentMessage.rowId != -1)
            {
              if (context.currentMessage.columnId == currentMessage.columnId &&
                  context.currentMessage.rowId == currentMessage.id)
              {
                color = ColorHelper.ColorAdd(color, 0.1f);
                message = currentMessage;
                GUIHelper.strokeWidth = 3;
              }
            }

            GUIHelper.labelStyle.normal.textColor = Color.black;
            textAreaStyle.normal.textColor = Color.black;

            if (color.r * color.g * color.b < 0.07f)
            {
              GUIHelper.labelStyle.normal.textColor = Color.white;
              textAreaStyle.normal.textColor = Color.white;
            }

            GUIHelper.DrawRectStroke(boxRect, color);
            EditorGUI.DrawRect(boxRect, color);

            string text = string.Empty;
            float height = 0;
            proColor = new Color(0.2196f, 0.2196f, 0.2196f);
            GUIHelper.labelStyle.alignment = TextAnchor.UpperLeft;

            if (context.idFilter)
            {
              text = "<b><i>Id:</i></b>\n\n";
              text += currentMessage.GetUniqueId();
              GUIHelper.labelStyle.normal.textColor =
                ColorHelper.ColorSub(GUIHelper.labelStyle.normal.textColor, 0, 0.4f);
              GUIHelper.textContent.text = text;
              height = GUIHelper.labelStyle.CalcHeight(GUIHelper.textContent, context.columnWidth);
              EditorGUILayout.LabelField(GUIHelper.textContent, GUIHelper.labelStyle,
                GUILayout.Width(context.columnWidth),
                GUILayout.Height(height));
              GUIHelper.labelStyle.normal.textColor =
                ColorHelper.ColorAdd(GUIHelper.labelStyle.normal.textColor, 0, 0.4f);
            }

            if (context.conditionsFilter)
            {
              if (currentMessage.conditions.Length > 0)
              {
                text = "<b><i>Conditions:</i></b>\n\n";

                for (int k = 0; k < currentMessage.conditions.Length; k++)
                {
                  var condition = currentMessage.conditions[k];

                  switch (condition.type)
                  {
                    case Condition.Type.None:
                      text += condition.DisplayNone();
                      break;
                    case Condition.Type.AfterOf:
                      if (condition.afterOf.GetMessage(context) != null)
                      {
                        text += condition.DisplayAfterOf(DictionariesHelper.ContainsKey(
                          condition.afterOf.GetMessage(context).content,
                          options.currentLanguage).value);
                      }

                      break;
                    case Condition.Type.InfluenceEqualTo:
                    case Condition.Type.InfluenceGreaterThan:
                    case Condition.Type.InfluenceLessThan:
                      if (talkable.GetType() == typeof(Character))
                      {
                        text += condition.DisplayCompareInfluence();
                      }
                      else
                      {
                        text += "Invalid condition.";
                      }

                      break;
                    case Condition.Type.HasItem:
                      var itemName = "";
                      if (Item.Find(inventory.items, condition.itemId) != null)
                      {
                        itemName = DictionariesHelper.ContainsKey(Item.Find(inventory.items, condition.itemId).name,
                          options.currentLanguage).value;
                      }

                      text += condition.DisplayHasItem(itemName);
                      break;
                    case Condition.Type.DoesNotHaveTheItem:
                      var itemNameDont = "";
                      if (Item.Find(inventory.items, condition.itemId) != null)
                      {
                        itemNameDont = DictionariesHelper.ContainsKey(Item.Find(inventory.items, condition.itemId).name,
                          options.currentLanguage).value;
                      }

                      text += condition.DisplayDoesNotHaveItem(itemNameDont);
                      break;
                    case Condition.Type.ItemWasDiscarded:
                      var itemNameDiscarded = "";
                      if (Item.Find(inventory.items, condition.itemId) != null)
                      {
                        itemNameDiscarded = DictionariesHelper.ContainsKey(
                          Item.Find(inventory.items, condition.itemId).name,
                          options.currentLanguage).value;
                      }

                      text += condition.DisplayItemWasDiscarded(itemNameDiscarded);
                      break;
                    case Condition.Type.ItemIsEquipped:
                      var itemNameEquipped = "";
                      if (Item.Find(inventory.items, condition.itemId) != null)
                      {
                        itemNameEquipped = DictionariesHelper.ContainsKey(
                          Item.Find(inventory.items, condition.itemId).name,
                          options.currentLanguage).value;
                      }

                      text += condition.DisplayItemIsEquipped(itemNameEquipped);
                      break;
                    case Condition.Type.GlobalFlagEqualTo:
                      text += condition.DisplayGlobalFlagEqualTo();
                      break;
                    case Condition.Type.QuestStateIs:
                      text += condition.DisplayQuestStateIs(quests);
                      break;
                  }

                  if (k < currentMessage.conditions.Length - 1)
                  {
                    text += "\n\n";
                  }
                }

                GUIHelper.labelStyle.normal.textColor =
                  ColorHelper.ColorSub(GUIHelper.labelStyle.normal.textColor, 0, 0.4f);
                GUIHelper.textContent.text = text;
                height = GUIHelper.labelStyle.CalcHeight(GUIHelper.textContent, context.columnWidth);
                GUILayout.Label(GUIHelper.textContent, GUIHelper.labelStyle, GUILayout.Width(context.columnWidth),
                  GUILayout.Height(height));
                GUIHelper.labelStyle.normal.textColor =
                  ColorHelper.ColorAdd(GUIHelper.labelStyle.normal.textColor, 0, 0.4f);
              }
            }

            EditorGUI.DrawRect(new Rect(boxRect.xMin, boxRect.yMin, boxRect.width, 5), label.color);

            if (context.contentFilter)
            {
              GUIHelper.textContent.text = "<b><i>Content:</i></b>";
              height = GUIHelper.labelStyle.CalcHeight(GUIHelper.textContent, context.columnWidth);
              GUILayout.Label(GUIHelper.textContent, GUIHelper.labelStyle, GUILayout.Width(context.columnWidth),
                GUILayout.Height(height));

              var content = DictionariesHelper.ContainsKey(currentMessage.content, options.currentLanguage);

              if (content == null)
              {
                currentMessage.content = ArrayHelper.Add(currentMessage.content,
                  new LanguageDictionary(options.currentLanguage, "[ Message content here ]"));
                content = DictionariesHelper.ContainsKey(currentMessage.content, options.currentLanguage);
              }

              GUIHelper.textContent.text = content.value;
              height = textAreaStyle.CalcHeight(GUIHelper.textContent, context.columnWidth);

              GUI.SetNextControlName("content" + currentMessage.id);
              content.value = EditorGUILayout.TextArea(content.value, textAreaStyle,
                GUILayout.Width(context.columnWidth), GUILayout.Height(height));
              EditorGUILayout.Separator();

              #region Attached Content

              if (!currentMessage.isAChoice)
              {
                if (currentMessage.attachedContent == null)
                  currentMessage.attachedContent = new AttachedContent[0];

                foreach (var attachedContent in currentMessage.attachedContent)
                {
                  var currentAttachedContent =
                    DictionariesHelper.ContainsKey(attachedContent.content, options.currentLanguage);

                  if (currentAttachedContent == null)
                  {
                    attachedContent.content = ArrayHelper.Add(attachedContent.content,
                      new LanguageDictionary(options.currentLanguage, "[ Message content here ]"));
                    currentAttachedContent =
                      DictionariesHelper.ContainsKey(attachedContent.content, options.currentLanguage);
                  }

                  GUIHelper.textContent.text = currentAttachedContent.value;
                  height = textAreaStyle.CalcHeight(GUIHelper.textContent, context.columnWidth);

                  EditorGUILayout.BeginHorizontal(GUILayout.Height(height));
                  currentAttachedContent.value = EditorGUILayout.TextArea(currentAttachedContent.value, textAreaStyle,
                    GUILayout.Width(context.columnWidth - 30), GUILayout.Height(height));

                  if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
                  {
                    if (EditorUtility.DisplayDialog("Are you sure?",
                      "Do you really want to delete?\nThis content will be lost forever.", "Yes", "No"))
                    {
                      currentMessage.attachedContent =
                        ArrayHelper.Remove(currentMessage.attachedContent, attachedContent);
                    }
                  }

                  EditorGUILayout.EndHorizontal();

                  EditorGUILayout.Separator();
                }

                // Attach content button.
                if (GUILayout.Button("Attach content", GUILayout.Width(context.columnWidth),
                  GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
                {
                  currentMessage.attachedContent =
                    ArrayHelper.Add(currentMessage.attachedContent, new AttachedContent());
                }
              }

              #endregion
            }

            if (context.effectsFilter)
            {
              if (currentMessage.effects.Length > 0)
              {
                text = "<b><i>Effects:</i></b>\n\n";

                for (int k = 0; k < currentMessage.effects.Length; k++)
                {
                  var effect = currentMessage.effects[k];

                  switch (effect.type)
                  {
                    case Effect.Type.None:
                      text += effect.DisplayNone();
                      break;

                    case Effect.Type.EndOfContext:
                      if (effect.endOfContext.talkableName != null && effect.endOfContext.talkableName != string.Empty)
                      {
                        var tempContext =
                          effect.endOfContext.GetContext(characters, TalkableMessagesEditor.interactables);
                        if (tempContext == null) break;
                        var tempName = DictionariesHelper.ContainsKey(tempContext.name, options.currentLanguage);
                        if (tempName == null) break;
                        text += effect.DisplayEndOfContext(tempName.value);
                      }
                      else
                      {
                        var tempColumn = Column.Find(context, currentMessage.columnId);
                        if (tempColumn == null) break;
                        effect.endOfContext.talkableName = tempColumn.emitter;
                        Save();
                      }

                      break;

                    case Effect.Type.GoTo:
                      if (effect.goTo.GetMessage(context) != null)
                      {
                        text += effect.DisplayGoTo(DictionariesHelper
                          .ContainsKey(effect.goTo.GetMessage(context).content, options.currentLanguage).value);
                      }

                      break;
                    case Effect.Type.SetAnimatorAttribute:
                      text += effect.DisplaySetAnimatorAttribute();
                      break;
                    case Effect.Type.GetItem:
                      var itemName = "";
                      if (Item.Find(inventory.items, effect.itemId) != null)
                      {
                        itemName = DictionariesHelper.ContainsKey(Item.Find(inventory.items, effect.itemId).name,
                          options.currentLanguage).value;
                      }

                      text += effect.DisplayGetItem(itemName);
                      break;
                    case Effect.Type.DiscardItem:
                      var discardItemName = "";
                      if (Item.Find(inventory.items, effect.itemId) != null)
                      {
                        discardItemName = DictionariesHelper.ContainsKey(Item.Find(inventory.items, effect.itemId).name,
                          options.currentLanguage).value;
                      }

                      text += effect.DisplayDiscardItem(discardItemName);
                      break;
                    case Effect.Type.EquipItem:
                      var equipItemName = "";
                      if (Item.Find(inventory.items, effect.itemId) != null)
                      {
                        equipItemName = DictionariesHelper.ContainsKey(Item.Find(inventory.items, effect.itemId).name,
                          options.currentLanguage).value;
                      }

                      text += effect.DisplayEquipItem(equipItemName);
                      break;
                    case Effect.Type.SetGlobalFlag:
                      text += effect.DisplayGlobalFlagEqualTo();
                      break;
                    case Effect.Type.EndOfDialogue:
                      text += effect.DisplayEndOfDialogue();
                      break;
                    case Effect.Type.SetQuestState:
                      text += effect.DisplaySetQuestState(quests);
                      break;
                    case Effect.Type.FinishQuest:
                      text += effect.DisplayFinishQuest(quests);
                      break;
                    case Effect.Type.StartQuest:
                      text += effect.DiplayStartQuest(quests);
                      break;
                  }

                  if (k < currentMessage.effects.Length - 1)
                  {
                    text += "\n\n";
                  }
                }

                GUIHelper.labelStyle.normal.textColor =
                  ColorHelper.ColorSub(GUIHelper.labelStyle.normal.textColor, 0, 0.4f);
                GUIHelper.textContent.text = text;
                height = GUIHelper.labelStyle.CalcHeight(GUIHelper.textContent, context.columnWidth);
                GUILayout.Label(GUIHelper.textContent, GUIHelper.labelStyle, GUILayout.Width(context.columnWidth),
                  GUILayout.Height(height));
                GUIHelper.labelStyle.normal.textColor =
                  ColorHelper.ColorAdd(GUIHelper.labelStyle.normal.textColor, 0, 0.4f);
              }
            }

            if (currentMessage.disposable || currentMessage.isAChoice)
            {
              GUIHelper.labelStyle.fontSize = context.fontSize;
              GUIHelper.labelStyle.alignment = TextAnchor.UpperRight;
              GUIHelper.labelStyle.normal.textColor =
                ColorHelper.ColorSub(GUIHelper.labelStyle.normal.textColor, 0, 0.5f);

              text = string.Empty;

              if (currentMessage.disposable)
              {
                text += "[ disposable ] ";
              }

              if (currentMessage.isAChoice)
              {
                text += "[ is a choice ]";

                if (GUILayout.Button("Add answer?", GUILayout.Width(context.columnWidth),
                  GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
                {
                  // Get next column.
                  var nextColumn = Column.Find(context, currentMessage.columnId + 1);

                  // If don't have a next column create one.
                  if (nextColumn == null)
                  {
                    context.columns = ArrayHelper.Add(context.columns, new Column(context.columns.Length));
                    nextColumn = context.columns[context.columns.Length];
                  }

                  // Create a new message.
                  var newMessage = new Message(nextColumn.messages.Length, options.playerCharacterName, nextColumn.id,
                    currentMessage.labelId);

                  // Create a new condition with a after of.
                  var newCondition = new Condition();
                  newCondition.type = Condition.Type.AfterOf;
                  newCondition.afterOf.uniqueId = currentMessage.GetUniqueId();

                  // Add condition to the message.
                  newMessage.conditions = ArrayHelper.Add(newMessage.conditions, newCondition);

                  // Add message to the column.
                  nextColumn.messages = ArrayHelper.Add(nextColumn.messages, newMessage);

                  Save();
                }
              }

              GUIHelper.textContent.text = text;
              height = GUIHelper.labelStyle.CalcHeight(GUIHelper.textContent, context.columnWidth);
              GUILayout.Label(GUIHelper.textContent, GUIHelper.labelStyle, GUILayout.Width(context.columnWidth),
                GUILayout.Height(height));

              GUIHelper.labelStyle.fontSize = context.fontSize;
              GUIHelper.labelStyle.alignment = TextAnchor.UpperLeft;
            }

            if (GUI.Button(boxRect, "", buttonStyle))
            {
              SetMessage(currentMessage);
              EditorGUI.FocusTextInControl("");
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
          }
        }

        if (GUILayout.Button("Add Message", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          column.messages = ArrayHelper.Add(column.messages,
            new Message(column.messages.Length, column.emitter, column.id, context.labels[0].uniqueId));
          SetMessage(null);
          Save();
        }

        EditorGUILayout.Separator();
        GUILayout.EndVertical();

        GUILayout.Space(GUIHelper.MARGIN);
      }

      if (GUILayout.Button("Add Column", GUILayout.Height(GUIHelper.BUTTON_HEIGHT),
        GUILayout.Width(context.columnWidth)))
      {
        context.columns = ArrayHelper.Add(context.columns, new Column(context.columns.Length));
        Save();
      }

      GUILayout.EndHorizontal();
      EditorGUILayout.EndScrollView();

      GUIHelper.labelStyle.padding = GUIHelper.zeroPadding;
    }

    public void LabelManager()
    {
      var height = ((180 + (context.labels.Length * 240)) >= Screen.width) ? LABEL_HEIGHT : HEADER_HEIGHT;

      scrollPosLabelManager = EditorGUILayout.BeginScrollView(scrollPosLabelManager,
        messagesWindowHeaderStyle, GUILayout.Width(Screen.width), GUILayout.Height(height));
      GUILayout.BeginHorizontal();

      GUILayout.Label("Labels: ", GUILayout.Width(60));

      for (int i = 0; i < context.labels.Length; i++)
      {
        var label = context.labels[i];
        label.name = EditorGUILayout.TextField(label.name, GUILayout.Width(100));
        label.color = EditorGUILayout.ColorField(label.color, GUILayout.Width(60));
        string show = (label.show) ? "hide" : "show";
        if (GUILayout.Button(show, GUILayout.Width(40), GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          label.show = (label.show) ? false : true;
        }

        if (i > 0)
        {
          if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
          {
            context.labels = ArrayHelper.Remove(context.labels, label);
            foreach (Column col in context.columns)
            {
              foreach (Message msg in col.messages)
              {
                if (msg.labelId == label.uniqueId)
                {
                  msg.labelId = context.labels[0].uniqueId;
                }
              }
            }

            Save();
          }

          GUILayout.Space(20);
        }
        else
        {
          GUILayout.Space(40);
        }
      }

      if (GUILayout.Button("Add label", GUILayout.Width(100), GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
      {
        context.labels = ArrayHelper.Add(context.labels, new Label());
        context.labels[context.labels.Length - 1].name += " (" + (context.labels.Length - 1) + ")";
        Save();
      }

      GUILayout.EndHorizontal();
      EditorGUILayout.EndScrollView();
    }

    private void SetMessage(Message msg)
    {
      if (msg == null)
      {
        TalkableMessagesEditor.context.messageEditorState = MessageEditorState.None;
        TalkableMessagesEditor.context.currentMessage.Set(-1, -1);
      }

      else
      {
        TalkableMessagesEditor.context.messageEditorState = MessageEditorState.Normal;
        TalkableMessagesEditor.context.currentMessage.Set(msg.columnId, msg.id);
        message = msg;
      }
    }

    private void ResetStyle()
    {
      textAreaStyle.normal.background = TalkableMessagesEditor.textAreaBGTextureNormal;
      textAreaStyle.focused.background = TalkableMessagesEditor.textAreaBGTextureFocused;
      buttonStyle.normal.background = GUIHelper.transparentTexture;
      buttonStyle.active.background = GUIHelper.transparentTexture;
      GUIHelper.labelStyle.padding = GUIHelper.padding;
    }

    private void Sidebar()
    {
      scrollPosSidebar = EditorGUILayout.BeginScrollView(scrollPosSidebar, messagesWindowSidebarStyle,
        GUILayout.Width(SIDEBAR_WIDTH), GUILayout.ExpandHeight(true));

      if (EditorGUIUtility.isProSkin)
      {
        GUIHelper.labelStyle.normal.textColor = GUIHelper.proTextColor;
      }
      else
      {
        GUIHelper.labelStyle.normal.textColor = GUIHelper.freeTextColor;
      }

      GUIHelper.labelStyle.fontSize = 12;
      GUIHelper.labelStyle.alignment = TextAnchor.UpperCenter;

      if (message != null)
      {
        switch (context.messageEditorState)
        {
          case MessageEditorState.Normal:

            GUILayout.Label("<b>Properties</b>", GUIHelper.labelStyle);
            EditorGUILayout.Separator();

            var column = Column.Find(context, message.columnId);

            if (column == null)
            {
              message = null;
              return;
            }

            var disposable = message.disposable;
            var isAChoice = message.isAChoice;

            GUILayout.BeginHorizontal();
            message.disposable = GUILayout.Toggle(message.disposable, "Disposable");
            message.isAChoice = GUILayout.Toggle(message.isAChoice, "Is a choice");
            GUILayout.EndHorizontal();

            if (message.disposable != disposable || message.isAChoice != isAChoice)
            {
              EditorGUI.FocusTextInControl("");
            }

            if (message.isAChoice)
            {
              EditorGUILayout.Separator();
              GUILayout.Label("Message attributes (most influence in): ");

              foreach (string attrName in options.attributes)
              {
                for (int i = 0; i < message.attributes.Length; i++)
                {
                  if (message.attributes[i].key == attrName)
                  {
                    break;
                  }
                  else if (i == message.attributes.Length - 1)
                  {
                    message.attributes = ArrayHelper.Add(message.attributes, new AttributeDictionary(attrName));
                  }
                }
              }

              for (int i = 0; i < message.attributes.Length; i++)
              {
                message.attributes[i].value =
                  (byte) EditorGUILayout.Slider(message.attributes[i].key, message.attributes[i].value, 0, 100);
              }
            }

            GUIHelper.Separator();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Label: ");

            var label = Label.Find(context.labels, message.labelId);
            var labelIndex = 0;

            UpdateLabelsList(context);

            if (label != null)
            {
              for (int labelI = 0; labelI < labelsList.Length; labelI++)
              {
                if (label.name == labelsList[labelI])
                {
                  labelIndex = labelI;
                  break;
                }
              }
            }

            EditorGUI.BeginChangeCheck();

            labelIndex = EditorGUILayout.Popup(labelIndex, labelsList);

            if (EditorGUI.EndChangeCheck())
            {
              message.labelId = context.labels[labelIndex].uniqueId;
            }

            GUILayout.EndHorizontal();
            GUIHelper.Separator();

            var screenplayNotes = DictionariesHelper.ContainsKey(message.screenplayNotes, options.currentLanguage);

            if (screenplayNotes == null)
            {
              message.screenplayNotes = ArrayHelper.Add(message.screenplayNotes,
                new LanguageDictionary(options.currentLanguage, ""));
              screenplayNotes = DictionariesHelper.ContainsKey(message.screenplayNotes, options.currentLanguage);
            }

            GUIHelper.labelStyle.alignment = TextAnchor.UpperLeft;
            GUILayout.Label("Screenplay notes:\n<size=10>(Example: <i>whispering and gasping</i>)</size>",
              GUIHelper.labelStyle);
            screenplayNotes.value = EditorGUILayout.TextField(screenplayNotes.value);

            GUIHelper.Separator();

            EditorGUILayout.Separator();

            var audioClipPath = DictionariesHelper.ContainsKey(message.audioClipPath, options.currentLanguage);

            if (audioClipPath == null)
            {
              message.audioClipPath = ArrayHelper.Add(message.audioClipPath,
                new LanguageDictionary(options.currentLanguage, string.Empty));
              audioClipPath = DictionariesHelper.ContainsKey(message.audioClipPath, options.currentLanguage);
            }

            message.audioClip = (AudioClip) Resources.Load(audioClipPath.value);

            if (message.audioClip == null && audioClipPath.value != string.Empty)
            {
              Debug.LogWarning("Cannot find the file \"" + audioClipPath.value + "\" in Resources folder.");
            }

            GUILayout.Label("Audio to play: ");
            EditorGUI.BeginChangeCheck();

            message.audioClip = (AudioClip) EditorGUILayout.ObjectField(message.audioClip, typeof(AudioClip), false);

            if (EditorGUI.EndChangeCheck())
            {
              if (message.audioClip != null)
              {
                var str = AssetDatabase.GetAssetPath(message.audioClip).Replace("Resources/", "¬");
                var strings = str.Split('¬');
                str = strings[1].Replace(".mp3", "");
                str = str.Replace(".aif", "");
                str = str.Replace(".aiff", "");
                str = str.Replace(".ogg", "");
                str = str.Replace(".wav", "");
                audioClipPath.value = str;
              }

              else
              {
                audioClipPath.value = string.Empty;
              }
            }

            EditorGUILayout.Separator();

            message.image = (Texture2D) Resources.Load(message.imagePath);

            if (message.image == null && message.imagePath != string.Empty)
            {
              Debug.LogWarning("Cannot find the file \"" + message.imagePath + "\" in Resources folder.");
            }

            GUILayout.Label("Static image: ");
            EditorGUI.BeginChangeCheck();

            message.image = (Texture2D) EditorGUILayout.ObjectField(message.image, typeof(Texture2D), false);

            if (EditorGUI.EndChangeCheck())
            {
              if (message.image != null)
              {
                var str = AssetDatabase.GetAssetPath(message.image).Replace("Resources/", "¬");
                var strings = str.Split('¬');
                str = strings[1].Replace(".png", "");
                str = str.Replace(".jpg", "");
                str = str.Replace(".jpeg", "");
                str = str.Replace(".psd", "");
                str = str.Replace(".tga", "");
                str = str.Replace(".tiff", "");
                str = str.Replace(".gif", "");
                str = str.Replace(".bmp", "");
                message.imagePath = str;
              }

              else
              {
                message.imagePath = string.Empty;
              }
            }

            EditorGUILayout.Separator();

            EditorGUILayout.HelpBox(
              "\nMake sure to store this audio clip, texture and animator controllers in Resources folder.\n\n" +
              "Use PlayMessageAudioContent() to play audio clip and StopMessageAudioContent() to stop.\n\n" +
              "Use SwapStaticSprite(Vector pivot) to show static image.\n", MessageType.Info);

            GUIHelper.Separator();

            GUILayout.Label("Animator Attributes Setters");

            foreach (AnimatorAttributeSetter animatorAttribute in message.animatorAttributesSetters)
            {
              EditorGUILayout.Separator();

              animatorAttribute.animator = (RuntimeAnimatorController) Resources.Load(animatorAttribute.animatorPath);

              if (animatorAttribute.animator == null && animatorAttribute.animatorPath != string.Empty)
              {
                Debug.LogWarning("Cannot find the file \"" + animatorAttribute.animatorPath +
                                 "\" in Resources folder.");
              }

              GUILayout.Label("Animator Controller: ");
              EditorGUI.BeginChangeCheck();

              animatorAttribute.animator =
                (RuntimeAnimatorController) EditorGUILayout.ObjectField(animatorAttribute.animator,
                  typeof(RuntimeAnimatorController), false);

              if (EditorGUI.EndChangeCheck())
              {
                if (animatorAttribute.animator != null)
                {
                  var str = AssetDatabase.GetAssetPath(animatorAttribute.animator).Replace("Resources/", "¬");
                  var strings = str.Split('¬');
                  str = strings[1].Replace(".controller", "");
                  animatorAttribute.animatorPath = str;
                }

                else
                {
                  animatorAttribute.animatorPath = string.Empty;
                }
              }

              GUILayout.BeginHorizontal();

              GUILayout.Label("Type: ");
              animatorAttribute.type =
                (AnimatorControllerParameterType) EditorGUILayout.EnumPopup(animatorAttribute.type);

              EditorGUI.BeginChangeCheck();

              GUILayout.Label("Name: ");
              animatorAttribute.name = EditorGUILayout.TextField(animatorAttribute.name);

              if (EditorGUI.EndChangeCheck())
              {
                animatorAttribute.setTrigger = Animator.StringToHash(animatorAttribute.name);
              }

              GUILayout.EndHorizontal();

              GUILayout.BeginHorizontal();

              switch (animatorAttribute.type)
              {
                case AnimatorControllerParameterType.Bool:
                  string selected = animatorAttribute.setBool.ToString();

                  EditorGUI.BeginChangeCheck();

                  selected = GUIHelper.Popup("Set boolean to ", selected, booleanArray);

                  if (EditorGUI.EndChangeCheck())
                  {
                    if (selected == "True")
                    {
                      animatorAttribute.setBool = true;
                    }

                    else
                    {
                      animatorAttribute.setBool = false;
                    }
                  }

                  break;

                case AnimatorControllerParameterType.Float:
                  GUILayout.Label("Set float to ");
                  animatorAttribute.setFloat = EditorGUILayout.FloatField(animatorAttribute.setFloat);
                  break;

                case AnimatorControllerParameterType.Int:
                  GUILayout.Label("Set integer to ");
                  animatorAttribute.setInt = EditorGUILayout.IntField(animatorAttribute.setInt);
                  break;

                case AnimatorControllerParameterType.Trigger:
                  GUILayout.Label("Pull the trigger of [ " + animatorAttribute.name + " ]");
                  break;
              }

              GUILayout.EndHorizontal();
              EditorGUILayout.Separator();

              if (GUILayout.Button("Delete Animator Attribute Setter", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
              {
                message.animatorAttributesSetters =
                  ArrayHelper.Remove(message.animatorAttributesSetters, animatorAttribute);
                Save();
              }

              GUIHelper.Separator();
            }

            if (GUILayout.Button("Add Animator Attribute Setter", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              message.animatorAttributesSetters =
                ArrayHelper.Add(message.animatorAttributesSetters, new AnimatorAttributeSetter());
              Save();
            }

            GUIHelper.Separator();

            GUILayout.Label("Edit: ");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Conditions", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              context.messageEditorState = MessageEditorState.Conditions;
            }

            if (GUILayout.Button("Effects", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              context.messageEditorState = MessageEditorState.Effects;
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            if (column.messages.Length > 1 || context.columns.Length > 1)
            {
              GUIHelper.Separator();
              GUILayout.Label("Move: ");

              fakeButtonStyle.richText = true;
              string color = "#989898";
              GUILayout.BeginHorizontal();

              if (column.id > 0)
              {
                if (GUILayout.Button("Left", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
                {
                  message.columnId -= 1;

                  var leftCol = Column.Find(context, message.columnId);

                  message.id = leftCol.messages.Length;

                  leftCol.messages = ArrayHelper.Add(leftCol.messages, message);
                  column.messages = ArrayHelper.Remove(column.messages, message);

                  column.messages = Message.ResetIDs(column.messages);
                  leftCol.messages = Message.ResetIDs(leftCol.messages);

                  SetMessage(message);
                  Save();
                }
              }

              else
              {
                GUILayout.Box("<color=" + color + ">Left</color>", fakeButtonStyle,
                  GUILayout.Height(GUIHelper.BUTTON_HEIGHT));
              }

              if (message.id > 0)
              {
                if (GUILayout.Button("Up", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
                {
                  Message.Find(column.messages, message.id - 1).id += 1;

                  message.id -= 1;

                  SetMessage(message);
                  Save();
                }
              }

              else
              {
                GUILayout.Box("<color=" + color + ">Up</color>", fakeButtonStyle,
                  GUILayout.Height(GUIHelper.BUTTON_HEIGHT));
              }

              if (message.id < column.messages.Length - 1)
              {
                if (GUILayout.Button("Down", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
                {
                  Message.Find(column.messages, message.id + 1).id -= 1;

                  message.id += 1;

                  SetMessage(message);
                  Save();
                }
              }

              else
              {
                GUILayout.Box("<color=" + color + ">Down</color>", fakeButtonStyle,
                  GUILayout.Height(GUIHelper.BUTTON_HEIGHT));
              }

              if (column.id < context.columns.Length - 1)
              {
                if (GUILayout.Button("Right", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
                {
                  message.columnId += 1;

                  var rightCol = Column.Find(context, message.columnId);

                  message.id = rightCol.messages.Length;

                  rightCol.messages = ArrayHelper.Add(rightCol.messages, message);
                  column.messages = ArrayHelper.Remove(column.messages, message);

                  column.messages = Message.ResetIDs(column.messages);
                  rightCol.messages = Message.ResetIDs(rightCol.messages);

                  SetMessage(message);
                  Save();
                }
              }

              else
              {
                GUILayout.Box("<color=" + color + ">Right</color>", fakeButtonStyle,
                  GUILayout.Height(GUIHelper.BUTTON_HEIGHT));
              }

              GUILayout.EndHorizontal();
            }

            GUIHelper.Separator();
            GUILayout.Label("Other options: ");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Duplicate", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              column.messages = ArrayHelper.Add(column.messages, new Message(message, column.messages.Length));

              SetMessage(null);

              Save();
            }

            if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              if (EditorUtility.DisplayDialog("Are you sure?",
                "If you agree all this message data will be lost forever.", "Yes", "No"))
              {
                column.messages = ArrayHelper.Remove(column.messages, message);

                SetMessage(null);

                column.messages = Message.ResetIDs(column.messages);

                Save();
              }
            }

            GUILayout.EndHorizontal();

            GUIHelper.Separator();

            GUILayout.Label("Columns: ");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("New column at left", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              context.columns = ArrayHelper.Add(context.columns, new Column(context.columns.Length));

              MoveColumnsToRight(context, column.id);

              SetMessage(null);
              Save();
            }

            if (GUILayout.Button("New column at right", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              context.columns = ArrayHelper.Add(context.columns, new Column(context.columns.Length));

              MoveColumnsToRight(context, column.id + 1);

              SetMessage(null);
              Save();
            }

            GUILayout.EndHorizontal();

            break;

          case MessageEditorState.Conditions:

            GUILayout.Label("<b>Conditions</b>", GUIHelper.labelStyle);
            EditorGUILayout.Separator();

            if (GUILayout.Button("< Back", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              context.messageEditorState = MessageEditorState.Normal;
            }

            GUIHelper.Separator();

            var j = 0;

            foreach (Condition condition in message.conditions)
            {
              GUIHelper.labelStyle.fontSize = 11;
              GUIHelper.labelStyle.alignment = TextAnchor.UpperLeft;
              GUILayout.Label("<i>Condition " + j + "</i>\n", GUIHelper.labelStyle);
              j++;

              GUILayout.BeginHorizontal();
              GUILayout.Label("Type: ");
              condition.type = (Condition.Type) EditorGUILayout.EnumPopup(condition.type);
              GUILayout.EndHorizontal();

              switch (condition.type)
              {
                case Condition.Type.AfterOf:
                  UpdateMessagesList(context);

                  var messageName = "";

                  if (condition.afterOf.GetMessage(context) != null)
                  {
                    messageName = DictionariesHelper
                      .ContainsKey(condition.afterOf.GetMessage(context).content, options.currentLanguage).value;
                  }

                  EditorGUI.BeginChangeCheck();

                  messageName = GUIHelper.Popup("Message: ", messageName, messageList);

                  if (EditorGUI.EndChangeCheck())
                  {
                    foreach (Column col in context.columns)
                    {
                      foreach (Message msg in col.messages)
                      {
                        if (DictionariesHelper.ContainsKey(msg.content, options.currentLanguage).value == messageName)
                        {
                          condition.afterOf.uniqueId = msg.GetUniqueId();
                          break;
                        }
                      }
                    }
                  }

                  break;

                case Condition.Type.InfluenceEqualTo:
                case Condition.Type.InfluenceGreaterThan:
                case Condition.Type.InfluenceLessThan:
                  if (talkable.GetType() == typeof(Character))
                  {
                    UpdateCharacterList();
                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();

                    if (condition.characterInfluencedName == string.Empty && characterList.Length > 0)
                    {
                      condition.characterInfluencedName = characterList[0];
                    }

                    condition.comparedInfluence = EditorGUILayout.IntField(condition.comparedInfluence);
                    condition.characterInfluencedName =
                      GUIHelper.Popup(" in ", condition.characterInfluencedName, characterList);

                    if (EditorGUI.EndChangeCheck())
                    {
                      condition.DisplayCompareInfluence();
                    }

                    GUILayout.EndHorizontal();
                  }
                  else
                  {
                    EditorGUILayout.HelpBox("This condition is only valid to a character.", MessageType.Warning);
                  }

                  break;

                case Condition.Type.HasItem:
                  GUILayout.BeginHorizontal();
                  UpdateItemList();

                  var itemName = "";

                  if (itemList.Length > 0)
                  {
                    itemName = DictionariesHelper.ContainsKey(Item.Find(inventory.items, condition.itemId).name,
                      options.currentLanguage).value;
                  }

                  EditorGUI.BeginChangeCheck();

                  itemName = GUIHelper.Popup("Has item ", itemName, itemList);

                  if (EditorGUI.EndChangeCheck())
                  {
                    foreach (Item item in inventory.items)
                    {
                      if (DictionariesHelper.ContainsKey(item.name, options.currentLanguage).value == itemName)
                      {
                        condition.itemId = item.id;
                        break;
                      }
                    }
                  }

                  GUILayout.EndHorizontal();
                  break;

                case Condition.Type.DoesNotHaveTheItem:
                  GUILayout.BeginHorizontal();
                  UpdateItemList();

                  var itemNameDont = "";

                  if (itemList.Length > 0)
                  {
                    itemNameDont = DictionariesHelper.ContainsKey(Item.Find(inventory.items, condition.itemId).name,
                      options.currentLanguage).value;
                  }

                  EditorGUI.BeginChangeCheck();

                  itemNameDont = GUIHelper.Popup("Does not have the item ", itemNameDont, itemList);

                  if (EditorGUI.EndChangeCheck())
                  {
                    foreach (Item item in inventory.items)
                    {
                      if (DictionariesHelper.ContainsKey(item.name, options.currentLanguage).value == itemNameDont)
                      {
                        condition.itemId = item.id;
                        break;
                      }
                    }
                  }

                  GUILayout.EndHorizontal();
                  break;

                case Condition.Type.ItemIsEquipped:
                  GUILayout.BeginHorizontal();
                  UpdateItemList();

                  var equippedItemName = "";

                  if (itemList.Length > 0)
                  {
                    equippedItemName = DictionariesHelper.ContainsKey(Item.Find(inventory.items, condition.itemId).name,
                      options.currentLanguage).value;
                  }

                  EditorGUI.BeginChangeCheck();

                  equippedItemName = GUIHelper.Popup("Item is equipped ", equippedItemName, itemList);

                  if (EditorGUI.EndChangeCheck())
                  {
                    foreach (Item item in inventory.items)
                    {
                      if (DictionariesHelper.ContainsKey(item.name, options.currentLanguage).value == equippedItemName)
                      {
                        condition.itemId = item.id;
                        break;
                      }
                    }
                  }

                  GUILayout.EndHorizontal();
                  break;

                case Condition.Type.ItemWasDiscarded:
                  GUILayout.BeginHorizontal();
                  UpdateItemList();

                  var discardedItemName = "";

                  if (itemList.Length > 0)
                  {
                    discardedItemName = DictionariesHelper
                      .ContainsKey(Item.Find(inventory.items, condition.itemId).name, options.currentLanguage).value;
                  }

                  EditorGUI.BeginChangeCheck();

                  discardedItemName = GUIHelper.Popup("Item was discarded ", discardedItemName, itemList);

                  if (EditorGUI.EndChangeCheck())
                  {
                    foreach (Item item in inventory.items)
                    {
                      if (DictionariesHelper.ContainsKey(item.name, options.currentLanguage).value == discardedItemName)
                      {
                        condition.itemId = item.id;
                        break;
                      }
                    }
                  }

                  GUILayout.EndHorizontal();
                  break;

                case Condition.Type.GlobalFlagEqualTo:
                  UpdateGlobalFlagsList();

                  condition.globalFlag.name = GUIHelper.Popup("Flag: ", condition.globalFlag.name, globalFlagsList);

                  string selected = condition.globalFlag.value.ToString();

                  EditorGUI.BeginChangeCheck();

                  selected = GUIHelper.Popup("is ", selected, booleanArray);

                  if (EditorGUI.EndChangeCheck())
                  {
                    if (selected == "True")
                    {
                      condition.globalFlag.value = true;
                    }

                    else
                    {
                      condition.globalFlag.value = false;
                    }
                  }

                  break;
                case Condition.Type.QuestStateIs:
                  // Get the quest name.
                  var quest = Quest.Find(quests, condition.questAndState.questId);
                  var questName = quest != null ? quest.Name : string.Empty;

                  // Quest name Popup with a change checker.
                  EditorGUI.BeginChangeCheck();
                  var questNames = Quest.GetNames(quests);
                  questNames = ArrayHelper.Add(questNames, string.Empty);
                  questName = GUIHelper.Popup("Quest: ", questName, questNames);
                  if (EditorGUI.EndChangeCheck())
                  {
                    var questIndex = ArrayHelper.GetIndex(Quest.GetNames(quests), questName);
                    if (questIndex > -1) condition.questAndState.questId = Quest.GetIDs(quests)[questIndex];
                  }

                  // Get the quest state name.
                  quest = Quest.Find(quests, condition.questAndState.questId);
                  var questState = quest != null ? quest.GetState(condition.questAndState.questStateId) : null;
                  var questStateName = questState != null ? questState.ShortDescription : string.Empty;

                  // Quest state Popup with a change checker.
                  if (quest != null)
                  {
                    EditorGUI.BeginChangeCheck();
                    var questStateNames = QuestState.GetShortDescriptions(quest.questStates);
                    questStateNames = ArrayHelper.Add(questStateNames, string.Empty);
                    questStateName = GUIHelper.Popup("Quest state: ", questStateName, questStateNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                      var questStateIndex = ArrayHelper.GetIndex(QuestState.GetShortDescriptions(quest.questStates),
                        questStateName);
                      if (questStateIndex > -1)
                        condition.questAndState.questStateId = QuestState.GetIDs(quest.questStates)[questStateIndex];
                    }
                  }

                  break;
              }

              if (GUILayout.Button("Delete Condition", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
              {
                message.conditions = ArrayHelper.Remove(message.conditions, condition);
                Save();
              }

              GUIHelper.Separator();
            }

            if (GUILayout.Button("Add Condition", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              message.conditions = ArrayHelper.Add(message.conditions, new Condition());
              Save();
            }

            break;

          case MessageEditorState.Effects:

            GUILayout.Label("<b>Effects</b>", GUIHelper.labelStyle);
            EditorGUILayout.Separator();

            if (GUILayout.Button("< Back", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              context.messageEditorState = MessageEditorState.Normal;
            }

            GUIHelper.Separator();

            var k = 0;

            foreach (Effect effect in message.effects)
            {
              GUIHelper.labelStyle.fontSize = 11;
              GUIHelper.labelStyle.alignment = TextAnchor.UpperLeft;
              GUILayout.Label("<i>Effect " + k + "</i>\n", GUIHelper.labelStyle);
              k++;

              GUILayout.BeginHorizontal();
              GUILayout.Label("Type: ");
              effect.type = (Effect.Type) EditorGUILayout.EnumPopup(effect.type);
              GUILayout.EndHorizontal();

              switch (effect.type)
              {
                case Effect.Type.EndOfContext:

                  UpdateContextList();
                  var contextName = string.Empty;

                  if (effect.endOfContext.talkableName != null)
                  {
                    Context contextToEnd = effect.endOfContext.GetContext(characters, interactables);
                    if (contextToEnd != null)
                    {
                      contextName = DictionariesHelper.ContainsKey(contextToEnd.name, options.currentLanguage).value;
                    }
                  }

                  EditorGUI.BeginChangeCheck();

                  contextName = GUIHelper.Popup("Context: ", contextName, contextList);

                  if (EditorGUI.EndChangeCheck())
                  {
                    foreach (Character tempCharacter in characters)
                    {
                      foreach (Context ctx in tempCharacter.contexts)
                      {
                        if (DictionariesHelper.ContainsKey(ctx.name, options.currentLanguage).value == contextName)
                        {
                          effect.endOfContext.Set(tempCharacter.name, ctx.id);
                          break;
                        }
                      }
                    }

                    foreach (Interactable tempInteractable in interactables)
                    {
                      foreach (Context ctx in tempInteractable.contexts)
                      {
                        if (DictionariesHelper.ContainsKey(ctx.name, options.currentLanguage).value == contextName)
                        {
                          effect.endOfContext.Set(tempInteractable.name, ctx.id);
                          break;
                        }
                      }
                    }
                  }

                  break;

                case Effect.Type.GoTo:
                  UpdateMessagesList(context);
                  var messageContent = string.Empty;

                  if (effect.goTo.GetMessage(context) != null)
                  {
                    messageContent = DictionariesHelper
                      .ContainsKey(effect.goTo.GetMessage(context).content, options.currentLanguage).value;
                  }

                  EditorGUI.BeginChangeCheck();

                  messageContent = GUIHelper.Popup("Message: ", messageContent, messageList);

                  if (EditorGUI.EndChangeCheck())
                  {
                    foreach (Column col in context.columns)
                    {
                      foreach (Message msg in col.messages)
                      {
                        if (DictionariesHelper.ContainsKey(msg.content, options.currentLanguage).value ==
                            messageContent)
                        {
                          effect.goTo.uniqueId = msg.GetUniqueId();
                          break;
                        }
                      }
                    }
                  }

                  break;

                case Effect.Type.SetAnimatorAttribute:

                  effect.animatorAttributeSetter.animator =
                    (RuntimeAnimatorController) Resources.Load(effect.animatorAttributeSetter.animatorPath);

                  if (effect.animatorAttributeSetter.animator == null &&
                      effect.animatorAttributeSetter.animatorPath != string.Empty)
                  {
                    Debug.LogWarning("Cannot find the file \"" + effect.animatorAttributeSetter.animatorPath +
                                     "\" in Resources folder.");
                  }

                  GUILayout.Label("Animator Controller: ");
                  EditorGUI.BeginChangeCheck();

                  effect.animatorAttributeSetter.animator = (RuntimeAnimatorController) EditorGUILayout.ObjectField(
                    effect.animatorAttributeSetter.animator,
                    typeof(RuntimeAnimatorController), false);

                  if (EditorGUI.EndChangeCheck())
                  {
                    if (effect.animatorAttributeSetter.animator != null)
                    {
                      var str = AssetDatabase.GetAssetPath(effect.animatorAttributeSetter.animator)
                        .Replace("Resources/", "¬");
                      var strings = str.Split('¬');
                      str = strings[1].Replace(".controller", "");
                      effect.animatorAttributeSetter.animatorPath = str;
                    }

                    else
                    {
                      effect.animatorAttributeSetter.animatorPath = string.Empty;
                    }
                  }

                  GUILayout.BeginHorizontal();

                  GUILayout.Label("Type: ");
                  effect.animatorAttributeSetter.type =
                    (AnimatorControllerParameterType) EditorGUILayout.EnumPopup(effect.animatorAttributeSetter.type);

                  EditorGUI.BeginChangeCheck();

                  GUILayout.Label("Name: ");
                  effect.animatorAttributeSetter.name = EditorGUILayout.TextField(effect.animatorAttributeSetter.name);

                  if (EditorGUI.EndChangeCheck())
                  {
                    effect.animatorAttributeSetter.setTrigger =
                      Animator.StringToHash(effect.animatorAttributeSetter.name);
                  }

                  GUILayout.EndHorizontal();

                  GUILayout.BeginHorizontal();

                  switch (effect.animatorAttributeSetter.type)
                  {
                    case AnimatorControllerParameterType.Bool:
                      string selected = effect.animatorAttributeSetter.setBool.ToString();

                      EditorGUI.BeginChangeCheck();

                      selected = GUIHelper.Popup("Set boolean to ", selected, booleanArray);

                      if (EditorGUI.EndChangeCheck())
                      {
                        if (selected == "True")
                        {
                          effect.animatorAttributeSetter.setBool = true;
                        }

                        else
                        {
                          effect.animatorAttributeSetter.setBool = false;
                        }
                      }

                      break;

                    case AnimatorControllerParameterType.Float:
                      GUILayout.Label("Set float to ");
                      effect.animatorAttributeSetter.setFloat =
                        EditorGUILayout.FloatField(effect.animatorAttributeSetter.setFloat);
                      break;

                    case AnimatorControllerParameterType.Int:
                      GUILayout.Label("Set integer to ");
                      effect.animatorAttributeSetter.setInt =
                        EditorGUILayout.IntField(effect.animatorAttributeSetter.setInt);
                      break;

                    case AnimatorControllerParameterType.Trigger:
                      GUILayout.Label("Pull the trigger of [ " + effect.animatorAttributeSetter.name + " ]");
                      break;
                  }

                  GUILayout.EndHorizontal();

                  break;

                case Effect.Type.GetItem:
                  GUILayout.BeginHorizontal();
                  UpdateItemList();

                  var itemName = "";

                  if (itemList.Length > 0)
                  {
                    itemName = DictionariesHelper.ContainsKey(Item.Find(inventory.items, effect.itemId).name,
                      options.currentLanguage).value;
                  }

                  EditorGUI.BeginChangeCheck();

                  itemName = GUIHelper.Popup("Get item ", itemName, itemList);

                  if (EditorGUI.EndChangeCheck())
                  {
                    foreach (Item item in inventory.items)
                    {
                      if (DictionariesHelper.ContainsKey(item.name, options.currentLanguage).value == itemName)
                      {
                        effect.itemId = item.id;
                        break;
                      }
                    }
                  }

                  GUILayout.EndHorizontal();
                  break;

                case Effect.Type.DiscardItem:
                  GUILayout.BeginHorizontal();
                  UpdateItemList();

                  var discardItemName = "";

                  if (itemList.Length > 0)
                  {
                    discardItemName = DictionariesHelper.ContainsKey(Item.Find(inventory.items, effect.itemId).name,
                      options.currentLanguage).value;
                  }

                  EditorGUI.BeginChangeCheck();

                  discardItemName = GUIHelper.Popup("Discard item ", discardItemName, itemList);

                  if (EditorGUI.EndChangeCheck())
                  {
                    foreach (Item item in inventory.items)
                    {
                      if (DictionariesHelper.ContainsKey(item.name, options.currentLanguage).value == discardItemName)
                      {
                        effect.itemId = item.id;
                        break;
                      }
                    }
                  }

                  GUILayout.EndHorizontal();
                  break;

                case Effect.Type.EquipItem:
                  GUILayout.BeginHorizontal();
                  UpdateItemList();

                  var equipItemName = "";

                  if (itemList.Length > 0)
                  {
                    equipItemName = DictionariesHelper.ContainsKey(Item.Find(inventory.items, effect.itemId).name,
                      options.currentLanguage).value;
                  }

                  EditorGUI.BeginChangeCheck();

                  equipItemName = GUIHelper.Popup("Discard item ", equipItemName, itemList);

                  if (EditorGUI.EndChangeCheck())
                  {
                    foreach (Item item in inventory.items)
                    {
                      if (DictionariesHelper.ContainsKey(item.name, options.currentLanguage).value == equipItemName)
                      {
                        effect.itemId = item.id;
                        break;
                      }
                    }
                  }

                  GUILayout.EndHorizontal();
                  break;

                case Effect.Type.SetGlobalFlag:
                  UpdateGlobalFlagsList();

                  effect.globalFlag.name = GUIHelper.Popup("Flag: ", effect.globalFlag.name, globalFlagsList);

                  string effectSelected = effect.globalFlag.value.ToString();

                  EditorGUI.BeginChangeCheck();

                  effectSelected = GUIHelper.Popup(" set to ", effectSelected, booleanArray);

                  if (EditorGUI.EndChangeCheck())
                  {
                    if (effectSelected == "True")
                    {
                      effect.globalFlag.value = true;
                    }

                    else
                    {
                      effect.globalFlag.value = false;
                    }
                  }

                  break;

                case Effect.Type.EndOfDialogue:
                  break;

                case Effect.Type.SetQuestState:
                  // Get the quest name.
                  var quest = Quest.Find(quests, effect.questAndState.questId);
                  var questName = quest != null ? quest.Name : string.Empty;

                  // Quest Popup with a change checker.
                  EditorGUI.BeginChangeCheck();
                  var questNames = Quest.GetNames(quests);
                  questNames = ArrayHelper.Add(questNames, string.Empty);
                  questName = GUIHelper.Popup("Quest: ", questName, questNames);
                  if (EditorGUI.EndChangeCheck())
                  {
                    var questIndex = ArrayHelper.GetIndex(Quest.GetNames(quests), questName);
                    if (questIndex > -1) effect.questAndState.questId = Quest.GetIDs(quests)[questIndex];
                  }

                  // Get the quest state name.
                  quest = Quest.Find(quests, effect.questAndState.questId);
                  var questState = quest != null ? quest.GetState(effect.questAndState.questStateId) : null;
                  var questStateName = questState != null ? questState.ShortDescription : string.Empty;

                  // Quest state Popup with a change checker.
                  if (quest != null)
                  {
                    EditorGUI.BeginChangeCheck();
                    var questStateNames = QuestState.GetShortDescriptions(quest.questStates);
                    questStateNames = ArrayHelper.Add(questStateNames, string.Empty);
                    questStateName = GUIHelper.Popup("Quest state: ", questStateName, questStateNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                      var questStateIndex = ArrayHelper.GetIndex(QuestState.GetShortDescriptions(quest.questStates),
                        questStateName);
                      if (questStateIndex > -1)
                        effect.questAndState.questStateId = QuestState.GetIDs(quest.questStates)[questStateIndex];
                    }
                  }

                  break;

                case Effect.Type.FinishQuest:
                  // Get the quest name.
                  var questToFinish = Quest.Find(quests, effect.questAndState.questId);
                  var questToFinishName = questToFinish != null ? questToFinish.Name : string.Empty;

                  // Quest Popup with a change checker.
                  EditorGUI.BeginChangeCheck();
                  var questToFinishNames = Quest.GetNames(quests);
                  questToFinishNames = ArrayHelper.Add(questToFinishNames, string.Empty);
                  questToFinishName = GUIHelper.Popup("Quest: ", questToFinishName, questToFinishNames);
                  if (EditorGUI.EndChangeCheck())
                  {
                    var questIndex = ArrayHelper.GetIndex(Quest.GetNames(quests), questToFinishName);
                    if (questIndex > -1) effect.questAndState.questId = Quest.GetIDs(quests)[questIndex];
                  }

                  break;

                case Effect.Type.StartQuest:
                  // Get the quest name.
                  var questToStart = Quest.Find(quests, effect.questAndState.questId);
                  var questToStartName = questToStart != null ? questToStart.Name : string.Empty;

                  // Quest Popup with a change checker.
                  EditorGUI.BeginChangeCheck();
                  var questToStartNames = Quest.GetNames(quests);
                  questToStartNames = ArrayHelper.Add(questToStartNames, string.Empty);
                  questToStartName = GUIHelper.Popup("Quest: ", questToStartName, questToStartNames);
                  if (EditorGUI.EndChangeCheck())
                  {
                    var questIndex = ArrayHelper.GetIndex(Quest.GetNames(quests), questToStartName);
                    if (questIndex > -1) effect.questAndState.questId = Quest.GetIDs(quests)[questIndex];
                  }

                  break;
              }

              if (GUILayout.Button("Delete Effect", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
              {
                message.effects = ArrayHelper.Remove(message.effects, effect);
                Save();
              }

              GUIHelper.Separator();
            }

            if (GUILayout.Button("Add Effect", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
            {
              message.effects = ArrayHelper.Add(message.effects, new Effect(talkable.name));
              Save();
            }

            break;
        }
      }

      #region Local Variables

      GUIHelper.Separator();

      EditorGUILayout.LabelField("Context local variables:");
      GUILayout.Space(GUIHelper.MARGIN);

      if (context.LocalVariables == null)
        context.LocalVariables = new LocalVariable[0];

      foreach (var localVariable in context.LocalVariables)
      {
        EditorGUILayout.BeginHorizontal();

        localVariable.Type = (VariableType) EditorGUILayout.EnumPopup(localVariable.Type);
        localVariable.Name =
          EditorGUILayout.TextField(localVariable.Name, GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL));

        switch (localVariable.Type)
        {
          case VariableType.String:
            DictionariesHelper.ContainsKey(localVariable.StringValue, options.currentLanguage).value =
              EditorGUILayout.TextField(
                DictionariesHelper.ContainsKey(localVariable.StringValue, options.currentLanguage).value,
                GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL));
            break;
          case VariableType.Int:
            localVariable.IntValue =
              EditorGUILayout.IntField(localVariable.IntValue, GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL));
            break;
          case VariableType.Float:
            localVariable.FloatValue = EditorGUILayout.FloatField(localVariable.FloatValue,
              GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL));
            break;
        }

        if (GUILayout.Button("X", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL),
          GUILayout.Width(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          context.LocalVariables = ArrayHelper.Remove(context.LocalVariables, localVariable);
          Save();
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(GUIHelper.MARGIN);
      }

      if (GUILayout.Button("Add local variable", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
      {
        context.AddLocalVariable(string.Format("variable{0}", context.LocalVariables.Length), VariableType.String,
          string.Empty);
        Save();
      }

      #endregion

      GUIHelper.Separator();

      if (GUILayout.Button("Remove empty columns", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
      {
        context.columns = Column.RemoveEmptyColumns(context.columns);
        context.messageEditorState = MessageEditorState.None;

        Save();
      }

      EditorGUILayout.EndScrollView();
    }

    private void MoveColumnsToRight(Context context, int toIndex)
    {
      for (int i = context.columns.Length - 1; i >= toIndex; i--)
      {
        var col = Column.Find(context, i);
        var rightCol = Column.Find(context, i + 1);

        foreach (Message msg in col.messages)
        {
          msg.columnId += 1;

          msg.id = rightCol.messages.Length;

          rightCol.messages = ArrayHelper.Add(rightCol.messages, msg);
          col.messages = ArrayHelper.Remove(col.messages, msg);

          col.messages = Message.ResetIDs(col.messages);
          rightCol.messages = Message.ResetIDs(rightCol.messages);

          rightCol.emitter = col.emitter;
        }
      }
    }

    private void UpdateCharacterList()
    {
      characterList = new string[0];

      foreach (string str in options.characterList)
      {
        if (str != options.playerCharacterName)
        {
          characterList = ArrayHelper.Add(characterList, str);
        }
      }
    }

    private void UpdateItemList()
    {
      itemList = new string[0];

      foreach (Item item in inventory.items)
      {
        itemList = ArrayHelper.Add(itemList, DictionariesHelper.ContainsKey(item.name, options.currentLanguage).value);
      }
    }

    private void UpdateMessagesList(Context context)
    {
      messageList = new string[0];

      foreach (Column col in context.columns)
      {
        foreach (Message msg in col.messages)
        {
          LanguageDictionary content = DictionariesHelper.ContainsKey(msg.content, options.currentLanguage);

          if (content == null)
          {
            msg.content = ArrayHelper.Add(msg.content, new LanguageDictionary(options.currentLanguage, ""));
            content = DictionariesHelper.ContainsKey(msg.content, options.currentLanguage);
          }

          messageList = ArrayHelper.Add(messageList, content.value);
        }
      }
    }

    private void UpdateContextList()
    {
      contextList = new string[0];

      foreach (Character character in characters)
      {
        foreach (Context context in character.contexts)
        {
          LanguageDictionary contextName = DictionariesHelper.ContainsKey(context.name, options.currentLanguage);

          if (contextName == null)
          {
            context.name = ArrayHelper.Add(context.name,
              new LanguageDictionary(options.currentLanguage, "Name [Change clicking on Edit]"));
            contextName = DictionariesHelper.ContainsKey(context.name, options.currentLanguage);
          }

          contextList = ArrayHelper.Add(contextList, contextName.value);
        }
      }

      foreach (Interactable interactable in interactables)
      {
        foreach (Context context in interactable.contexts)
        {
          LanguageDictionary contextName = DictionariesHelper.ContainsKey(context.name, options.currentLanguage);

          if (contextName == null)
          {
            context.name = ArrayHelper.Add(context.name,
              new LanguageDictionary(options.currentLanguage, "Name [Change clicking on Edit]"));
            contextName = DictionariesHelper.ContainsKey(context.name, options.currentLanguage);
          }

          contextList = ArrayHelper.Add(contextList, contextName.value);
        }
      }
    }

    private void UpdateGlobalFlagsList()
    {
      globalFlagsList = new string[0];

      foreach (Flag flag in globalFlags.flags)
      {
        globalFlagsList = ArrayHelper.Add(globalFlagsList, flag.name);
      }
    }

    private void UpdateLabelsList(Context context)
    {
      labelsList = new string[0];

      foreach (Label label in context.labels)
      {
        labelsList = ArrayHelper.Add(labelsList, label.name);
      }
    }
  }
}
