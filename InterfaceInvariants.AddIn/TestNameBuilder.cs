using System;
using System.Reflection;

namespace InterfaceInvariants.AddIn
{
  public class TestNameBuilder
  {

    public static string GetTestName(MethodInfo method, Type targetType)
    {
      return targetType.Name + "(" + method.Name + ")";
    }

    public static string GetFullTestName(MethodInfo method, Type targetType)
    {
      return targetType.FullName + "(" + method.DeclaringType.FullName + ")";
    }
  }
}

