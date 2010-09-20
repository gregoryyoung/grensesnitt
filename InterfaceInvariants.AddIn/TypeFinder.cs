using System;
using System.Collections.Generic;
namespace InterfaceInvariants.AddIn
{
	public class TypeFinder
	{
		public static IEnumerable<Type> GetTypesMatchingInterface(Type t) {
			var ret = new List<Type>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(var assembly in assemblies) {
				foreach(Type test in assembly.GetTypes()) {
					if(test.IsClass && t.IsAssignableFrom(test) && t != test) {
						Console.WriteLine("\n\n\n" + test + " is class etc\n\n\n");
						ret.Add(test);
					}
				}
			}
			return ret;
		}
	}
}

