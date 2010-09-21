using System;
using System.Collections.Generic;
namespace grensesnitt.AddIn
{
	public class TypeFinder
	{
		public static IEnumerable<Type> GetTypesMatchingInterface(Type t) {
			var ret = new List<Type>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(var assembly in assemblies) {
				foreach(Type test in assembly.GetTypes()) {
					if(test.IsClass && t.IsAssignableFrom(test) && t != test) {
						ret.Add(test);
					}
				}
			}
			return ret;
		}
	}
}

