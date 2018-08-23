using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Diplomata.Helpers
{
  public class Subject
  {
    // TODO: Consider arrays: create a method to bring a new Subject from a array field.

    private Type type;
    private Array collection;
    private List<object> results;

    public object[] Results
    {
      get
      {
        if (results.Count < 1) return new object[] { null };
        return results.ToArray();
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
      if (collection.Length > 0)
      {
        type = collection[0] != null ? collection[0].GetType() : null;
      }
    }

    public Subject Where(string fieldName, object value)
    {
      if (type != null)
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
