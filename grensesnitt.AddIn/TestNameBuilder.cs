using System;
using System.Reflection;

namespace grensesnitt.AddIn
{
  public class TestNameBuilder
  {

    public static string GetTestName(MethodInfo method, Type targetType)
    {
      return method.Name;
    }

    public static string GetFullTestName(MethodInfo method, Type targetType)
    {
      return method.DeclaringType.FullName + "(" + targetType.FullName + ")";
    }
  }
}

