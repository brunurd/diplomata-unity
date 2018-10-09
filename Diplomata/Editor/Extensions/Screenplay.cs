using System;
using LavaLeak.Diplomata.Editor;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Editor.Windows;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using RTFExporter;
using UnityEditor;

namespace LavaLeak.Diplomata.Editor.Extensions
{
  public class Screenplay
  {
    public static void Save(Talkable talkable)
    {
      var options = OptionsController.GetOptions();
      RTFDocument doc = CreateDocument();

      doc = AddTalkable(doc, talkable, options);

      RTFParser.ToFile("Assets/" + PlayerSettings.productName + " Screenplay - " + talkable.name + " - " + options.currentLanguage + ".rtf", doc);
      AssetDatabase.Refresh();
    }

    [MenuItem("Diplomata/Export To.../.rtf (Screenplay)", false, 2)]
    public static void SaveAll()
    {
      var options = OptionsController.GetOptions();
      var characters = CharactersController.GetCharacters(options);
      RTFDocument doc = CreateDocument();

      foreach (Character character in characters)
      {
        doc = AddTalkable(doc, character, options);
      }

      RTFParser.ToFile("Assets/" + PlayerSettings.productName + " Screenplay - " + options.currentLanguage + ".rtf", doc);
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

    private static RTFDocument AddTalkable(RTFDocument document, Talkable character, Options options)
    {
      RTFTextStyle style = new RTFTextStyle(false, false, 12, "Courier", Color.black);
      RTFTextStyle styleAllcaps = new RTFTextStyle(false, false, false, false, true, false, 12, "Courier", Color.black, Underline.None);

      RTFParagraphStyle noIndent = new RTFParagraphStyle(Alignment.Left, new Indent(0, 0, 0), 0, 400);
      RTFParagraphStyle messageContentIndent = new RTFParagraphStyle(Alignment.Left, new Indent(0, 2.54f, 0), 0, 400);

      var presentation = document.AppendParagraph(noIndent);
      presentation.AppendText(character.name, styleAllcaps);

      var characterDescription = DictionariesHelper.ContainsKey(character.description, options.currentLanguage);
      var text = string.Empty;

      text = ", " + characterDescription.value;

      if (text[text.Length - 1] != '.')
      {
        text += ".";
      }

      presentation.AppendText(text, style);

      for (var i = 0; i < character.contexts.Length; i++)
      {
        var context = character.contexts[i];
        var contextPar = document.AppendParagraph(noIndent);
        var name = DictionariesHelper.ContainsKey(context.name, options.currentLanguage);

        contextPar.AppendText(string.Format("CTX {0}\n", i), styleAllcaps);
        contextPar.AppendText(name.value, style);

        foreach (var column in context.columns)
        {
          for (var j = 0; j < column.messages.Length; j++)
          {
            var messagePar = document.AppendParagraph(messageContentIndent);
            messagePar.AppendText("\t\t" + column.emitter + "\n", styleAllcaps);

            var screenplayNotes = DictionariesHelper.ContainsKey(column.messages[j].screenplayNotes, options.currentLanguage);

            if (screenplayNotes != null)
            {
              if (screenplayNotes.value != "")
              {
                messagePar.AppendText("\t(" + screenplayNotes.value + ")\n", style);
              }
            }

            var content = DictionariesHelper.ContainsKey(column.messages[j].content, options.currentLanguage);

            if (column.messages.Length > 1)
            {
              messagePar.AppendText("(" + j + "): " + content.value, style);
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
