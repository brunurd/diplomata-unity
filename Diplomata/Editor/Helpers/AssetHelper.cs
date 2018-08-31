using System.IO;
using Diplomata.Editor;
using Diplomata.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Helpers
{
  public class AssetHelper
  {
    private static string resourcesFolder = "Assets/Resources/";

    public static void Create(Object obj, string name, string folder = "")
    {
      string path = resourcesFolder + folder + name;

      try
      {
        CreateFolder(folder);
        AssetDatabase.CreateAsset(obj, path);
        AssetDatabase.Refresh();
      }

      catch (System.Exception e)
      {
        Debug.LogError("Cannot create this asset. Review the path: \"" + path + "\". " + e.Message);
      }
    }

    public static Object Read(string name, string folder = "")
    {
      string path = resourcesFolder + folder + name;

      try
      {
        return AssetDatabase.LoadAssetAtPath(path, typeof(Object));
      }

      catch (System.Exception e)
      {
        Debug.LogError("This asset doesn't exist. Review the path: \"" + path + "\". " + e.Message);
        return null;
      }
    }

    public static void Delete(string name, string folder = "")
    {
      string path = resourcesFolder + folder + name;

      try
      {
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.Refresh();
      }

      catch (System.Exception e)
      {
        Debug.LogError("Cannot access the path: \"" + path + "\". " + e.Message);
      }
    }

    public static bool Exists(string filename, string folder = "")
    {
      if (!File.Exists(resourcesFolder + filename) &&
        !File.Exists(resourcesFolder + folder + filename))
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
