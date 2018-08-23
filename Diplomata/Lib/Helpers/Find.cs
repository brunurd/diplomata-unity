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

    public object Result
    {
      get
      {
        return Results[0];
      }
      private set
      {
        results[0] = value;
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

    public Subject In(string collectionName)
    {
      var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      foreach (var field in fields)
      {
        if (field.Name == collectionName)
        {
          foreach (var instance in collection)
          {
            if (field.GetType() == typeof(Array))
            {
              return new Subject((object[]) field.GetValue(instance));
            }
            if (field.GetType() == typeof(List<object>))
            {
              var list = (List<object>) field.GetValue(instance);
              return new Subject(list.ToArray());
            }
          }
        }
      }

      Debug.LogError(string.Format("No results found. for {0}.", collectionName));
      return null;
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
