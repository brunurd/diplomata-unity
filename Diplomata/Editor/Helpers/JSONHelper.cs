using System;
using System.IO;
using Diplomata.Editor;
using Diplomata.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Helpers
{
  public class JSONHelper
  {
    public static void Create(System.Object obj, string filename, bool prettyPrint = false, string folder = "")
    {
      try
      {
        CreateFolder(folder);

        var file = File.Create(DiplomataEditorData.resourcesFolder + folder + filename + ".json");
        file.Close();

        AssetDatabase.Refresh();

        Update(obj, filename, prettyPrint, folder);
      }

      catch (Exception e)
      {
        Debug.LogError("Cannot create " + folder + filename + ".json in " + DiplomataEditorData.resourcesFolder + ". " + e.Message);
      }
    }

    public static T Read<T>(string filename, string folder = "")
    {
      try
      {
        TextAsset json = (TextAsset) Resources.Load(folder + filename);

        if (json == null)
        {
          json = (TextAsset) Resources.Load(filename);
        }

        return JsonUtility.FromJson<T>(json.text);
      }

      catch (Exception e)
      {
        Debug.LogError("Cannot read " + filename + " in Resources folder. " + e.Message);
        return default(T);
      }
    }

    public static void Update(System.Object obj, string filename, bool prettyPrint = false, string folder = "")
    {
      try
      {
        string json = JsonUtility.ToJson(obj, prettyPrint);

        using(FileStream fs = new FileStream(DiplomataEditorData.resourcesFolder + folder + filename + ".json", FileMode.Create))
        {
          using(StreamWriter writer = new StreamWriter(fs))
          {
            writer.Write(json);
          }
        }

        AssetDatabase.Refresh();
      }

      catch (Exception e)
      {
        Debug.LogError("Cannot update " + filename + " in Resources folder. " + e.Message);
      }
    }

    public static void Delete(string filename, string folder = "")
    {
      var path = DiplomataEditorData.resourcesFolder + folder + filename;

      try
      {
        File.Delete(path + ".json");
        File.Delete(path + ".json.meta");
      }

      catch (Exception e)
      {
        Debug.LogError("Cannot delete " + path + ".json. " + e.Message);
      }

      AssetDatabase.Refresh();
    }

    public static bool Exists(string filename, string folder = "")
    {
      if (!File.Exists(DiplomataEditorData.resourcesFolder + filename + ".json") &&
        !File.Exists(DiplomataEditorData.resourcesFolder + folder + filename + ".json"))
      {
        return false;
      }

      else
      {
        return true;
      }
    }

    public static void CreateFolder(string folderName)
    {
      var path = DiplomataEditorData.resourcesFolder + folderName;

      if (!Directory.Exists(path) && folderName != "")
      {
        Directory.CreateDirectory(path);

        AssetDatabase.Refresh();
      }
    }
  }
}
