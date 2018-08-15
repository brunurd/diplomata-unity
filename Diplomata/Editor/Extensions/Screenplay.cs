using System;
using Diplomata.Helpers;
using Diplomata.Models;
using DiplomataEditor;
using DiplomataEditor.Helpers;
using DiplomataEditor.Windows;
using RTFExporter;
using UnityEditor;

namespace DiplomataEditor
{
  public class Screenplay
  {
    private static DiplomataEditorData diplomataEditor;

    public static void Save(Talkable talkable)
    {
      RTFDocument doc = CreateDocument();
      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");

      doc = AddTalkable(doc, talkable);

      RTFParser.ToFile("Assets/" + PlayerSettings.productName + " Screenplay - " + talkable.name + " - " + diplomataEditor.options.currentLanguage + ".rtf", doc);
      AssetDatabase.Refresh();
    }

    public static void SaveAll()
    {
      RTFDocument doc = CreateDocument();
      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");

      foreach (Character character in diplomataEditor.characters)
      {
        doc = AddTalkable(doc, character);
      }

      RTFParser.ToFile("Assets/" + PlayerSettings.productName + " Screenplay - " + diplomataEditor.options.currentLanguage + ".rtf", doc);
      AssetDatabase.Refresh();
    }

    private static RTFDocument CreateDocument()
    {
      RTFDocument document = new RTFDocument(21, 29.7f, Orientation.Portrait, Units.Centimeters);
      document.margin = new Margin(2.54f, 2.54f, 2.54f, 2.54f);

      if (Environment.UserName != null)
      {
        document.author = Environment.UserName;
      }

      return document;
    }

    private static RTFDocument AddTalkable(RTFDocument document, Talkable character)
    {
      RTFTextStyle style = new RTFTextStyle(false, false, 12, "Courier", Color.black);
      RTFTextStyle styleAllcaps = new RTFTextStyle(false, false, false, false, true, false, 12, "Courier", Color.black, Underline.None);

      RTFParagraphStyle noIndent = new RTFParagraphStyle(Alignment.Left, new Indent(0, 0, 0), 0, 400);
      RTFParagraphStyle messageContentIndent = new RTFParagraphStyle(Alignment.Left, new Indent(0, 2.54f, 0), 0, 400);

      var presentation = document.AppendParagraph(noIndent);
      presentation.AppendText(character.name, styleAllcaps);

      var characterDescription = DictionariesHelper.ContainsKey(character.description, diplomataEditor.options.currentLanguage);
      var text = string.Empty;

      text = ", " + characterDescription.value;

      if (text[text.Length - 1] != '.')
      {
        text += ".";
      }

      presentation.AppendText(text, style);

      foreach (Context context in character.contexts)
      {

        var contextPar = document.AppendParagraph(noIndent);

        var name = DictionariesHelper.ContainsKey(context.name, diplomataEditor.options.currentLanguage);
        var contextDescription = DictionariesHelper.ContainsKey(context.description, diplomataEditor.options.currentLanguage);

        contextPar.AppendText(name.value + "\n", styleAllcaps);
        contextPar.AppendText(contextDescription.value, style);

        foreach (Column column in context.columns)
        {
          for (int i = 0; i < column.messages.Length; i++)
          {
            var messagePar = document.AppendParagraph(messageContentIndent);
            messagePar.AppendText("\t\t" + column.emitter + "\n", styleAllcaps);

            var screenplayNotes = DictionariesHelper.ContainsKey(column.messages[i].screenplayNotes, diplomataEditor.options.currentLanguage);

            if (screenplayNotes != null)
            {
              if (screenplayNotes.value != "")
              {
                messagePar.AppendText("\t(" + screenplayNotes.value + ")\n", style);
              }
            }

            var content = DictionariesHelper.ContainsKey(column.messages[i].content, diplomataEditor.options.currentLanguage);

            if (column.messages.Length > 1)
            {
              messagePar.AppendText("(" + i + "): " + content.value, style);
            }

            else
            {
              messagePar.AppendText(content.value, style);
            }
          }
        }

      }

      return document;
    }
  }
}
