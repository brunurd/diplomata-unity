using System.Collections.Generic;
using Diplomata.Preferences;
using Diplomata.GameProgess;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata
{
  [AddComponentMenu("")]
  [ExecuteInEditMode]
  public class DiplomataManager : MonoBehaviour
  {
    public static DiplomataManager instance = null;
    public static Options options = new Options();
    public static GameProgressManager gameProgress = new GameProgressManager();
    public static List<Character> characters = new List<Character>();
    public static Inventory inventory = new Inventory();
    public static GlobalFlags globalFlags = new GlobalFlags();

    private void Awake()
    {
      if (instance == null)
      {
        instance = this;

        if (Application.isPlaying)
        {
          DontDestroyOnLoad(gameObject);
        }

        Restart();
      }

      else
      {
        DestroyImmediate(gameObject);
      }
    }

    public static void Restart()
    {
      options = new Options();
      var json = (TextAsset) Resources.Load("Diplomata/preferences");

      if (json != null)
      {
        options = JsonUtility.FromJson<Options>(json.text);
      }

      characters = new List<Character>();
      Character.UpdateList();

      inventory = new Inventory();
      json = (TextAsset) Resources.Load("Diplomata/inventory");

      if (json != null)
      {
        inventory = JsonUtility.FromJson<Inventory>(json.text);
      }

      inventory.SetImagesAndSprites();

      globalFlags = new GlobalFlags();
      json = (TextAsset) Resources.Load("Diplomata/globalFlags");

      if (json != null)
      {
        globalFlags = JsonUtility.FromJson<GlobalFlags>(json.text);
      }

      gameProgress = new GameProgressManager();
      gameProgress.Start();
    }
  }
}
