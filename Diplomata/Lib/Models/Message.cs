using System;
using System.Security.Cryptography;
using System.Text;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Message
  {
    [SerializeField]
    private string uniqueId;

    public int id;
    public bool isAChoice;
    public bool disposable;
    public int columnId;
    public string imagePath = string.Empty;
    public Condition[] conditions;
    public LanguageDictionary[] content;
    public LanguageDictionary[] screenplayNotes;
    public AttributeDictionary[] attributes;
    public Effect[] effects;
    public AnimatorAttributeSetter[] animatorAttributesSetters;
    public LanguageDictionary[] audioClipPath;
    public string labelId;

    [NonSerialized]
    public bool alreadySpoked;

    [NonSerialized]
    public AudioClip audioClip;

    [NonSerialized]
    public Texture2D image;

    [NonSerialized]
    public Sprite sprite;

    public Message() {}

    public Message(Message msg, int id)
    {
      this.id = id;
      isAChoice = msg.isAChoice;
      disposable = msg.disposable;
      columnId = msg.columnId;
      imagePath = msg.imagePath;
      labelId = msg.labelId;

      conditions = ArrayHelper.Copy(msg.conditions);
      content = ArrayHelper.Copy(msg.content);
      screenplayNotes = ArrayHelper.Copy(msg.screenplayNotes);
      attributes = ArrayHelper.Copy(msg.attributes);
      effects = ArrayHelper.Copy(msg.effects);
      animatorAttributesSetters = ArrayHelper.Copy(msg.animatorAttributesSetters);
      audioClipPath = ArrayHelper.Copy(msg.audioClipPath);

      uniqueId = SetUniqueId();
    }

    public Message(int id, string emitter, int columnId, string labelId)
    {
      conditions = new Condition[0];
      content = new LanguageDictionary[0];
      attributes = new AttributeDictionary[0];
      screenplayNotes = new LanguageDictionary[0];
      effects = new Effect[0];
      audioClipPath = new LanguageDictionary[0];
      animatorAttributesSetters = new AnimatorAttributeSetter[0];
      this.labelId = labelId;

      foreach (string str in DiplomataData.options.attributes)
      {
        attributes = ArrayHelper.Add(attributes, new AttributeDictionary(str));
      }

      foreach (Language lang in DiplomataData.options.languages)
      {
        content = ArrayHelper.Add(content, new LanguageDictionary(lang.name, "[ Message content here ]"));
        screenplayNotes = ArrayHelper.Add(screenplayNotes, new LanguageDictionary(lang.name, ""));
        audioClipPath = ArrayHelper.Add(audioClipPath, new LanguageDictionary(lang.name, string.Empty));
      }

      this.id = id;
      this.columnId = columnId;

      uniqueId = SetUniqueId();
    }

    private string SetUniqueId()
    {
      MD5 md5Hash = MD5.Create();

      byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(
        "Diplomata" + id + columnId +
        UnityEngine.Random.Range(-2147483648, 2147483647)
      ));

      StringBuilder sBuilder = new StringBuilder();

      for (int i = 0; i < data.Length; i++)
      {
        sBuilder.Append(data[i].ToString("x2"));
      }

      return sBuilder.ToString();
    }

    public string GetUniqueId()
    {
      return uniqueId;
    }

    public Sprite GetSprite(Vector2 pivot)
    {
      if (sprite == null)
      {
        image = (Texture2D) Resources.Load(imagePath);
        sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), pivot);
      }

      return sprite;
    }

    public static Message Find(Message[] messages, int rowId)
    {
      foreach (Message message in messages)
      {
        if (message.id == rowId)
        {
          return message;
        }
      }

      return null;
    }

    public static Message Find(Message[] messages, string uniqueId)
    {
      foreach (Message message in messages)
      {
        if (message.uniqueId == uniqueId)
        {
          return message;
        }
      }

      return null;
    }

    public static Message[] ResetIDs(Message[] array)
    {

      Message[] temp = new Message[0];

      for (int i = 0; i < array.Length + 1; i++)
      {
        Message msg = Find(array, i);

        if (msg != null)
        {
          temp = ArrayHelper.Add(temp, msg);
        }
      }

      for (int j = 0; j < temp.Length; j++)
      {
        if (temp[j].id == j + 1)
        {
          temp[j].id = j;
        }
      }

      return temp;
    }

    public Effect AddCustomEffect()
    {
      effects = ArrayHelper.Add(effects, new Effect());
      return effects[effects.Length - 1];
    }
  }

}
