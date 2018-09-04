using System;
using UnityEngine;

namespace LavaLeak.Diplomata.Models.Submodels
{
  /// <summary>
  /// Class to set a attribute value to a Animator.
  /// </summary>
  [Serializable]
  public class AnimatorAttributeSetter
  {
    [SerializeField]
    private string uniqueId = Guid.NewGuid().ToString();

    public AnimatorControllerParameterType type = AnimatorControllerParameterType.Float;
    public string name;
    public float setFloat;
    public int setInt;
    public bool setBool;
    public int setTrigger;
    public string animatorPath = string.Empty;

    [NonSerialized]
    public RuntimeAnimatorController animator;

    /// <summary>
    /// Get the unique id of the AnimatorAttributeSetter.
    /// </summary>
    /// <returns>The unique id (a string guid).</returns>
    public string GetId()
    {
      return uniqueId;
    }
  }
}
