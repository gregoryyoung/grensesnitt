using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using InterfaceInvariants.Framework;

namespace InterfaceInvariants.AddIn
{
	public class SearchPaths
	{
		public static IEnumerable<string> GetSearchPaths(Assembly assembly) {
			string origin = Path.GetDirectoryName(assembly.CodeBase);
			//Directory.SetCurrentDirectory(origin);
			object[] attributes = 
        			assembly.GetCustomAttributes(
        			typeof(InterfaceInvariantSearchLocationAttribute), false);			
			foreach(InterfaceInvariantSearchLocationAttribute attr in attributes) {
				yield return Path.GetFullPath(attr.Path);
			}
		}
	}
}

