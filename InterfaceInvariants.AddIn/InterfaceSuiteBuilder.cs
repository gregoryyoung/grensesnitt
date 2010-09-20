using System;
using NUnit.Core;
using System.Reflection;
using System.Collections.Generic;
using InterfaceInvariants.Framework;

namespace InterfaceInvariants.AddIn
{
	public class InterfaceSuiteBuilder : TestSuite
	{
		private IEnumerable<Type> GetInterfaceTypes(Type t) {
			Type[] ifaces = t.GetInterfaces();
			foreach(Type cur in ifaces) {
				if(cur.IsGenericType && cur.GetGenericTypeDefinition() == typeof(AppliesTo<>)) {
					yield return cur;
				}
			}
		}
		
        public InterfaceSuiteBuilder(Type fixtureType): base(fixtureType)
        {
		    var types = GetInterfaceTypes(fixtureType);
			foreach(Type t in types) {
				Console.WriteLine("Searching: " + t);
				var matchingTypes = TypeFinder.GetTypesMatchingInterface(t.GetGenericArguments()[0]);
				foreach(Type q in matchingTypes) Console.WriteLine("\n\n\nSearching for:" + q + "\n\n");
			    foreach (MethodInfo method in fixtureType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
	            {
					if(!method.IsConstructor) {
						foreach(Type toTest in matchingTypes) {
	                    	this.Add(new InstanceTestMethod(method, toTest, fixtureType));
						}
					}
	            }
			}
        }
    }
}

