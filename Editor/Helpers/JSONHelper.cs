using System;
using System.IO;
using LavaLeak.Diplomata.Editor;
using LavaLeak.Diplomata.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Helpers
{
  public class JSONHelper
  {
    public static string resourcesFolder = "Assets/Resources/";

    public static void Create(System.Object obj, string filename, bool prettyPrint = false, string folder = "")
    {
      try
      {
        CreateFolder(folder);

        var file = File.Create(resourcesFolder + folder + filename + ".json");
        file.Close();

        AssetDatabase.Refresh();

        Update(obj, filename, prettyPrint, folder);
      }

      catch (Exception e)
      {
        Debug.LogError("Cannot create " + folder + filename + ".json in " + resourcesFolder + ". " + e.Message);
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

        using(FileStream fs = new FileStream(resourcesFolder + folder + filename + ".json", FileMode.Create))
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
      var path = resourcesFolder + folder + filename;

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
      if (!File.Exists(resourcesFolder + filename + ".json") &&
        !File.Exists(resourcesFolder + folder + filename + ".json"))
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
      var path = resourcesFolder + folderName;

      if (!Directory.Exists(path) && folderName != "")
      {
        Directory.CreateDirectory(path);

        AssetDatabase.Refresh();
      }
    }
  }
}
