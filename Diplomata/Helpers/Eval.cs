using System;
using System.Reflection;
using UnityEngine;

namespace Diplomata.Helpers
{
  public static class Eval
  {
    private static object GetStaticFieldValue(string className, string fieldName, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.GetField)
    {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (var assembly in assemblies)
      {
        foreach (var type in assembly.GetTypes())
        {
          if (type.Name == className)
          {
            var fields = type.GetFields(flags);
            foreach (var field in fields)
            {
              if (field.Name == fieldName)
              {
                return field.GetValue(null);
              }
            }
          }
        }
      }
      throw new Exception(string.Format("Field \"{0}.{1}\" value not found for evaluation.", className, fieldName));
    }

    private static object GetStaticPropertyValue(string className, string propertyName, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.GetProperty)
    {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (var assembly in assemblies)
      {
        foreach (var type in assembly.GetTypes())
        {
          if (type.Name == className)
          {
            var properties = type.GetProperties(flags);
            foreach (var property in properties)
            {
              if (property.Name == propertyName)
              {
                return property.GetValue(null,null);
              }
            }
          }
        }
      }
      throw new Exception(string.Format("Propety \"{0}.{1}\" value not found for evaluation.", className, propertyName));
    }
  }
}
