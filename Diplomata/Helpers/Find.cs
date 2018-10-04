using System;
using System.Collections.Generic;
using System.Reflection;

namespace LavaLeak.Diplomata.Helpers
{
  /// <summary>
  /// The main Find class.
  /// </summary>
  public static class Find
  {
    /// <summary>
    /// Find something in a array.
    /// </summary>
    /// <param name="collection">The array collection.</param>
    /// <returns>A <seealso cref="Diplomata.Helpers.Subject" /> object.</returns>
    public static Subject In(object[] collection)
    {
      return new Subject(collection);
    }
  }

  /// <summary>
  /// The subject class with the Results of a search.
  /// </summary>
  public class Subject
  {
    // TODO: Consider arrays: create a method to bring a new Subject from a array field.
    private readonly Type _type;
    private readonly Array _collection;
    private readonly List<object> _results;

    /// <summary>
    /// The result array.
    /// </summary>
    /// <value>A array of objects.</value>
    public object[] Results
    {
      get
      {
        return _results.Count < 1 ? new object[0] : _results.ToArray();
      }
      private set
      {
        foreach (var result in value)
          _results.Add(result);
      }
    }

    /// <summary>
    /// The first result with of the results array.
    /// </summary>
    /// <value>A result object.</value>
    public object Result
    {
      get
      {
        return Results.Length >= 1 ? Results[0] : null;
      }
      private set
      {
        _results[0] = value;
      }
    }

    /// <summary>
    /// The subject constructor with a array of results to preset.
    /// </summary>
    /// <param name="collection">The array of results.</param>
    public Subject(object[] collection)
    {
      _results = new List<object>();
      _collection = collection;
      if (collection.Length > 0)
      {
        _type = collection[0] != null ? collection[0].GetType() : null;
      }
    }

    /// <summary>
    /// Find a array of object with a field name and a value.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="value">The value to find in the array.</param>
    /// <returns>A subject with preset results.</returns>
    public Subject Where(string fieldName, object value)
    {
      if (_type == null) return this;
      var fields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

      foreach (var field in fields)
      {
        if (field.Name != fieldName) continue;
        foreach (var instance in _collection)
        {
          if (field.GetValue(instance).Equals(value))
          {
            _results.Add(instance);
          }
        }
      }
      return this;
    }

    /// <summary>
    /// Find a bunch of results from a field in a array.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The results of all the fields.</returns>
    public Subject Where(string fieldName)
    {
      if (_type != null)
      {
        var fields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

        foreach (var field in fields)
        {
          if (field.Name != fieldName) continue;
          foreach (var instance in _collection)
          {
            _results.Add(field.GetValue(instance));
          }
        }
      }
      return this;
    }
  }
}
