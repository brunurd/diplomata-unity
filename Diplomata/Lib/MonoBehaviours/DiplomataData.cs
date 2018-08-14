using System.Collections.Generic;
using System.Linq;
using Diplomata.GameProgess;
using Diplomata.Interfaces;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata
{
  [AddComponentMenu("")]
  [ExecuteInEditMode]
  public class DiplomataData : MonoBehaviour
  {
    private static DiplomataData instance = null;
    public static Options options = new Options();
    public static GameProgressManager gameProgress = new GameProgressManager();
    public static List<Character> characters = new List<Character>();
    public static Inventory inventory = new Inventory();
    public static GlobalFlags globalFlags = new GlobalFlags();
    public static IEnumerable<ITalk> iTalks;
    public static IEnumerable<IChoice> iChoices;
    public static IEnumerable<IMessage> iMessages;
    public static bool isTalking;

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

    public static void SetData()
    {
      if (instance == null && FindObjectsOfType<DiplomataData>().Length < 1)
      {
        GameObject obj = new GameObject("[Diplomata]");
        obj.AddComponent<DiplomataData>();
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

      iTalks = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<ITalk>();
      iChoices = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IChoice>();
      iMessages = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<IMessage>();
    }
  }
}
