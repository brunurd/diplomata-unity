using System;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class AnimatorAttributeSetter
  {
    // [SerializeField] private string uniqueId = Guid.NewGuid().ToString();
    public AnimatorControllerParameterType type = AnimatorControllerParameterType.Float;
    public string name;
    public float setFloat;
    public int setInt;
    public bool setBool;
    public int setTrigger;
    public string animatorPath = string.Empty;

    [NonSerialized]
    public RuntimeAnimatorController animator;
  }
}
