using System;
using Diplomata.Models;
using Diplomata.Persistence;
using UnityEngine;

namespace Diplomata.Persistence
{
  [Serializable]
  abstract public class Persistent
  {
    public override string ToString()
    {
      return JsonUtility.ToJson(this);
    }
  }
}
