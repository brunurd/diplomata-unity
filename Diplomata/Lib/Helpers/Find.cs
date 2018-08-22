using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Diplomata.Helpers
{
  public class Subject
  {
    private Type type;
    private Array collection;
    private List<object> results;

    public object[] Results
    {
      get
      {
        try
        {
          if (results.Count < 1) throw new IndexOutOfRangeException();
          return results.ToArray();
        }
        catch (Exception e)
        {
          Debug.LogError(string.Format("No results found. {0}", e.Message));
          return new object[] { null };
        }
      }
      private set
      {
        foreach (object result in value)
          results.Add(result);
      }
    }

    public Subject(object[] collection)
    {
      results = new List<object>();
      this.collection = collection;
      type = collection[0] != null ? collection[0].GetType() : typeof(object);
    }

    public Subject Where(string fieldName, object value)
    {
      var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      foreach (var field in fields)
      {
        if (field.Name == fieldName)
        {
          foreach (var instance in collection)
          {
            if (field.GetValue(instance).Equals(value))
            {
              results.Add(instance);
            }
          }
        }
      }

      return this;
    }
  }

  public static class Find
  {
    public static Subject In(object[] collection)
    {
      return new Subject(collection);
    }
  }
}
