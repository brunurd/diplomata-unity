using System.Collections.Generic;
using UnityEngine;

namespace DiplomataLib
{
  [AddComponentMenu("")]
  [ExecuteInEditMode]
  public class Diplomata : MonoBehaviour
  {
    public static Diplomata instance = null;
    public static Preferences preferences = new Preferences();
    public static GameProgress gameProgress = new GameProgress();
    public static List<Character> characters = new List<Character>();
    public static Inventory inventory = new Inventory();
    public static CustomFlags customFlags = new CustomFlags();

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
      preferences = new Preferences();
      var json = (TextAsset) Resources.Load("Diplomata/preferences");

      if (json != null)
      {
        preferences = JsonUtility.FromJson<Preferences>(json.text);
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

      customFlags = new CustomFlags();
      json = (TextAsset) Resources.Load("Diplomata/customFlags");

      if (json != null)
      {
        customFlags = JsonUtility.FromJson<CustomFlags>(json.text);
      }

      gameProgress = new GameProgress();
      gameProgress.Start();
    }
  }
}
