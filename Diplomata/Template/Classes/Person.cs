using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diplomata
{
  [RequireComponent(typeof(DiplomataCharacter))]
  public class Person : MonoBehaviour
  {
    [HideInInspector] public DiplomataCharacter character;
    public Template template;

    void Start()
    {
      character = GetComponent<DiplomataCharacter>();
    }
  }
}
